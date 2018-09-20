using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MmWizard.Db;
using MmWizard.Helper;
using MmWizard.Models;
using MmWizard.Protocol;

namespace MmWizard.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    // [EnableCors("AllowAll")]
    public class ArticleController : MyController
    {
        public ArticleController(IDbService db):
            base(db)
        {
            
        }

        public Result<PageResult<Article>> GetArticlePage(Args<SearchParameters> args)
        {
            return Execute(() =>
            {
                using (var conn = this._db.GetConn())
                {
                    var list = conn.QueryByPage<Article>("GetArticlePage", args?.v);
                    return new PageResult<Article>
                    {
                        ListValue = list,
                        Page = args?.v?.PageInfo
                    };
                }

            });
        }
    }
}