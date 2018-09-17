using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MmWizard.Models;
using StackExchange.Redis;

namespace MmWizard.Helper
{
    public class RedisConnectionManager
    {
        private static readonly object _lockObj = new object();
        private static readonly Dictionary<string, ConnectionMultiplexer> ConnectionCache = new Dictionary<string, ConnectionMultiplexer>();

        public static IDatabase GetDatabase(RedisServer srvCfg, int dbIndex = 0)
        {
            int error = 0;
            gotoHere:
            ConnectionMultiplexer conn = null;
            try
            {
                conn = GetConnection(srvCfg);
                IDatabase db = conn?.GetDatabase();
                return db;
            }
            catch (Exception ex)
            {
                if (error < 2)//出错可以重试两次
                {
                    if (conn != null)
                    {
                        RedisConnectionManager.Dispose(srvCfg, conn);  // conn = null;//把这个链接设置为空，防止第二次还是被取出来
                    }

                    error += 1;
                    System.Threading.Thread.Sleep(1000);
                    goto gotoHere;
                }
                Logger?.Log(LogLevel.Error, new Exception("RedisNode.GetDatabase.Error", ex), "", null);
                return null;
            }
        }

        public static ILogger Logger { get; set; }

        /// <summary>
        /// 单例获取
        /// </summary>
        private static ConnectionMultiplexer GetConnection(RedisServer srvCfg)
        {
            // var date = DateTime.Now;
            lock (_lockObj)
            {
                // 业务模型节点+索引确定一个连接
                string key = string.Format("{0}_{1}", srvCfg.Server, srvCfg.Port);
                ConnectionMultiplexer conn = null;
                if (ConnectionCache.ContainsKey(key))
                {
                    conn = ConnectionCache[key];
                }

                if (conn == null || !conn.IsConnected)
                {
                    if (conn != null)
                    {
                        conn.Dispose(); // 如果一个连接断了，必须释放掉，才能重新建
                    }

                    // Console.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff") + "---Wait---" + DateTime.Now.Subtract(date).TotalMilliseconds);
                    conn = getConnection(srvCfg);
                    ConnectionCache[key] = conn;
                }

                return conn;
            }
        }

        /// <summary>
        /// 释放掉坏掉的连接
        /// </summary>
        /// <param name="srvCfg">srvCfg</param>
        /// <param name="conn">conn</param>
        private static void Dispose(RedisServer srvCfg, ConnectionMultiplexer conn)
        {
            lock (_lockObj)
            {
                try
                {
                    if (conn != null)
                    {
                        conn.Dispose();
                        conn = null;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Log(LogLevel.Error,new Exception("RedisConnectionManager.Dispose.Error", ex),"",null);
                }

                string key = string.Format("{0}_{1}", srvCfg.Server, srvCfg.Port);

                if (ConnectionCache.ContainsKey(key))
                {
                    ConnectionCache.Remove(key);
                }
            }
        }

        private static ConnectionMultiplexer getConnection(RedisServer cfg)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}:{1}", cfg.Server, cfg.Port);
                sb.AppendFormat(",allowAdmin={0}", cfg.AllowAdmin);
                sb.AppendFormat(",abortConnect=false,connectRetry=3,syncTimeout=3000");
                sb.AppendFormat(",connectTimeout={0}", cfg.ConnectTimeout > 0 ? cfg.ConnectTimeout : 5000);
                if (cfg.Ssl)
                {
                    // sb.AppendFormat(",ssl={0},password={1}", cfg.Ssl,cfg.Pwd);
                    sb.AppendFormat(",password={1}", cfg.Ssl, cfg.Pwd);
                }

                //return new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(sb.ToString())).Value;
                return ConnectionMultiplexer.Connect(sb.ToString());
            }
            catch (Exception ex)
            {
                Logger?.Log(LogLevel.Error, new Exception("redis连接创建失败", ex), "", null);
                return null;
            }
        }
    }
}
