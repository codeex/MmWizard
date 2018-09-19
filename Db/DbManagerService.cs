using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MmWizard.Db
{
    public class DbManagerService : IDbService
    {
        private DbConnectionManager _manager;
        public DbManagerService(DbConnOption dbInfo, ILogger logger = null, int initConnNum = 5)
        {
            _manager = new DbConnectionManager(dbInfo, initConnNum);
            _manager.Logger = logger;
        }
        public DbConnectionWrapper GetConn()
        {
            return _manager.GetConnection();
        }
    }
}
