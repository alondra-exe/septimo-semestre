using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Actividad1.Models;
using Actividad1.Repositories;
using Actividad1.Helpers;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace Actividad1.Controllers
{
    public class HomeController : Controller
    {
        public IWebHostEnvironment Environment { get; set; }
        public HomeController(IWebHostEnvironment env)
        {
            Environment = env;
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IniciarSesion(Usuario usuario, bool persistente)
        {
            usuariosContext context = new usuariosContext();
            UsuarioRepository<Usuario> repository = new UsuarioRepository<Usuario>(context);
            var datos = repository.ObtenerUsuarioPorCorreo(usuario.CorreoElectronico);
            if (datos != null && HashHelper.GetHash(usuario.Contrasena) == datos.Contrasena)
            {
                if (datos.Activo == 1)
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, "Usuario" + datos.NombreUsuario));
                    info.Add(new Claim(ClaimTypes.Role, "Cliente"));
                    info.Add(new Claim("Correo", datos.CorreoElectronico));
                    info.Add(new Claim("Nombre", datos.NombreUsuario));

                    var claimsidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsprincipal = new ClaimsPrincipal(claimsidentity);

                    if (persistente == true)
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                            new AuthenticationProperties { IsPersistent = true });
                    }
                    else
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                            new AuthenticationProperties { IsPersistent = false });
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Esta cuenta aún no ha sido activada. ¡Necesita ir a su correo y activarla para continuar!");
                    return View(usuario);
                }
            }
            else
            {
                ModelState.AddModelError("", "El correo electrónico o la contraseña son incorrectas.");
                return View(usuario);
            }
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("IniciarSesion");
        }

        [AllowAnonymous]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Registrar(Usuario usuario, string contra1, string contra2)
        {
            usuariosContext context = new usuariosContext();
            UsuarioRepository<Usuario> repository = new UsuarioRepository<Usuario>(context);
            try
            {
                if (context.Usuario.Any(x => x.CorreoElectronico == usuario.CorreoElectronico))
                {
                    ModelState.AddModelError("", "Este correo electrónico ya se encuentra registrado.");
                    return View(usuario);
                }
                else
                {
                    if (contra1 == contra2)
                    {
                        usuario.Contrasena = HashHelper.GetHash(contra1);
                        usuario.Codigo = CodeHelper.GetCode();
                        usuario.Activo = 0;
                        repository.Insertar(usuario);

                        MailMessage mensaje = new MailMessage();
                        mensaje.From = new MailAddress("sistemascomputacionales7g@gmail.com", "OursCafé");
                        mensaje.To.Add(usuario.CorreoElectronico);
                        mensaje.Subject = "Confirma tu correo electrónico en OursCafé";
                        string text = System.IO.File.ReadAllText(Environment.WebRootPath + "/ConfirmarCorreo.html");
                        mensaje.Body = text.Replace("{##codigo##}", usuario.Codigo.ToString());
                        mensaje.IsBodyHtml = true;

                        SmtpClient cliente = new SmtpClient("smtp.gmail.com", 587);
                        cliente.EnableSsl = true;
                        cliente.UseDefaultCredentials = false;
                        cliente.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                        cliente.Send(mensaje);
                        return RedirectToAction("Activar");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Las contraseñas no son iguales.");
                        return View(usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(usuario);
            }
        }

        [AllowAnonymous]
        public IActionResult Activar()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Activar(int codigo)
        {
            usuariosContext context = new usuariosContext();
            UsuarioRepository<Usuario> repository = new UsuarioRepository<Usuario>(context);
            var usuario = context.Usuario.FirstOrDefault(x => x.Codigo == codigo);

            if (usuario != null && usuario.Activo == 0)
            {
                var code = usuario.Codigo;
                if (codigo == code)
                {
                    usuario.Activo = 1;
                    repository.Editar(usuario);
                    return RedirectToAction("IniciarSesion");
                }
                else
                {
                    ModelState.AddModelError("", "El código ingresado no coincide.");
                    return View((object)codigo);
                }
            }
            else
            {
                ModelState.AddModelError("", "El usuario no existe.");
                return View((object)codigo);
            }
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult CambiarContra()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public IActionResult CambiarContra(string correo, string contra, string nuevaContra, string nuevaContraConfirm)
        {
            usuariosContext context = new usuariosContext();
            UsuarioRepository<Usuario> repository = new UsuarioRepository<Usuario>(context);
            try
            {
                var usuario = repository.ObtenerUsuarioPorCorreo(correo);

                if (usuario.Contrasena != HashHelper.GetHash(contra))
                {
                    ModelState.AddModelError("", "La contraseña que ingresó es incorrecta.");
                    return View();
                }
                else
                {
                    if (nuevaContra != nuevaContraConfirm)
                    {
                        ModelState.AddModelError("", "Las nuevas contraseñas no son iguales.");
                        return View();
                    }
                    else if (usuario.Contrasena == HashHelper.GetHash(nuevaContra))
                    {
                        ModelState.AddModelError("", "La nueva contraseña no puede ser igual a la actual.");
                        return View();
                    }
                    else
                    {
                        usuario.Contrasena = HashHelper.GetHash(nuevaContra);
                        repository.Editar(usuario);
                        return RedirectToAction("IniciarSesion");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult RecuperarContra()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult RecuperarContra(string correo)
        {
            try
            {
                usuariosContext context = new usuariosContext();
                UsuarioRepository<Usuario> repository = new UsuarioRepository<Usuario>(context);
                var usuario = repository.ObtenerUsuarioPorCorreo(correo);

                if (usuario != null)
                {
                    var contraTemp = CodeHelper.GetCode();
                    MailMessage mensaje = new MailMessage();
                    mensaje.From = new MailAddress("sistemascomputacionales7g@gmail.com", "OursCafé");
                    mensaje.To.Add(correo);
                    mensaje.Subject = "Recupera tu contraseña en OursCafé";
                    string text = System.IO.File.ReadAllText(Environment.WebRootPath + "/RecuperarContra.html");
                    mensaje.Body = text.Replace("{##contraTemp##}", contraTemp.ToString());
                    mensaje.IsBodyHtml = true;

                    SmtpClient cliente = new SmtpClient("smtp.gmail.com", 587);
                    cliente.EnableSsl = true;
                    cliente.UseDefaultCredentials = false;
                    cliente.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                    cliente.Send(mensaje);
                    usuario.Contrasena = HashHelper.GetHash(contraTemp.ToString());
                    repository.Editar(usuario);
                    return RedirectToAction("IniciarSesion");
                }
                else
                {
                    ModelState.AddModelError("", "El correo electrónico que ingresó no se encuentra registrado :(");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View((object)correo);
            }
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult EliminarCuenta()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public IActionResult EliminarCuenta(string correo, string contra)
        {
            try
            {
                usuariosContext context = new usuariosContext();
                UsuarioRepository<Usuario> repository = new UsuarioRepository<Usuario>(context);
                var usuario = repository.ObtenerUsuarioPorCorreo(correo);
                if (usuario != null)
                {
                    if (HashHelper.GetHash(contra) == usuario.Contrasena)
                    {
                        repository.Eliminar(usuario);
                    }
                    else
                    {
                        ModelState.AddModelError("", "La contraseña está incorrecta.");
                        return View();
                    }
                }
                return RedirectToAction("IniciarSesion");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ocurrió un error. Inténtelo en unos minutos.");
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }
    }
}