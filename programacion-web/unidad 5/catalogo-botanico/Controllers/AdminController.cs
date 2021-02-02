using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CatalogoBotanico.Models;
using CatalogoBotanico.Repositories;
using Microsoft.AspNetCore.Hosting;
using CatalogoBotanico.Models.ViewModels;
using CatalogoBotanico.Helpers;

namespace CatalogoBotanico.Controllers
{
    public class AdminController : Controller
    {
        sistem14_botanicaContext context = new sistem14_botanicaContext();
        public IWebHostEnvironment Enviroment { get; set; }
        public AdminController(IWebHostEnvironment environment)
        {
            Enviroment = environment;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Usuarios()
        {
            UserRepository repository = new UserRepository(context);
            var users = repository.GetAll();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult ActivarUsuario(Userdata user)
        {
            UserRepository repository = new UserRepository(context);
            var userdata = repository.GetById(user.Id);
            if (userdata != null && userdata.Active == 0)
            {
                userdata.Active = 1;
                repository.Update(userdata);
            }
            else
            {
                userdata.Active = 0;
                repository.Update(userdata);
            }
            return RedirectToAction("Usuarios");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CatalogoUsuario(int id)
        {
            UserRepository repository = new UserRepository(context);
            var catalogo = repository.GetPlantByUser(id);
            if (catalogo != null)
            {
                return View(catalogo);
            }
            else
            {
                return RedirectToAction("Usuarios");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CatalogoUsuario(PlantViewModel viewModel)
        {
            UserRepository userRepos = new UserRepository(context);
            PlantRepository plantRepos = new PlantRepository(context);
            try
            {
                if (context.Plantdata.Any(x => x.CommonName == viewModel.Plant.CommonName))
                {
                    ModelState.AddModelError("!", "Esta planta ya se encuentra registrada en el catálogo.");
                    return View(viewModel);
                }
                else
                {
                    var user = userRepos.GetUserById(viewModel.User.Id).Id;
                    viewModel.Plant.IdUser = user;
                    plantRepos.Insert(viewModel.Plant);
                    return RedirectToAction("Usuario/Catalogo", new { id = user });
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
        public IActionResult EditarUsuario(int id)
        {
            UserRepository repository = new UserRepository(context);
            var userdata = repository.GetById(id);
            try
            {
                if (userdata != null)
                {
                    return View(userdata);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(userdata);
            }
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult EditarUsuario(Userdata user)
        {
            UserRepository repository = new UserRepository(context);
            var userdata = repository.GetById(user.Id);
            try
            {
                if (userdata != null)
                {
                    if (User.IsInRole("User"))
                    {
                        userdata.Alias = user.Alias;
                        userdata.Bio = user.Bio;
                        repository.Update(userdata);
                    }
                    else if (User.IsInRole("Admin"))
                    {
                        userdata.Name = user.Name;
                        userdata.Email = user.Email;
                        userdata.Alias = user.Alias;
                        userdata.Bio = user.Bio;
                        repository.Update(userdata);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(userdata);
            }
        }

        [Authorize(Roles = "User, Admin")]
        public IActionResult CambiarClave(int id)
        {
            UserRepository repository = new UserRepository(context);
            var user = repository.GetUserById(id);
            try
            {
                if (user == null)
                {
                    return RedirectToAction("Usuarios");
                }
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(user);
            }
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult CambiarClave(Userdata user, string pw1, string pw2)
        {
            UserRepository repository = new UserRepository(context);
            var userdata = repository.GetUserById(user.Id);
            try
            {
                if (userdata != null)
                {
                    if (pw1 != pw2)
                    {
                        ModelState.AddModelError("!", "Las contraseñas no coinciden.");
                        return View(userdata);
                    }
                    else if (userdata.Password == HashHelper.GetHash(pw1))
                    {
                        ModelState.AddModelError("!", "La nueva contraseña debe ser diferente a la actual.");
                        return View(userdata);
                    }
                    else
                    {
                        userdata.Password = HashHelper.GetHash(pw1);
                        repository.Update(userdata);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("!", "Esta cuenta no existe.");
                    return View(userdata);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(userdata);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult BorrarPlanta(int id)
        {
            PlantRepository repository = new PlantRepository(context);
            var plantdata = repository.GetById(id);
            try
            {
                if (plantdata != null)
                {
                    return View(plantdata);
                }
                return RedirectToAction("BorrarPlanta");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(plantdata);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult BorrarPlanta(Plantdata plant)
        {
            PlantRepository repository = new PlantRepository(context);
            var plantdata = repository.GetById(plant.Id);
            if (plantdata != null)
            {
                repository.Delete(plantdata);
                return RedirectToAction("CatalogoUsuario", new { id = plantdata.IdUser });
            }
            else
            {
                ModelState.AddModelError("!", "La planta que intentó eliminar no está disponible.");
                return View(plantdata);
            }
        }
    }
}