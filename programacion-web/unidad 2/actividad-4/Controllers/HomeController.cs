using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCUnidad2Actividad4.Models;

namespace MVCUnidad2Actividad4.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [Route("/Peliculas")]
        public IActionResult Peliculas()
        {
            pixarContext context = new pixarContext();
            var pelicula = context.Pelicula.OrderBy(x => x.Nombre).ToList();
            return View(pelicula);
        }

        [Route("Peliculas/{id}")]
        public IActionResult DatosPelicula(string id)
        {
            var nombre = id.Replace("-", " ").ToLower();
            pixarContext context = new pixarContext();
            var pelicula = context.Pelicula.Include(x => x.Apariciones).
                FirstOrDefault(x => x.Nombre == nombre);
            var apariciones = context.Apariciones.Include
                (x => x.IdPersonajeNavigation).Include(x => x.IdPeliculaNavigation).
                Where(x => (x.IdPelicula == pelicula.Id)).Select(x => x);

            if (pelicula == null)
                return RedirectToAction("Peliculas");
            else
            {
                DatosPeliculaViewModel vm = new DatosPeliculaViewModel();
                vm.Nombre = pelicula.Nombre;
                vm.NombreOriginal = pelicula.NombreOriginal;
                vm.Id = pelicula.Id;
                vm.FechaEstreno = pelicula.FechaEstreno;
                vm.Descripcion = pelicula.Descripción;
                vm.Apariciones = apariciones;
                return View(vm);
            }
        }

        [Route("/Cortos")]
        public IActionResult Cortos()
        {
            pixarContext context = new pixarContext();
            var categoria = context.Categoria.Include(x => x.Cortometraje).
                OrderBy(x => x.Nombre).Select(x => new DatosCortoViewModel
                { Categoria = x.Nombre, Cortometrajes = x.Cortometraje });
            return View(categoria);
        }

        [Route("Cortos/{id}")]
        public IActionResult DatosCorto(string id)
        {
            var nombre = id.Replace("-", " ").ToLower();
            pixarContext context = new pixarContext();
            var corto = context.Cortometraje.
               FirstOrDefault(x => x.Nombre == nombre);
            if (corto == null)
                return RedirectToAction("Cortos");
            else
            {
                Cortometraje c = new Cortometraje();
                c.Id = corto.Id;
                c.Nombre = corto.Nombre;
                c.Descripcion = corto.Descripcion;
                return View(c);
            }
        }
    }
}
