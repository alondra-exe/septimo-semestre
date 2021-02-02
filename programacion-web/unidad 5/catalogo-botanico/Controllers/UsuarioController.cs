using System;
using System.IO;
using System.Linq;
using CatalogoBotanico.Helpers;
using CatalogoBotanico.Models;
using CatalogoBotanico.Models.ViewModels;
using CatalogoBotanico.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoBotanico.Controllers
{
    public class UsuarioController : Controller
    {
        sistem14_botanicaContext context = new sistem14_botanicaContext();
        public IWebHostEnvironment Enviroment { get; set; }
        public UsuarioController(IWebHostEnvironment environment)
        {
            Enviroment = environment;
        }

        [Authorize(Roles = "User")]
        public IActionResult Catalogo(int id)
        {
            UserRepository repository = new UserRepository(context);
            var catalogo = repository.GetPlantByUser(id);
            if (catalogo != null)
            {
                if (User.IsInRole("User"))
                {
                    if (User.Claims.FirstOrDefault(x => x.Type == "Id").Value == catalogo.Id.ToString())
                    {
                        return View(catalogo);
                    }
                    else
                    {
                        return RedirectToAction("Denegado");
                    }
                }
                else
                {
                    return View(catalogo);
                }
            }
            else
            {
                return RedirectToAction("Catalogo", new { id });
            }
        }

        [Authorize(Roles = "User, Admin")]
        public IActionResult Tarjeta(int id)
        {
            PlantRepository repository = new PlantRepository(context);
            Plantdata plantdata = repository.GetPlantById(id);
            return View(plantdata);
        }

        [Authorize(Roles = "User, Admin")]
        public IActionResult Agregar(int id)
        {
            UserRepository repository = new UserRepository(context);
            PlantViewModel viewModel = new PlantViewModel();
            viewModel.User = repository.GetById(id);
            try
            {
                if (viewModel.User != null)
                {
                    return View(viewModel);
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(viewModel);
            }
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult Agregar(PlantViewModel viewModel)
        {
            UserRepository userRepos = new UserRepository(context);
            PlantRepository plantRepos = new PlantRepository(context);
            if (viewModel.Archivo.ContentType != "image/png" || viewModel.Archivo.Length > 1024 * 1024 * 2)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo tipo .png menor de 2MB.");
                return View(viewModel);
            }
            try
            {
                if (context.Plantdata.Any(x => x.Id == viewModel.Plant.Id))
                {
                    ModelState.AddModelError("!", "No se pueden repetir id.");
                    return View(viewModel);
                }
                else
                {
                    var user = userRepos.GetUserById(viewModel.User.Id).Id;
                    viewModel.Plant.IdUser = user;
                    plantRepos.Insert(viewModel.Plant);
                    FileStream fs = new FileStream(Enviroment.WebRootPath + "/imgs/" + viewModel.Plant.Id + ".png", FileMode.Create);
                    viewModel.Archivo.CopyTo(fs);
                    fs.Close();
                    if (User.IsInRole("User"))
                    {
                        return RedirectToAction("Catalogo", new { id = user });
                    }
                    else if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("CatalogoUsuario", "Admin", new { id = user });
                    }
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                viewModel.User = userRepos.GetById(viewModel.User.Id);
                viewModel.Users = userRepos.GetAll();
                ModelState.AddModelError("!", ex.Message);
                return View(viewModel);
            }
        }

        [Authorize(Roles = "User, Admin")]
        public IActionResult Editar(int id)
        {
            UserRepository userRepos = new UserRepository(context);
            PlantRepository plantRepos = new PlantRepository(context);
            PlantViewModel viewModel = new PlantViewModel();
            viewModel.Plant = plantRepos.GetPlantById(id);
            viewModel.Users = userRepos.GetAll();
            if (viewModel.Plant != null)
            {
                viewModel.User = userRepos.GetById(viewModel.Plant.IdUser);
                if (User.Claims.FirstOrDefault(x => x.Type == "Id").Value == viewModel.User.Id.ToString())
                {
                    if (System.IO.File.Exists(Enviroment.WebRootPath + "/imgs/" + id + ".png"))
                    {
                        viewModel.Image = id + ".png";
                    }
                    else
                    {
                        viewModel.Image = "no-disponible.png";
                    }
                    return View(viewModel);
                }
                return View(viewModel);
            }
            return RedirectToAction("Index", new { id = viewModel.Plant.IdUser });
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult Editar(PlantViewModel viewModel)
        {
            UserRepository userRepos = new UserRepository(context);
            PlantRepository plantRepos = new PlantRepository(context);
            var plant = plantRepos.GetById(viewModel.Plant.Id);
            try
            {
                if (plant != null)
                {
                    plant.CommonName = viewModel.Plant.CommonName;
                    plant.ScientificName = viewModel.Plant.ScientificName;
                    plant.Division = viewModel.Plant.Division;
                    plant.Family = viewModel.Plant.Family;
                    plant.Subfamily = viewModel.Plant.Subfamily;
                    plant.Gender = viewModel.Plant.Gender;
                    plant.Info = viewModel.Plant.Info;
                    plantRepos.Update(plant);
                    if (viewModel.Archivo != null)
                    {
                        FileStream fs = new FileStream(Enviroment.WebRootPath + "/imgs/" + viewModel.Plant.Id + ".png", FileMode.Create);
                        viewModel.Archivo.CopyTo(fs);
                        fs.Close();
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("!", "La planta que intentó editar no está disponible.");
                    viewModel.User = userRepos.GetById(viewModel.Plant.IdUser);
                    viewModel.Users = userRepos.GetAll();
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                viewModel.User = userRepos.GetById(viewModel.Plant.IdUser);
                viewModel.Users = userRepos.GetAll();
                return View(viewModel);
            }
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult Borrar(Plantdata plant)
        {
            PlantRepository repository = new PlantRepository(context);
            var plantdata = repository.GetById(plant.Id);
            if (plantdata != null)
            {
                repository.Delete(plantdata);
            }
            else
            {
                ModelState.AddModelError("!", "La planta que intentó eliminar no está disponible.");
                return View(plant);
            }
            return RedirectToAction("Catalogo", new { id = plantdata.IdUser });
        }
    }
}
