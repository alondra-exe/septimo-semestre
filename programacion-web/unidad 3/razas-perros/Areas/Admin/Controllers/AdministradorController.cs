using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RazasPerros.Models;
using RazasPerros.Models.ViewModels;
using RazasPerros.Repositories;

namespace RazasPerros.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdministradorController : Controller
    {
        public IWebHostEnvironment Environment { get; set; }
        public AdministradorController(IWebHostEnvironment env)
        {
            Environment = env;
        }
        public IActionResult Index()
        {
            sistem14_razasContext context = new sistem14_razasContext();
            RazasRepository repos = new RazasRepository(context);
            return View(repos.GetAll().Where(x => x.Eliminado == false));
        }
        public IActionResult Agregar()
        {
            RazaViewModel vm = new RazaViewModel();
            sistem14_razasContext context = new sistem14_razasContext();
            PaisesRepository paisesRepository = new PaisesRepository(context);
            vm.Paises = paisesRepository.GetAll();
            return View(vm);
        }
        [HttpPost]
        public IActionResult Agregar(RazaViewModel vm)
        {
            sistem14_razasContext context = new sistem14_razasContext();
            if (vm.Archivo.ContentType != "image/jpeg" || vm.Archivo.Length > 1024 * 1024 * 2)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo tipo .jpg menor de 2MB.");
                PaisesRepository paisesRepository = new PaisesRepository(context);
                vm.Paises = paisesRepository.GetAll();
                return View(vm);
            }
            RazasRepository razasRepository = new RazasRepository(context);
            try
            {
                vm.Paises = razasRepository.GetPaises();
                razasRepository.Insert(vm.Raza);
                FileStream fs = new FileStream
                   (Environment.WebRootPath + "/imgs_perros/" + vm.Raza.Id + "_0.jpg", FileMode.Create);
                vm.Archivo.CopyTo(fs);
                fs.Close();
                return RedirectToAction("Index", "Administrador");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.Paises = razasRepository.GetPaises();
                return View(vm);
            }
        }
        public IActionResult Editar(uint id)
        {
            sistem14_razasContext context = new sistem14_razasContext();
            RazaViewModel vm = new RazaViewModel();
            RazasRepository razasRepository = new RazasRepository(context);
            vm.Paises = razasRepository.GetPaises();
            vm.Raza = razasRepository.GetRazaById(id);
            if (vm.Raza == null)
            {
                return RedirectToAction("Index", "Administrador");
            }
            if (System.IO.File.Exists(Environment.WebRootPath + "/imgs_perros/" + vm.Raza.Id + "_0.jpg"))
            {
                vm.Imagen = vm.Raza.Id + "_0.jpg";
            }
            else
            {
                vm.Imagen = "NoPhoto.jpg";
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult Editar(RazaViewModel vm)
        {
            sistem14_razasContext context = new sistem14_razasContext();
            if (vm.Archivo != null)
            {
                if (vm.Archivo.ContentType != "image/jpeg" || vm.Archivo.Length > 1024 * 1024 * 2)
                {
                    ModelState.AddModelError("", "Debe seleccionar un archivo jpg de menos de 2MB.");
                    PaisesRepository paisesRepository = new PaisesRepository(context);
                    vm.Paises = paisesRepository.GetAll();
                    return View(vm);
                }
            }
            try
            {
                RazasRepository razasRepository = new RazasRepository(context);
                var raza = razasRepository.GetRazaById(vm.Raza.Id);
                vm.Paises = razasRepository.GetPaises();
                if (raza != null)
                {
                    raza.Nombre = vm.Raza.Nombre;
                    raza.Descripcion = vm.Raza.Descripcion;
                    raza.OtrosNombres = vm.Raza.OtrosNombres;
                    raza.IdPais = vm.Raza.IdPais;
                    raza.PesoMin = vm.Raza.PesoMin;
                    raza.PesoMax = vm.Raza.PesoMax;
                    raza.AlturaMin = vm.Raza.AlturaMin;
                    raza.AlturaMax = vm.Raza.AlturaMax;
                    raza.EsperanzaVida = vm.Raza.EsperanzaVida;

                    raza.Caracteristicasfisicas.Patas = vm.Raza.Caracteristicasfisicas.Patas;
                    raza.Caracteristicasfisicas.Cola = vm.Raza.Caracteristicasfisicas.Cola;
                    raza.Caracteristicasfisicas.Hocico = vm.Raza.Caracteristicasfisicas.Hocico;
                    raza.Caracteristicasfisicas.Pelo = vm.Raza.Caracteristicasfisicas.Pelo;
                    raza.Caracteristicasfisicas.Color = vm.Raza.Caracteristicasfisicas.Color;

                    raza.Estadisticasraza.NivelEnergia = vm.Raza.Estadisticasraza.NivelEnergia;
                    raza.Estadisticasraza.FacilidadEntrenamiento = vm.Raza.Estadisticasraza.FacilidadEntrenamiento;
                    raza.Estadisticasraza.EjercicioObligatorio = vm.Raza.Estadisticasraza.EjercicioObligatorio;
                    raza.Estadisticasraza.AmistadDesconocidos = vm.Raza.Estadisticasraza.AmistadDesconocidos;
                    raza.Estadisticasraza.AmistadPerros = vm.Raza.Estadisticasraza.AmistadPerros;
                    raza.Estadisticasraza.NecesidadCepillado = vm.Raza.Estadisticasraza.NecesidadCepillado;
                    razasRepository.Update(raza);
                }
                if (vm.Archivo != null)
                {
                    FileStream fs = new FileStream
                        (Environment.WebRootPath + "/imgs_perros/" + vm.Raza.Id + "_0.jpg", FileMode.Create);
                    vm.Archivo.CopyTo(fs);
                    fs.Close();
                }
                return RedirectToAction("Index", "Administrador");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                PaisesRepository paisesRepository = new PaisesRepository(context);
                vm.Paises = paisesRepository.GetAll();
                return View(vm);
            }
        }
        public IActionResult Eliminar(uint id)
        {
            using (sistem14_razasContext context = new sistem14_razasContext())
            {
                RazasRepository repos = new RazasRepository(context);
                var objeto = repos.GetById(id);
                if (objeto != null)
                {
                    return View(objeto);
                }
                else
                    return RedirectToAction("Index", "Administrador");
            }
        }
        [HttpPost]
        public IActionResult Eliminar(Razas ra)
        {
            try
            {
                using (sistem14_razasContext context = new sistem14_razasContext())
                {
                    RazasRepository repos = new RazasRepository(context);
                    var raza = repos.GetRazaById(ra.Id);
                    raza.Eliminado = true;
                    repos.Update(raza);
                    return RedirectToAction("Index", "Administrador");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ra);
            }
        }
    }
}