using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MmWizard.Db;
using MmWizard.Models;
using Dapper;

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
            for (int i = 0; i < 100; i++)
            {
                var conn = _db.GetConn();
                {
                    var a = conn.Conn.Query<Article>("select * from article");
                }
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
