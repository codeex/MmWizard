using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Db
{
    public class DbConnectionWrapper :IDisposable
    {
        public DbConnectionWrapper(DbConnectionManager manager, DbConnection conn)
        {
            this._manager = manager;
            this._dbConn = conn;
            LastAccessed = DateTime.Now;
        }

        private DbConnectionManager _manager;
        private DbConnection _dbConn;
        public DbConnection Conn {
            get
            {
                this.MarkAccessed();
                return _dbConn;
            }
        }

        /// <summary>
        /// 修改最后访问时间
        /// </summary>
        public void MarkAccessed()
        {
            LastAccessed = DateTime.Now;
        }

        /// <summary>
        /// 空闲时间超过给定时间
        /// </summary>
        /// <param name="timeout">秒</param>
        /// <returns>空闲</returns>
        public bool IdleTime(int timeout)
        {
            return (DateTime.Now - LastAccessed).TotalSeconds >= timeout;
        }

        /// <summary>
        /// 连接是否可用
        /// </summary>
        /// <returns></returns>
        public bool CanUse()
        {
            return this._dbConn?.State.HasFlag(ConnectionState.Open) ?? false;
        }
        public DateTime LastAccessed { get; set; }
        public void Dispose()
        {
            this._manager?.CloseConn(this);

            if(this._manager == null)
            {
                try
                {
                    this._dbConn?.Close();
                }
                catch { }
            }
        }
    }
}
