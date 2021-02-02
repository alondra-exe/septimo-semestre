using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Actividad2.Models;
using Actividad2.Repositories;
using Actividad2.Helpers;
using Actividad2.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Actividad2.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "Maestro, Director")]
        public IActionResult Index(int nocontrol)
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult MaestroIS()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> MaestroIS(Maestro m)
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);
            var maestro = repository.ObtenerMaestroPorNoControl(m.NoControl);
            try
            {
                if (maestro != null && maestro.Contrasena == HashHelper.GetHash(m.Contrasena))
                {
                    if (maestro.Activo == 1)
                    {
                        List<Claim> info = new List<Claim>();
                        info.Add(new Claim(ClaimTypes.Name, "Usuario" + maestro.Nombre));
                        info.Add(new Claim(ClaimTypes.Role, "Maestro"));
                        info.Add(new Claim("NoControl", maestro.NoControl.ToString()));
                        info.Add(new Claim("Nombre", maestro.Nombre));
                        info.Add(new Claim("Id", maestro.Id.ToString()));

                        var claimsidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsprincipal = new ClaimsPrincipal(claimsidentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                            new AuthenticationProperties { IsPersistent = true });
                        return RedirectToAction("Index", maestro.NoControl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Su cuenta se encuentra inactiva.");
                        return View(m);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El número de control o la contraseña del maestro son incorrectas.");
                    return View(m);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(m);
            }
        }

        [AllowAnonymous]
        public IActionResult DirectorIS()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> DirectorIS(Director d)
        {
            roleContext context = new roleContext();
            RolesRepository<Director> repository = new RolesRepository<Director>(context);
            var director = context.Director.FirstOrDefault(x => x.NoControl == d.NoControl);
            try
            {
                if (director != null && director.Contrasena == HashHelper.GetHash(d.Contrasena))
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, "Usuario" + director.Nombre));
                    info.Add(new Claim(ClaimTypes.Role, "Director"));
                    info.Add(new Claim("NoControl", director.Nombre.ToString()));
                    info.Add(new Claim("Nombre", director.Nombre));

                    var claimsidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsprincipal = new ClaimsPrincipal(claimsidentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                        new AuthenticationProperties { IsPersistent = true });
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "El número de control o la contraseña del director son incorrectas.");
                    return View(d);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(d);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Director")]
        public IActionResult ListaMaestros()
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);
            var maestros = repository.ObtenerTodo();
            return View(maestros);
        }

        [Authorize(Roles = "Director")]
        public IActionResult DarAltaMaestros()
        {
            return View();
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult DarAltaMaestros(Maestro m)
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);

            try
            {
                var maestro = repository.ObtenerMaestroPorNoControl(m.NoControl);
                if (maestro == null)
                {
                    m.Activo = 1;
                    m.Contrasena = HashHelper.GetHash(m.Contrasena);
                    repository.Insertar(m);
                    return RedirectToAction("ListaMaestros");
                }
                else
                {
                    ModelState.AddModelError("", "El número de control que ingresó no está disponible.");
                    return View(m);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(m);
            }
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult EstadoMaestro(Maestro m)
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);
            var maestro = repository.ObtenerPorId(m.Id);
            if (maestro != null && maestro.Activo == 0)
            {
                maestro.Activo = 1;
                repository.Editar(maestro);
            }
            else
            {
                maestro.Activo = 0;
                repository.Editar(maestro);
            }
            return RedirectToAction("ListaMaestros");
        }

        [Authorize(Roles = "Director")]
        public IActionResult ModificarInfoMaestros(int id)
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);
            var maestro = repository.ObtenerPorId(id);
            if (maestro != null)
            {
                return View(maestro);
            }
            return RedirectToAction("ListaMaestros");
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult ModificarInfoMaestros(Maestro m)
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);
            var maestro = repository.ObtenerPorId(m.Id);
            try
            {
                if (maestro != null)
                {
                    maestro.Nombre = m.Nombre;
                    repository.Editar(maestro);
                }
                return RedirectToAction("ListaMaestros");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(maestro);
            }
        }

        [Authorize(Roles = "Director")]
        public IActionResult CambiarContraMaestro(int id)
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);
            var maestro = repository.ObtenerPorId(id);
            if (maestro == null)
            {
                return RedirectToAction("ListaMaestros");
            }
            return View(maestro);
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult CambiarContraMaestro(Maestro m, string nuevaContra, string nuevaContraConfirm)
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);
            var maestro = repository.ObtenerPorId(m.Id);
            try
            {

                if (maestro != null)
                {
                    if (nuevaContra != nuevaContraConfirm)
                    {
                        ModelState.AddModelError("", "Las nuevas contraseñas no son iguales.");
                        return View(maestro);
                    }
                    else if (maestro.Contrasena == HashHelper.GetHash(nuevaContra))
                    {
                        ModelState.AddModelError("", "La nueva contraseña no puede ser igual a la actual.");
                        return View(maestro);
                    }
                    else
                    {
                        maestro.Contrasena = HashHelper.GetHash(nuevaContra);
                        repository.Editar(maestro);
                        return RedirectToAction("ListaMaestros");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El maestro al que intentó modificar no existe.");
                    return View(maestro);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(maestro);
            }
        }

        [Authorize(Roles = "Maestro, Director")]
        public IActionResult ListaAlumnos(int id)
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);
            var maestro = repository.ObtenerAlumnosPorMaestro(id);
            if (maestro != null)
            {
                if (User.IsInRole("Maestro"))
                {
                    if (User.Claims.FirstOrDefault(x => x.Type == "Id").Value == maestro.Id.ToString())
                    {
                        return View(maestro);
                    }
                    else
                    {
                        return RedirectToAction("Denegado");
                    }
                }
                else
                {
                    return View(maestro);
                }
            }
            else
            {
                return RedirectToAction("ListaAlumnos");
            }
        }

        [Authorize(Roles = "Maestro, Director")]
        public IActionResult AlumnoAgregar(int id)
        {
            roleContext context = new roleContext();
            MaestroRepository repository = new MaestroRepository(context);
            AlumnoViewModel viewModel = new AlumnoViewModel();
            viewModel.Maestro = repository.ObtenerPorId(id);
            if (viewModel.Maestro != null)
            {
                if (User.IsInRole("Maestro"))
                {
                    if (User.Claims.FirstOrDefault(x => x.Type == "Id").Value == viewModel.Maestro.Id.ToString())
                    {
                        return View(viewModel);
                    }
                    else
                    {
                        return RedirectToAction("Denegado");
                    }
                }
                else
                {
                    return View(viewModel);
                }
            }
            return View(viewModel);
        }

        [Authorize(Roles = "Maestro, Director")]
        [HttpPost]
        public IActionResult AlumnoAgregar(AlumnoViewModel viewModel)
        {
            roleContext context = new roleContext();
            MaestroRepository maestroRepository = new MaestroRepository(context);
            AlumnosRepository alumnosRepository = new AlumnosRepository(context);
            try
            {
                if (context.Alumno.Any(x => x.NoControl == viewModel.Alumno.NoControl))
                {
                    ModelState.AddModelError("", "Este número de control ya se encuentra registrado.");
                    return View(viewModel);
                }
                else
                {
                    var maestro = maestroRepository.ObtenerMaestroPorNoControl(viewModel.Maestro.NoControl).Id;
                    viewModel.Alumno.IdMaestro = maestro;
                    alumnosRepository.Insertar(viewModel.Alumno);
                    return RedirectToAction("ListaAlumnos", new { id = maestro });
                }

            }
            catch (Exception ex)
            {
                viewModel.Maestro = maestroRepository.ObtenerPorId(viewModel.Maestro.Id);
                viewModel.Maestros = maestroRepository.ObtenerTodo();
                ModelState.AddModelError("", ex.Message);
                return View(viewModel);
            }
        }

        [Authorize(Roles = "Maestro, Director")]
        public IActionResult AlumnoEditar(int id)
        {
            roleContext context = new roleContext();
            MaestroRepository maestroRepository = new MaestroRepository(context);
            AlumnosRepository alumnosRepository = new AlumnosRepository(context);
            AlumnoViewModel viewModel = new AlumnoViewModel();
            viewModel.Alumno = alumnosRepository.ObtenerPorId(id);
            viewModel.Maestros = maestroRepository.ObtenerTodo();
            if (viewModel.Alumno != null)
            {
                viewModel.Maestro = maestroRepository.ObtenerPorId(viewModel.Alumno.IdMaestro);
                if (User.IsInRole("Maestro"))
                {
                    viewModel.Maestro = maestroRepository.ObtenerPorId(viewModel.Alumno.IdMaestro);
                    if (User.Claims.FirstOrDefault(x => x.Type == "NoControl").Value == viewModel.Maestro.NoControl.ToString())
                    {

                        return View(viewModel);
                    }
                }
                return View(viewModel);

            }
            else return RedirectToAction("Index");
        }

        [Authorize(Roles = "Maestro, Director")]
        [HttpPost]
        public IActionResult AlumnoEditar(AlumnoViewModel viewModel)
        {
            roleContext context = new roleContext();
            MaestroRepository maestroRepository = new MaestroRepository(context);
            AlumnosRepository alumnosRepository = new AlumnosRepository(context);
            try
            {
                var alumno = alumnosRepository.ObtenerPorId(viewModel.Alumno.Id);
                if (alumno != null)
                {
                    alumno.Nombre = viewModel.Alumno.Nombre;
                    alumnosRepository.Editar(alumno);
                    return RedirectToAction("ListaAlumnos", new { id = alumno.IdMaestro });
                }
                else
                {
                    ModelState.AddModelError("", "El alumno que intentó editar no existe.");
                    viewModel.Maestro = maestroRepository.ObtenerPorId(viewModel.Alumno.IdMaestro);
                    viewModel.Maestros = maestroRepository.ObtenerTodo();
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                viewModel.Maestro = maestroRepository.ObtenerPorId(viewModel.Alumno.IdMaestro);
                viewModel.Maestros = maestroRepository.ObtenerTodo();
                return View(viewModel);
            }
        }

        [Authorize(Roles = "Maestro, Director")]
        [HttpPost]
        public IActionResult AlumnoEliminar(Alumno a)
        {
            roleContext context = new roleContext();
            AlumnosRepository repository = new AlumnosRepository(context);
            var alumno = repository.ObtenerPorId(a.Id);
            if (alumno != null)
            {
                repository.Eliminar(alumno);
            }
            else
            {
                ModelState.AddModelError("", "El alumnó que intentó eliminar no existe.");
            }
            return RedirectToAction("ListaAlumnos", new { id = alumno.IdMaestro });
        }

        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }
    }
}