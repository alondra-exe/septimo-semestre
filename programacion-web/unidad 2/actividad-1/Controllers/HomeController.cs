using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVC2Unidad1Actividad.Models;

namespace MVC2Unidad1Actividad.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Resultado(Suma s)
        {
            return View(s);
        }
    }
}
