using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.Data;
using EasyEncryption;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace Final.Models
{
    public class UsuariosController : Controller
    {
        private const int cantMaxIntentos = 3;
        public bool loginOk;
        public IDictionary<int, int> loginHistory = new Dictionary<int, int>();
        private readonly MyContext _context;

        public UsuariosController(MyContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        [HttpGet("Usuarios")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index(String personal)
        {
            if(User.IsInRole("User") || personal == "personal")
            {
                return RedirectToAction("Personal");
            }

            ViewData["personal"] = personal;
            return View(await _context.Usuario.ToListAsync());
        }

        // GET: Usuarios
        [HttpGet("Usuarios/Personal")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Personal()
        {
            ViewData["personal"] = "";
            if (User.Identity.Name != null)
            {
                var usuario = await _context.Usuario.FirstOrDefaultAsync(m => m.id == int.Parse(User.Identity.Name));
                if (usuario != null)
                {
                    ViewData["currentUser"] = usuario.DNI;
                    if (User.IsInRole("Admin"))
                    {
                        ViewData["personal"] = "personal";
                    }
                    return View(usuario);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return Redirect("/Logins/Login");
            }
        }

        // GET: Usuarios/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int DNI, String password, [Bind("id,DNI,nombre,mail,password,esADMIN,bloqueado")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (!existeUsuario(DNI)) {
                    usuario.bloqueado = false;
                    string passwordHash = SHA.ComputeSHA256Hash(password);
                    usuario.password = passwordHash;
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                    TempData["status"] = "Accion realizada con exito.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = string.Format("Ya existe un usuario con ese DNI");
                    return View(usuario);
                }

            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [HttpGet("Usuarios/Edit")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id, String personal)
        {            
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["personal"] = personal;
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,DNI,nombre,mail,password,esADMIN,bloqueado")] Usuario usuario)
        {
            if (id != usuario.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    TempData["status"] = "Accion realizada con exito.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (User.IsInRole("User") || (User.IsInRole("Admin") && id == Int32.Parse(User.Identity.Name)))
                {
                    return RedirectToAction("Personal");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        [HttpGet("Usuarios/Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id, String personal)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewData["personal"] = personal;
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            TempData["status"] = "Accion realizada con exito.";
            return RedirectToAction("Index");
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.id == id);
        }

        // GET: Usuarios/ChangePassword/5
        [HttpGet("Usuarios/ChangePassword")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ChangePassword(int? id, String personal)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewData["personal"] = personal;
            return View(usuario);
        }

        // POST: Usuarios/ChangePassword/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Usuarios/ChangePassword")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ChangePassword(int id, String oldPassword, String newPassword, String confirmPassword, int DNI, String personal, [Bind("id,DNI,nombre,mail,password,esADMIN,bloqueado")] Usuario usuario)
        {
            if (id != usuario.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                String error;
                String oldPassEncriptada ="";
                bool authOK = false;
                bool passOK = false;
                ViewData["personal"] = personal;

                if(id == Int32.Parse(User.Identity.Name))
                {
                    oldPassEncriptada = SHA.ComputeSHA256Hash(oldPassword);
                    if(usuario.password == oldPassEncriptada)
                    {
                        passOK = true;
                    }
                    
                }else if(personal == "personal" && id != Int32.Parse(User.Identity.Name))
                {
                    passOK = true;
                }

                if (usuario.DNI == DNI && (usuario.password == oldPassEncriptada || passOK)) { authOK = true; }
                    
                if (authOK)
                {
                    if (oldPassword == newPassword)
                    {
                        error = "La nueva clave no puede ser igual a la anterior.";
                        ViewBag.Message = string.Format(error);
                        return View(usuario);
                    }
                    else if (newPassword == "")
                    {
                        error = "La nueva clave no puede estar vacía.";
                        ViewBag.Message = string.Format(error);
                        return View(usuario);
                    }
                    else if (newPassword != confirmPassword)
                    {
                        error = "La nueva clave no coincide en el segundo campo.";
                        ViewBag.Message = string.Format(error);
                        return View(usuario);
                    }else if (newPassword.Length < 8 || confirmPassword.Length < 8 )
                    {
                        error = "La clave debe tener 8 caracteres como minimo. Por favor, reintentar.";
                        ViewBag.Message = string.Format(error);
                        return View(usuario);
                    }
                    else
                    {
                        try
                        {
                            usuario.password = SHA.ComputeSHA256Hash(newPassword);
                            _context.Update(usuario);
                            await _context.SaveChangesAsync();
                            TempData["status"] = "Accion realizada con exito.";
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!UsuarioExists(usuario.id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        if (User.IsInRole("User") || (User.IsInRole("Admin") && id == Int32.Parse(User.Identity.Name)))
                        {
                            return RedirectToAction("Personal");
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    error = "El usuario y la clave no coinciden.";
                    ViewBag.Message = string.Format(error);
                    return View(usuario);
                }                
            }
            
            return View(usuario);
        }
        
        public bool existeUsuario(int dni)
        {
            foreach (Usuario usuario in _context.Usuario)
            {
                if (usuario.DNI == dni)
                {
                    return true;
                }
            }
            return false;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
