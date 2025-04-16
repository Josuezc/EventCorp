using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventCorp.Data;
using EventCorp.Models;
using Microsoft.AspNetCore.Identity;
using Q_Manage.Models;
using Microsoft.AspNetCore.Authorization;

namespace EventCorp.Controllers
{
    public class EventoController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EventoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager )  
            
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Disponibles()
        {
            var hoy = DateTime.Today;
            var user = await _userManager.GetUserAsync(User);

            var eventos = await _context.Eventos
                .Include(e => e.Categoria)
                .Where(e => e.Fecha >= hoy)
                .OrderBy(e => e.Fecha)
                .ToListAsync();

            var inscritos = await _context.Inscripciones
                .Where(i => i.UsuarioId == user.Id)
                .Select(i => i.EventoId)
                .ToListAsync();

            ViewBag.EventosInscritos = inscritos;

            return View(eventos);
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Eventos.Include(e => e.Categoria).Include(e => e.UsuarioRegistro);
            return View(await applicationDbContext.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.Categoria)
                .Include(e => e.UsuarioRegistro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
               return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descripcion,CategoriaId,Fecha,Hora,DuracionMinutos,Ubicacion,CupoMaximo")] Evento evento)
        {
            evento.FechaRegistro = DateTime.Now;
            evento.UsuarioRegistroId = _userManager.GetUserId(User);
            if (evento.Fecha < DateTime.Today)
            {
                ModelState.AddModelError("Fecha", "La fecha del evento no puede estar en el pasado.");
            }
            if (evento.DuracionMinutos <= 0)
            {
                ModelState.AddModelError("DuracionMinutos", "La duración debe ser mayor a 0.");
            }
            if (evento.CupoMaximo <= 0)
            {
                ModelState.AddModelError("CupoMaximo", "El cupo máximo debe ser mayor a 0.");
            }
           
            if (ModelState.IsValid)
            {
                
                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", evento.CategoriaId);
             return View(evento);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", evento.CategoriaId);
            ViewData["UsuarioRegistroId"] = new SelectList(_context.Users, "Id", "UserName", evento.UsuarioRegistroId);
            return View(evento);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descripcion,CategoriaId,Fecha,Hora,DuracionMinutos,Ubicacion,CupoMaximo,FechaRegistro,UsuarioRegistroId")] Evento evento)
        {
            if (id != evento.Id)
            {
                return NotFound();
            }
            if (evento.Fecha < DateTime.Today)
            {
                ModelState.AddModelError("Fecha", "La fecha del evento no puede estar en el pasado.");
            }
            if (evento.DuracionMinutos <= 0)
            {
                ModelState.AddModelError("DuracionMinutos", "La duración debe ser mayor a 0.");
            }
            if (evento.CupoMaximo <= 0)
            {
                ModelState.AddModelError("CupoMaximo", "El cupo máximo debe ser mayor a 0.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.Id))
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
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", evento.CategoriaId);
            ViewData["UsuarioRegistroId"] = new SelectList(_context.Users, "Id", "UserName", evento.UsuarioRegistroId);
            return View(evento);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.Categoria)
                .Include(e => e.UsuarioRegistro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }
    }
}
