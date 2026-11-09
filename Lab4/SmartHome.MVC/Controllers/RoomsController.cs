using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.Infrastructure;
using SmartHome.Infrastructure.Models;

namespace SmartHome.MVC.Controllers
{
    public class RoomsController : Controller
    {
        private readonly SmartHomeContext _context;

        public RoomsController(SmartHomeContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rooms.AsNoTracking().OrderBy(room => room.Name).ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomModel = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roomModel == null)
            {
                return NotFound();
            }

            return View(roomModel);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Floor")] RoomModel roomModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roomModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(roomModel);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomModel = await _context.Rooms.FindAsync(id);
            if (roomModel == null)
            {
                return NotFound();
            }
            return View(roomModel);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Floor")] RoomModel roomModel)
        {
            if (id != roomModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Rooms.AnyAsync(room => room.Id == id))
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
            return View(roomModel);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomModel = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roomModel == null)
            {
                return NotFound();
            }

            return View(roomModel);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var roomModel = await _context.Rooms.FindAsync(id);
            if (roomModel != null)
            {
                _context.Rooms.Remove(roomModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
