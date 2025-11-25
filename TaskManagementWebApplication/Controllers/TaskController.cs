using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TaskManagementWebApplication.Data;
using TaskManagementWebApplication.Models;

namespace TaskManagementWebApplication.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TaskController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var tasks = await _context.Tasks
                .Where(t => t.UserId == user.Id)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            return View(tasks);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            model.UserId = user.Id;

            _context.Tasks.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (task == null) return NotFound();

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);

            var existing = await _context.Tasks
                 .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (existing == null) return Unauthorized();

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.DueDate = model.DueDate;
            existing.Status = model.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (task == null) return NotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (task == null) return Unauthorized();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
