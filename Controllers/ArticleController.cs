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
    // [EnableCors("AllowAll")]
    public class ArticleController : MyController
    {
        public ArticleController(IDbService db):
            base(db)
        {
            
        }

        public Result<PageResult<Article>> GetArticlePage(SearchParameters args)
        {
            return Execute(() =>
            {
                using (var conn = this._db.GetConn())
                {
                    var list = conn.QueryByPage<Article>("GetArticlePage", args);
                    return new PageResult<Article>
                    {
                        ListValue = list,
                        Page = args?.PageInfo
                    };
                }

            });
        }
    }
}