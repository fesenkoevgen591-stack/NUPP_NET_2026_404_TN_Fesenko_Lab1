using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartHome.Infrastructure;
using SmartHome.Infrastructure.Models;

namespace SmartHome.MVC.Controllers
{
    public class DevicesController : Controller
    {
        private readonly SmartHomeContext _context;

        public DevicesController(SmartHomeContext context)
        {
            _context = context;
        }

        // GET: Devices
        public async Task<IActionResult> Index()
        {
            var smartHomeContext = _context.Devices.AsNoTracking().Include(d => d.Room).OrderBy(d => d.Name);
            return View(await smartHomeContext.ToListAsync());
        }

        // GET: Devices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceModel = await _context.Devices
                .Include(d => d.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deviceModel == null)
            {
                return NotFound();
            }

            return View(deviceModel);
        }

        // GET: Devices/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms.AsNoTracking().OrderBy(room => room.Name), "Id", "Name");
            return View();
        }

        // POST: Devices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IsOnline,RoomId")] DeviceModel deviceModel)
        {
            await ValidateRoomAsync(deviceModel.RoomId);
            if (ModelState.IsValid)
            {
                _context.Add(deviceModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms.AsNoTracking().OrderBy(room => room.Name), "Id", "Name", deviceModel.RoomId);
            return View(deviceModel);
        }

        // GET: Devices/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceModel = await _context.Devices.FindAsync(id);
            if (deviceModel == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms.AsNoTracking().OrderBy(room => room.Name), "Id", "Name", deviceModel.RoomId);
            return View(deviceModel);
        }

        // POST: Devices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,IsOnline,RoomId")] DeviceModel deviceModel)
        {
            if (id != deviceModel.Id)
            {
                return NotFound();
            }

            await ValidateRoomAsync(deviceModel.RoomId);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deviceModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Devices.AnyAsync(device => device.Id == id))
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
            ViewData["RoomId"] = new SelectList(_context.Rooms.AsNoTracking().OrderBy(room => room.Name), "Id", "Name", deviceModel.RoomId);
            return View(deviceModel);
        }

        // GET: Devices/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceModel = await _context.Devices
                .Include(d => d.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deviceModel == null)
            {
                return NotFound();
            }

            return View(deviceModel);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var deviceModel = await _context.Devices.FindAsync(id);
            if (deviceModel != null)
            {
                _context.Devices.Remove(deviceModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateRoomAsync(Guid roomId)
        {
            // The form binds the foreign key. EF resolves the navigation property.
            ModelState.Remove(nameof(DeviceModel.Room));
            if (ModelState.IsValid && !await _context.Rooms.AnyAsync(room => room.Id == roomId))
            {
                ModelState.AddModelError(nameof(DeviceModel.RoomId), "Виберіть наявну кімнату.");
            }
        }
    }
}
