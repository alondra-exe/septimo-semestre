using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
////
using MVC2Unidad3Actividad.Models;

namespace MVC2Unidad3Actividad.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            villancicosContext context = new villancicosContext();
            var villancico = context.Villancico.OrderBy(x => x.Nombre).ToList();
            return View(villancico);
        }

        public IActionResult Villancico(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("index");
            }

            villancicosContext context = new villancicosContext();
            var villancico = context.Villancico.FirstOrDefault(x => x.Id == id);
            if (villancico == null)
            {
                return RedirectToAction("index");
            }
            else
            {
                return View(villancico);
            }
        }
    }
}
