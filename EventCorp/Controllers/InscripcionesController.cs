using EventCorp.Data;
using EventCorp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Q_Manage.Models;

public class InscripcionesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public InscripcionesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Participantes(int eventoId)
    {
        var evento = await _context.Eventos
            .Include(e => e.Inscripciones)
                .ThenInclude(i => i.Usuario)
            .FirstOrDefaultAsync(e => e.Id == eventoId);

        if (evento == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(int eventoId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var evento = await _context.Eventos
            .Include(e => e.Inscripciones)
            .FirstOrDefaultAsync(e => e.Id == eventoId);

        if (evento == null)
            return NotFound();

        if (_context.Inscripciones.Any(i => i.UsuarioId == user.Id && i.EventoId == eventoId))
        {
            TempData["Error"] = "Ya estás inscrito en este evento.";
            return RedirectToAction("ResumenInscripcion", "Inscripciones", new { eventoId = eventoId });
        }

        int inscritos = await _context.Inscripciones.CountAsync(i => i.EventoId == eventoId);
        if (inscritos >= evento.CupoMaximo)
        {
            TempData["Error"] = "El cupo para este evento ya está lleno.";
            return RedirectToAction("ResumenInscripcion", "Inscripciones", new { eventoId = eventoId });
        }

        var fechaHoraInicio = evento.Fecha.Add(evento.Hora);
        var fechaHoraFin = fechaHoraInicio.AddMinutes(evento.DuracionMinutos);

        var inscripciones = await _context.Inscripciones
            .Include(i => i.Evento)
            .Where(i => i.UsuarioId == user.Id)
            .ToListAsync();

        var cruce = inscripciones.Any(i =>
        {
            var inicioExistente = i.Evento.Fecha.Date + i.Evento.Hora;
            var finExistente = inicioExistente.AddMinutes(i.Evento.DuracionMinutos);

            return fechaHoraInicio < finExistente && fechaHoraFin > inicioExistente;
        });

        if (cruce)
        {
            TempData["Error"] = "Ya estás inscrito en un evento que se cruza en fecha y hora.";
            return RedirectToAction("ResumenInscripcion", "Inscripciones", new { eventoId = eventoId });
        }

        var inscripcion = new Inscripcion
        {
            EventoId = eventoId,
            UsuarioId = user.Id,
            Asistio = false
        };

        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Te has inscrito correctamente al evento.";
        return RedirectToAction("Confirmacion", "Inscripciones", new { eventoId = eventoId });
    }

    public async Task<IActionResult> Confirmacion(int eventoId)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == eventoId);
        if (evento == null) return NotFound();

        return View(evento);
    }

    [Authorize]
    public async Task<IActionResult> ResumenInscripcion(int eventoId)
    {
        var evento = await _context.Eventos
            .Include(e => e.Categoria)
            .FirstOrDefaultAsync(e => e.Id == eventoId);

        if (evento == null)
            return NotFound();

        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "organizador,administrador")]
    public async Task<IActionResult> MarcarAsistencia(int inscripcionId)
    {
        var inscripcion = await _context.Inscripciones
            .Include(i => i.Evento)
            .FirstOrDefaultAsync(i => i.Id == inscripcionId);

        if (inscripcion == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (User.IsInRole("organizador") && inscripcion.Evento.UsuarioRegistroId != user.Id)
            return Forbid();

        inscripcion.Asistio = true;
        _context.Update(inscripcion);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Asistencia marcada correctamente.";
        return RedirectToAction("Participantes", new { eventoId = inscripcion.EventoId });
    }


}
