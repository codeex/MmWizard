using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Db
{
    public class DbConnOption
    {
        public DbConnOption(string conn, string cType = "mysql")
        {
            this.DbConnectionString = conn;
            DbConnectionType = cType;
        }

        public string DbConnectionString { get; set; }

        /// <summary>
        /// 目前仅支持mysql
        /// </summary>
        public string DbConnectionType { get; set; }

        public override string ToString()
        {
            return this.DbConnectionString;
        }

        /// <summary>
        /// 应该去掉敏感信息，日志输出用
        /// </summary>
        /// <returns></returns>
        public string GetDescInfo()
        {
            return this.DbConnectionString;
        }
    }
}
