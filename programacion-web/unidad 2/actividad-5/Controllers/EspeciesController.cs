using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCUnidad2Actividad4.Models;

namespace MVCUnidad2Actividad4.Controllers
{
    public class EspeciesController : Controller
    {
        [Route("/Especie/{id}")]
        public IActionResult Index(string Id)
        {
            animalesContext context = new animalesContext();
            var nombre = Id.Replace("-", " ").ToLower();
            var especies = context.Especies.Where
                (x => x.IdClaseNavigation.Nombre.ToLower() == Id.ToLower())
                .OrderBy(x => x.Especie);
            if (especies == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
                return View(especies);
        }
    }
}
