using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//
using MVC2Unidad2Actividad.Models;

namespace MVC2Unidad2Actividad.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            mexicoContext context = new mexicoContext();
            var estados = context.Estados.OrderBy(x => x.Nombre).ToList();
            return View(estados);
        }
    }
}
