using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MmWizard.Db;
using MmWizard.Models;
using Dapper;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Controllers;
using MmWizard.Middle;
using MmWizard.Protocol;

namespace MmWizard.Controllers
{
    public class HomeController : Controller
    {
        private IDbService _db;
        /// <summary>
        /// 如果调用其他控制器，需要启用 services.AddMvc().AddControllersAsServices();
        /// 此方式一般不推荐，如果有共用的方法调用，大可封装为共用的注入方法，两个控制器都引用
        /// 此处偷懒了，采用mvc控制器调用api控制器方式，保持与前端调用api控制器的一致性
        /// </summary>
        private ArticleController _controller;
        public HomeController(IDbService db, ArticleController controller)
        {
            this._db = db;
            this._controller = controller;
        }
        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Article(SearchParameters sp)
        {
            if (sp?.PageInfo?.IsGetTotalCount != null)
            {
                sp.PageInfo.IsGetTotalCount = true;
            }

            if (sp == null)
            {
                sp = new SearchParameters()
                {
                    PageInfo = new PageInfo
                    {
                        IsGetTotalCount = true,
                        PageSize = 50
                    }
                };
            }
            
            var ret = this._controller.GetArticlePage(new Args<SearchParameters>
            {
                v = sp,
            });

            return View(ret);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
