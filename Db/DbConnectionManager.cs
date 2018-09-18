using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MmWizard.Db
{
    public class DbConnectionManager
    {
        private object _lockObject = new object();
        private List<DbConnectionWrapper> _dbCanUse = new List<DbConnectionWrapper>();
        private List<DbConnectionWrapper> _dbUsed = new List<DbConnectionWrapper>();
        private DbConnOption _connInfo;
        private int _initConn;
        private int _checkConnTimeout = 60 * 1000; //60s
        

        /// <summary>
        /// 使用连接信息构造一个数据库连接管理类
        /// </summary>
        /// <param name="connInfo"></param>
        /// <param name="initConn">初始连接</param>
        public DbConnectionManager(DbConnOption connInfo, int initConn = 5)
        {
            this._initConn = initConn;
            this._connInfo = new DbConnOption(connInfo.DbConnectionString,connInfo.DbConnectionType);
            if(initConn > 0)
            {
                DbConnectionWrapper[] arrConn = new DbConnectionWrapper[initConn];
                Parallel.For(0, initConn, x =>
                {
                    arrConn[x] = this.CreateNew();
                });
                this._dbCanUse.AddRange(arrConn.Where(x => x != null));
            }

            //开启线程，定时检查释放多余的连接
            Thread th = new Thread(CheckConn)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
                Name = "检查数据库连接"
            };
            th.Start(this);
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// 得到一个连接,保证可用
        /// </summary>
        /// <returns></returns>
        public DbConnectionWrapper GetConnection()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            lock (this._lockObject)
            {
                int tryNum = this._dbCanUse.Count;
                //随机返回一个
                if (this._dbCanUse.Count > 0)
                {
                    checkCanUse:
                    var dbw = _dbCanUse[rnd.Next(0, this._dbCanUse.Count-1)];
                    if (!dbw.CanUse())
                    {
                        if (--tryNum > 0)
                        {
                            goto checkCanUse;
                        }
                        else
                        {
                            goto createNew;
                        }
                    }

                    this._dbCanUse.Remove(dbw);
                    this._dbUsed.Add(dbw);
                    dbw.MarkAccessed();
                    return dbw;
                }                
            }

            createNew:
            //如果走到这里，说明没有拿到连接,则建立一个
            var dbwNew = CreateNew();
            if(dbwNew == null)
            {
                throw new Exception($"数据库({this._connInfo.GetDescInfo()})无法取到连接!");
            }

            lock (this._lockObject)
            {
                this._dbUsed.Add(dbwNew);                
            }
            return dbwNew;

        }

        /// <summary>
        /// 关闭连接，返回连接池
        /// </summary>
        /// <param name="dbw"></param>
        public void CloseConn(DbConnectionWrapper dbw)
        {
            if (dbw == null) return;

            dbw.MarkAccessed();
            lock (this._lockObject)
            {
                if(this._dbUsed.Contains(dbw))
                {
                    this._dbUsed.Remove(dbw);
                    //如果连接已经不能用，则释放掉
                    if (dbw.CanUse())
                    {
                        this._dbCanUse.Add(dbw);
                    }
                    else
                    {
                        try
                        {
                            dbw.Conn?.Close();
                        }
                        catch { }
                    }
                }
                else
                {
                    this._dbCanUse.Add(dbw);
                }
            }
        }

        /// <summary>
        /// 定时检查连接，并释放
        /// </summary>
        /// <param name="obj"></param>
        protected void CheckConn(object obj)
        {
            var m = obj as DbConnectionManager;
            if (m == null) return;

            List<DbConnectionWrapper> removeList = null;
            while (true)
            {
                Random rnd = new Random(DateTime.Now.Millisecond);
                lock (m._lockObject)
                {
                    //检查 能用的连接列表
                    int mayDelete = m._dbCanUse.Count - m._initConn;
                    var arr = m._dbCanUse.Where(x => x.CanUse()).ToList();
                    removeList = m._dbCanUse.Where(x => !x.CanUse()).ToList();
                    if (arr.Count >= m._initConn && removeList.Count > 0)
                    {
                        //一次仅删除一个，缓慢释放
                        removeList = removeList.Take(1).ToList();
                        removeList.ForEach(x => m._dbCanUse.Remove(x));
                    }
                    else if (arr.Count >= m._initConn && mayDelete > 0)
                    {
                        removeList = m._dbCanUse.Where(x => x.IdleTime(m._checkConnTimeout)).Take(1).ToList();
                        removeList.ForEach(x => m._dbCanUse.Remove(x));
                    }
                    else if (arr.Count < m._initConn && removeList.Count > 0)
                    { //先重建1连接

                        var dbw = removeList[rnd.Next(0, removeList.Count - 1)];
                        try { 
                            dbw.Conn?.OpenAsync();
                            }
                        catch { }
                        removeList.Clear();
                    }

                    //检查正在用的列表，超时的一律删除,一次删除一个
                    var tw = m._dbUsed.FirstOrDefault(x => x.IdleTime(m._checkConnTimeout));
                    if(tw != null)
                    {
                        m._dbUsed.Remove(tw);
                        removeList.Add(tw);
                    }

                }

                Trace.WriteLine($"数据库可用连接：{m._dbCanUse.Count}");
                removeList?.ForEach(x =>
                {
                    try {
                        x.Conn?.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex, $"数据库({m._connInfo.GetDescInfo()})连接关闭失败", null);
                    }
                });

                Thread.Sleep(150);
            }
        }
        /// <summary>
        /// 建立新的连接
        /// </summary>
        /// <returns>null</returns>
        private DbConnectionWrapper CreateNew()
        {
            if(this._connInfo == null)
            {
                return null;
            }

            int tryConn = 0;

            tryConn:
            try
            {
                DbConnection conn = null;
                switch (this._connInfo.DbConnectionType)
                {
                    case "mysql":
                        conn = new MySqlConnection(this._connInfo.ToString());
                        conn.Open();
                        break;
                    default:
                        Logger?.LogError($"数据库连接建立不支持[{this._connInfo.DbConnectionType}]", null);
                        break;
                }

                return new DbConnectionWrapper(this, conn);
            }
            catch(Exception ex)
            {
                Logger?.LogWarning(ex, $"数据库({this._connInfo.GetDescInfo()})连接重试[{tryConn}]",null);
                Random rnd = new Random(DateTime.Now.Millisecond);
                if(++tryConn < 3)
                {                    
                    Thread.Sleep(rnd.Next(800,15000));
                    goto tryConn;
                }
                Logger?.LogError($"数据库({this._connInfo.GetDescInfo()})连接失败[{tryConn}]", null);

                return null;
            }
        }
    }
}
