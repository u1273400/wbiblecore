using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace wbible.Controllers
{
    public class SimpleController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "This is a simple controller.";
            return View();
        }

    }
}
