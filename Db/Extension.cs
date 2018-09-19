using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MmWizard.Db
{
    public static class Extension
    {
        public static IServiceCollection AddDbService(this IServiceCollection service, DbConnOption dbInfo, int initConnNum = 5)
        {
            var logger = service.BuildServiceProvider().GetService<ILoggerFactory>()?.CreateLogger("DB");
            return service.AddSingleton<IDbService>(factory => new DbManagerService(dbInfo, logger, initConnNum));
        }
    }
}
