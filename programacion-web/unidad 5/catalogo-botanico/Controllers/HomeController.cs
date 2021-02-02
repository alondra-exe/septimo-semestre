using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CatalogoBotanico.Models;
using CatalogoBotanico.Repositories;
using Microsoft.AspNetCore.Hosting;
using CatalogoBotanico.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net.Mail;
using System.Net;

namespace CatalogoBotanico.Controllers
{
    public class HomeController : Controller
    {
        sistem14_botanicaContext context = new sistem14_botanicaContext();
        public IWebHostEnvironment Enviroment { get; set; }
        public HomeController(IWebHostEnvironment environment)
        {
            Enviroment = environment;
        }

        [Authorize(Roles = "User, Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Iniciar()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Iniciar(Userdata usuario, bool recordar)
        {
            UserRepository repository = new UserRepository(context);
            var userdata = repository.GetUserByEmail(usuario.Email);
            try
            {
                if (userdata != null && HashHelper.GetHash(usuario.Password) == userdata.Password)
                {
                    if (userdata.Active == 1)
                    {
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, userdata.Name));
                        claims.Add(new Claim(ClaimTypes.Role, "User"));
                        claims.Add(new Claim("Id", userdata.Id.ToString()));
                        claims.Add(new Claim("Name", userdata.Name));
                        claims.Add(new Claim("Email", userdata.Email));
                        claims.Add(new Claim("Alias", userdata.Alias));
                        claims.Add(new Claim("Bio", userdata.Bio));

                        var claimsidentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsprincipal = new ClaimsPrincipal(claimsidentity);

                        if (recordar == true)
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                                new AuthenticationProperties { IsPersistent = true });
                        else
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                                new AuthenticationProperties { IsPersistent = false });

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("*", "Necesita activar su cuenta para ingresar.");
                        return View(usuario);
                    }
                }
                else
                {
                    ModelState.AddModelError("*", "El correo electrónico o la contraseña es incorrecta.");
                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(usuario);
            }
        }

        [AllowAnonymous]
        public IActionResult IniciarAdmin()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> IniciarAdmin(Admindata admin)
        {
            Repository<Admindata> repository = new Repository<Admindata>(context);
            var admindata = context.Admindata.FirstOrDefault(x => x.Name == admin.Name);
            try
            {
                if (admindata != null && admindata.Password == HashHelper.GetHash(admin.Password))
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, admindata.Name));
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    claims.Add(new Claim("Id", admin.Id.ToString()));
                    claims.Add(new Claim("Name", admin.Name));
                    var claimsidentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsprincipal = new ClaimsPrincipal(claimsidentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                        new AuthenticationProperties { IsPersistent = true });
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("!", "Los datos de ingreso son incorrectos.");
                    return View(admin);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(admin);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Cerrar()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Registrar()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Registrar(Userdata usuario, string pw1, string pw2)
        {
            UserRepository repository = new UserRepository(context);
            try
            {
                if (context.Userdata.Any(x => x.Email == usuario.Email))
                {
                    ModelState.AddModelError("!", $"{usuario.Email} ya está en uso.");
                    return View(usuario);
                }
                else
                {
                    if (pw1 == pw2)
                    {
                        usuario.Password = HashHelper.GetHash(pw1);
                        usuario.Code = CodeHelper.GetCode();
                        usuario.Active = 0;
                        repository.Insert(usuario);

                        MailMessage message = new MailMessage();
                        message.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Plantapedia");
                        message.To.Add(usuario.Email);
                        message.Subject = "Activa tu cuenta en Plantapedia.";
                        string text = System.IO.File.ReadAllText(Enviroment.WebRootPath + "/ActivarCuenta.html");
                        message.Body = text.Replace("{##codigo##}", usuario.Code.ToString());
                        message.IsBodyHtml = true;
                        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                        client.Send(message);
                        return RedirectToAction("Activar");
                    }
                    else
                    {
                        ModelState.AddModelError("!", "Las contraseñas no coinciden.");
                        return View(usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(usuario);
            }
        }

        [AllowAnonymous]
        public IActionResult Activar()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Activar(int code)
        {
            UserRepository repository = new UserRepository(context);
            var userdata = context.Userdata.FirstOrDefault(x => x.Code == code);
            try
            {
                if (userdata != null && userdata.Active == 0)
                {
                    var usercode = userdata.Code;
                    if (code == usercode)
                    {
                        userdata.Active = 1;
                        repository.Update(userdata);
                        return RedirectToAction("Iniciar");
                    }
                    else
                    {
                        ModelState.AddModelError("!", "El código ingresado es incorrecto.");
                        return View(code);
                    }
                }
                else
                {
                    ModelState.AddModelError("!", "La cuenta a la intenta acceder no existe.");
                    return View(code);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(code);
            }
        }

        [AllowAnonymous]
        public IActionResult Recuperar()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Recuperar(string email)
        {
            UserRepository repository = new UserRepository(context);
            var userdata = repository.GetUserByEmail(email);
            try
            {
                if (userdata != null)
                {
                    var temporal = CodeHelper.GetCode();
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Plantapedia");
                    message.To.Add(email);
                    message.Subject = "Recupera tu contraseña tu contraseña en Plantapedia";
                    string text = System.IO.File.ReadAllText(Enviroment.WebRootPath + "/RecuperarContra.html");
                    message.Body = text.Replace("{##codigo##}", temporal.ToString());
                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                    client.Send(message);
                    userdata.Password = HashHelper.GetHash(temporal.ToString());
                    repository.Update(userdata);
                    return RedirectToAction("Iniciar");
                }
                else
                {
                    ModelState.AddModelError("!", $"{userdata.Email} no se encuentra en uso.");
                    return View((object)email);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View((object)email);
            }
        }

        public IActionResult Denegado()
        {
            return View();
        }
    }
}