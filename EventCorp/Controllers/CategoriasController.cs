using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventCorp.Models;
using Q_Manage.Models;
using EventCorp.Data;

[Authorize]
public class CategoriasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CategoriasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var categorias = await _context.Categorias
            .Include(c => c.Usuario)
            .ToListAsync();
        return View("Index", categorias);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Categoria categoria)
    {
        if (!ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            categoria.FechaRegistro = DateTime.Now;
            categoria.UsuarioRegistro = user.Id;

            _context.Add(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(categoria);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
            return NotFound();

        return View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Categoria categoria)
    {
        if (id != categoria.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            try
            {
                var original = await _context.Categorias.FindAsync(id);
                if (original == null)
                    return NotFound();

                original.Nombre = categoria.Nombre;
                original.Descripcion = categoria.Descripcion;
                original.Estado = categoria.Estado;

                _context.Update(original);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categorias.Any(e => e.Id == categoria.Id))
                    return NotFound();
                else
                    throw;
            }
        }

        return View(categoria);

    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(m => m.Id == id);
        if (categoria == null)
            return NotFound();

        return View(categoria);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
