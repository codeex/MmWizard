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

namespace MmWizard.Controllers
{
    public class HomeController : Controller
    {
        private IDbService _db;
        public HomeController(IDbService db)
        {
            this._db = db;
        }
        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Article()
        {
            using (var conn = _db.GetConn())
            {
                var a = conn.Conn.Query<Article>(SiteConfig.GetSql("GetBigClass"));
            }

            return View();
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
