using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.Data;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace Final.Models
{
    public class ReservasController : Controller
    {
        private readonly MyContext _context;

        public ReservasController(MyContext context)
        {
            _context = context;
        }

        // GET: Reservas
        [HttpGet("Reservas")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("User"))
            {
                var myContext = _context.Reserva.Include(r => r.persona).Include(r => r.propiedad).Where(r => r.personaId == int.Parse(User.Identity.Name));
                return View(await myContext.ToListAsync());
            }
            else
            {
                var myContext = _context.Reserva.Include(r => r.persona).Include(r => r.propiedad);
                return View(await myContext.ToListAsync());
            }           
        }

        // GET: Reservas/Create
        [HttpGet("Reservas/Create")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create(int propiedadId, String aTipoElegido, DateTime fDesde, int aCantPersonas, String aTipo, DateTime fHasta, String aCiudad)
        {
            float precio = calcularPrecioReserva(propiedadId, fDesde, fHasta, aCantPersonas);
            
            if (User.IsInRole("User")) {
                ViewData["personaId"] = Int32.Parse(User.Identity.Name);
            }
            else
            {
                ViewData["personaId"] = new SelectList(_context.Usuario, "id", "DNI");
            }

            ViewData["cPrecioxDia"] = 0.0;
            ViewData["hPrecioxPersona"] = 0.0;

            using (var context = new MyContext())
            {
                var queryCabania = (from a in context.Alojamiento
                             where a.id == propiedadId
                             select a.cPrecioxDia);

                foreach (var q in queryCabania)
                {
                    ViewData["cPrecioxDia"] = q;
                }

                var queryHotel = (from a in context.Alojamiento
                                  where a.id == propiedadId
                                  select a.hPrecioxPersona);

                foreach (var q in queryHotel)
                {
                    ViewData["hPrecioxPersona"] = q;
                }
            }

            ViewData["propiedadId"] = propiedadId;
            ViewData["aCantPersonas"] = aCantPersonas;
            ViewData["aTipo"] = aTipo;
            ViewData["aCiudad"] = aCiudad;
            ViewData["aTipoElegido"] = aTipoElegido;

            String dateDesde = fDesde.ToString("yyyy-MM-dd");
            ViewData["fDesde"] = dateDesde;
            String dateHasta = fHasta.ToString("yyyy-MM-dd");
            ViewData["fHasta"] = dateHasta;
            ViewData["precio"] = precio;
            ViewData["fdHasta"] = fHasta;
            ViewData["fdDesde"] = fDesde;
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([Bind("id,fDesde,fHasta,precio,cantPersonas,propiedadId,personaId")] Reserva reserva)
        {
            if(User.IsInRole("User")){
                reserva.personaId = Int32.Parse(User.Identity.Name);
            }
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync(); 
                TempData["status"] = "Accion realizada con exito.";
                return RedirectToAction("Index");
            }
            ViewData["personaId"] = new SelectList(_context.Usuario, "id", "DNI", reserva.personaId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["personaId"] = new SelectList(_context.Usuario, "id", "DNI", reserva.personaId);
            ViewData["propiedadId"] = new SelectList(_context.Alojamiento, "id", "aCodigo", reserva.propiedadId);
            return View(reserva);
        }

        public bool modificarReserva(int id, DateTime fDesde, DateTime fHasta, int propiedadId)
        {
            bool count = true;

            using (var context = new MyContext())
            {
                var query = (from r in context.Reserva
                            where r.id != id && r.propiedadId == propiedadId
                            && ((fDesde.Date >= r.fDesde && fHasta.Date <= r.fHasta) ||
                            (fHasta.Date >= r.fDesde && fHasta.Date <= r.fHasta) ||
                            (fDesde.Date >= r.fDesde && fDesde.Date <= r.fHasta) ||
                            (fDesde.Date <= r.fDesde && fHasta.Date >= r.fHasta))
                            select r).Count();

                if(query > 0)
                {
                    count = false;
                }
            }

            return count;
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, int propiedadId, int personaId, DateTime fDesde, DateTime fHasta, int cantPersonas, [Bind("id,fDesde,fHasta,precio,cantPersonas,propiedadId,personaId")] Reserva reserva)
        {

            if (id != reserva.id)
            {
                return NotFound();
            }

            if (fDesde.Date < DateTime.Now.Date && fDesde.Year != 1)
            {
                ViewBag.Message = string.Format("La fecha desde no puede ser anterior a la fecha actual.");
                return RedirectToAction(nameof(Index));
            }
            else if (fDesde.Date > fHasta.Date)
            {
                ViewBag.Message = string.Format("La fecha desde no puede ser mayor a la fecha hasta.");
                return RedirectToAction(nameof(Index));
            }

            bool availability = modificarReserva(id, fDesde, fHasta, propiedadId);
            if (ModelState.IsValid)
            {
                if (availability) {
                    float precio = calcularPrecioReserva(propiedadId, fDesde, fHasta, cantPersonas);
                    try
                    {
                        reserva.precio = precio;
                        _context.Update(reserva);
                        await _context.SaveChangesAsync();
                        TempData["status"] = "Accion realizada con exito.";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ReservaExists(reserva.id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                TempData["status"] = "No hay disponibilidad para reservar con los datos seleccionados.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["personaId"] = new SelectList(_context.Usuario, "id", "DNI", reserva.personaId);
            ViewData["propiedadId"] = new SelectList(_context.Alojamiento, "id", "aCodigo", reserva.propiedadId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.persona)
                .Include(r => r.propiedad)
                .FirstOrDefaultAsync(m => m.id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            _context.Reserva.Remove(reserva);
            await _context.SaveChangesAsync();
            TempData["status"] = "Reserva eliminada con exito.";
            return RedirectToAction("Index");
        }

        private bool ReservaExists(int id)
        {
            return _context.Reserva.Any(e => e.id == id);
        }

        public Alojamiento devolverAlojamiento(int id)
        {
            var queryAlojamientos = from aloj in _context.Alojamiento
                                    select aloj;
    
            if (queryAlojamientos != null)
            {
                foreach (Alojamiento alojamiento in queryAlojamientos)
                {
                    if (alojamiento.id == id)
                    {
                        return alojamiento;
                    }
                }
            }
            return null;
        }

        public float calcularPrecioReserva(int codAloj, DateTime fDesde, DateTime fHasta, int cantPersonas)
        {
            float precio = 0;
            var span = fHasta.Date.Subtract(fDesde.Date);
            int dias = span.Days;
            dias = dias + 1;
            Alojamiento propiedad = devolverAlojamiento(codAloj);

            if (propiedad != null && propiedad.Tipo == "Hotel")
            {                
                precio = Convert.ToSingle(propiedad.hPrecioxPersona * cantPersonas * dias);
            }
            else if (propiedad != null)
            {
                precio = Convert.ToSingle(propiedad.cPrecioxDia * dias);
            }
            return precio;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
