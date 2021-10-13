using Final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Final.Data;
using EasyEncryption;
using Microsoft.AspNetCore.Authorization;

namespace Final.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("Home")]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name != null)
            {
                var usuario = await _context.Usuario.FindAsync(int.Parse(User.Identity.Name));
                if (usuario != null)
                {
                    @ViewData["currentUser"] = usuario.DNI;
                    return View();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
