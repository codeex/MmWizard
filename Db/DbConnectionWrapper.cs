using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MmWizard.Db
{
    public partial class DbConnectionWrapper :IDisposable
    {
        public DbConnectionWrapper(DbConnectionManager manager, DbConnection conn)
        {
            this._manager = manager;
            this._dbConn = conn;
            LastAccessed = DateTime.Now;
            InitThreadInfo();
        }

        private DbConnectionManager _manager;
        private DbConnection _dbConn;
        private int _refCount = 0;
        public DbConnection Conn {
            get
            {
                this.MarkAccessed();
                Trace.WriteLine($"db connection in {ThreadInfo.ThreadId}:{ThreadInfo.ThreadName},ref={RefCount}");
                return _dbConn;
            }
        }

        /// <summary>
        /// 线程信息
        /// </summary>
        public ThreadInfo ThreadInfo { get; private set; }

        private void InitThreadInfo()
        {
            this.ThreadInfo = new ThreadInfo
            {
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                ThreadName = Thread.CurrentThread.Name
            };
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        public int RefCount => _refCount;

        /// <summary>
        /// 修改最后访问时间
        /// </summary>
        private void MarkAccessed()
        {
            LastAccessed = DateTime.Now;
        }

        public void MarkRefUsed()
        {
            MarkAccessed();
            InitThreadInfo();
            Interlocked.Increment(ref _refCount);
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
            Interlocked.Decrement(ref _refCount);
            this._manager?.ReleaseConn(this);
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

    /// <summary>
    /// 线程信息
    /// </summary>
    public class ThreadInfo
    {
        public int ThreadId { get; set; }
        public string ThreadName { get; set; }
    }
}
