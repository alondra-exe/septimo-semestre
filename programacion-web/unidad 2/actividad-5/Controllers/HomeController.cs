using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCUnidad2Actividad4.Models;
using Microsoft.EntityFrameworkCore;
using MVCUnidad2Actividad4.Models.ViewModels;

namespace MVCUnidad2Actividad4.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            animalesContext context = new animalesContext();
            var clasificacion = context.Clase.OrderBy(x => x.Nombre);
            Random r = new Random();
            return View(clasificacion);
        }

        [Route("/Especie")]
        public IActionResult Especie()
        {
            animalesContext context = new animalesContext();
            var especies = context.Clase.Include(x => x.Especies).OrderBy(x=>x.Nombre)
                .Select(x => new EspeciesViewModel { NombreEspecie = x.Nombre, Especies = x.Especies });
            return View(especies);
        }
    }
}
