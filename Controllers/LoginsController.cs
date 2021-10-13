using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.Data;
using EasyEncryption;
using Final.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Diagnostics;

namespace Final.Controllers
{
    public class LoginsController : Controller
    {

        private readonly MyContext _context;
        public DbSet<Usuario> usuarios;
        private const int cantMaxIntentos = 3;
        public static IDictionary<int, int> loginHistory = new Dictionary<int, int>();

        public LoginsController(MyContext context)
        {
            _context = context;
            _context.Usuario.Load();
            usuarios = _context.Usuario;

        }
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet("Logins/Login")]
        public IActionResult Login()
        {
            if (User.Identity.Name != null)
            {
                return Redirect("/Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost("Logins/Login")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(int DNI, String password)
        {
            if (User.Identity.Name == null)
            {

                string passEncriptada = SHA.ComputeSHA256Hash(password);
                bool isOK = false;
                bool bloqueado = false;
                bool existeUsuario = false;

                using (var context = new MyContext())
                {
                    var query = (from u in context.Usuario
                                 where u.DNI == DNI && u.password == passEncriptada
                                 select u).Count();

                    var query2 = (from u in context.Usuario
                                  where u.DNI == DNI && u.password == passEncriptada && u.bloqueado
                                  select u).Count();

                    var query3 = (from u in context.Usuario
                                  where u.DNI == DNI
                                  select u).Count();

                    if (query > 0)
                    {
                        isOK = true;
                    }

                    if (query2 > 0)
                    {
                        bloqueado = true;
                    }

                    if (query3 > 0)
                    {
                        existeUsuario = true;
                    }
                }

                if (existeUsuario)
                {
                    var usuario = _context.Usuario.Where(user => user.DNI == DNI).First();
                    if (isOK)
                    {
                        if (!bloqueado)
                        {
                            loginHistory[DNI] = 0;
                            var claims = new List<Claim> {
                          new Claim(ClaimTypes.Name, usuario.id.ToString()),
                          new Claim(ClaimTypes.Role, usuario.esADMIN ? "Admin" : "User"),
                          new Claim("Usuario", usuario.nombre),
                        };

                            var claimsIdentity = new ClaimsIdentity(claims, "Login");
                            var authProperties = new AuthenticationProperties { ExpiresUtc = DateTimeOffset.Now.AddMinutes(5) };

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            if (usuario.esADMIN)
                            {
                                return Redirect("/Home");
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            TempData["status"] = "Su usuario esta bloqueado. Contactese con un administrador.";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        if (loginHistory.TryGetValue(DNI, out int value))
                        {
                            loginHistory[DNI] = loginHistory[DNI] + 1;
                            TempData["status"] = "Datos incorrectos, intento " + loginHistory[DNI] + "/" + cantMaxIntentos;
                            ViewData["cantIntentos"] = loginHistory[DNI];
                            if (loginHistory[DNI] >= cantMaxIntentos)
                            {
                                bloquearDesbloquearUsuario(DNI, true);
                                TempData["status"] = "Intento 3/" + cantMaxIntentos + ". Usuario bloqueado";
                                ViewData["cantIntentos"] = loginHistory[DNI];
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            TempData["status"] = "Datos incorrectos, intento 1/" + cantMaxIntentos;
                            loginHistory.Add(DNI, 1);
                            ViewData["cantIntentos"] = loginHistory[DNI];
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    TempData["status"] = "Usuario inexistente.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return Redirect("/Home");
            }
        }

        [HttpGet("Logins/Logout")]
        public async Task<ActionResult> Logout()
        {
            if (User.Identity.Name != null)
            {
                if (HttpContext.Request.Cookies.Count > 0)
                {
                    var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication"));
                    foreach (var cookie in siteCookies)
                    {
                        Response.Cookies.Delete(cookie.Key);
                    }
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public bool bloquearDesbloquearUsuario(int dni, bool bloqueado)
        {
            bool todoOk = false;
            foreach (Usuario u in usuarios)
            {
                if (u.DNI == dni)
                {
                    u.bloqueado = bloqueado;
                    usuarios.Update(u);
                    todoOk = true;
                }
            }

            if (todoOk)
            {
                _context.SaveChanges();
            }
            return todoOk;
        }

        public IActionResult Register()
        {
            if (User.Identity.Name == null)
            {
                return View();
            }
            else
            {
                return Redirect("/Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int DNI, String password, String confirmPassword, [Bind("id,DNI,nombre,mail,password,esADMIN,bloqueado")] Usuario usuario)
        {
            if (User.Identity.Name == null)
            {
                if (ModelState.IsValid)
                {
                    if (!existeUsuario(DNI))
                    {
                        if (password != confirmPassword)
                        {
                            ViewBag.Message = string.Format("Las claves no coinciden. Por favor, intentarlo nuevamente.");
                            return View();
                        }
                        string passwordHash = SHA.ComputeSHA256Hash(password);
                        usuario.password = passwordHash;
                        _context.Add(usuario);
                        await _context.SaveChangesAsync();
                        TempData["status"] = "Usuario creado con exito";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = string.Format("Ya existe un usuario con ese DNI");
                        return View();
                    }
                }
                return View();
            }
            else
            {
                return Redirect("/Home");
            }
        }

        public bool existeUsuario(int DNI)
        {
            bool count = false;

            using (var context = new MyContext())
            {
                var query = (from u in context.Usuario
                             where u.DNI == DNI
                             select u).Count();

                if (query > 0)
                {
                    count = true;
                }
            }

            return count;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
