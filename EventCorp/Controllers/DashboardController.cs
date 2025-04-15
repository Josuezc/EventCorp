using EventCorp.Data;
using EventCorp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventCorp.Controllers
{
    [Authorize(Roles = "administrador")]
    public class DashboardController  : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var mesActual = hoy.Month;
            var anioActual = hoy.Year;

            var totalEventos = await _context.Eventos.CountAsync();

            var totalUsuarios = await _context.Users.CountAsync();

            var asistentesxMes = await _context.Inscripciones
                .Where(i => i.Evento.Fecha.Month == mesActual && i.Evento.Fecha.Year == anioActual)
                .CountAsync();

            var top5Eventos = await _context.Eventos
             .Include(e => e.Inscripciones)
             .OrderByDescending(e => e.Inscripciones.Count)
             .Take(5)
             .ToListAsync();

            var viewModel = new Dashboard
            {
                TotalEventos = totalEventos,
                TotalUsuarios = totalUsuarios,
                AsistentesxMes = asistentesxMes,
                Top5Eventos = top5Eventos
            };

            return View(viewModel);
        }
    }
}
