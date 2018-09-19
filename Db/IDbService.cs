using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Db
{
    public interface IDbService
    {
        DbConnectionWrapper GetConn();
    }
}
