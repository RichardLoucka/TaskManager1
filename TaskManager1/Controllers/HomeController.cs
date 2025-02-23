using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManager1.Models;
using Microsoft.EntityFrameworkCore;
using Task = TaskManager1.Models.Task;

namespace TaskManager1.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string sortOrder)
    {
        ViewData["SortOrder"] = sortOrder == "asc" ? "desc" : "asc";
        var tasks = _context.Tasks.AsQueryable();
        tasks = sortOrder == "asc" ? tasks.OrderBy(t => t.Priority) : tasks.OrderByDescending(t => t.Priority);
        return View(await tasks.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Task task, IFormFile file)
    {
        if (file != null)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/", file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            task.FileName = file.FileName;
        }

        if (string.IsNullOrEmpty(task.FileName))
        {
            task.FileName = "";
        }

        _context.Add(task);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int[] selectedTasks)
    {
        if (selectedTasks != null && selectedTasks.Any())
        {
            var tasksToRemove = _context.Tasks.Where(t => selectedTasks.Contains(t.Id)).ToList();
            
            if (tasksToRemove.Any())
            {
                _context.Tasks.RemoveRange(tasksToRemove);
                await _context.SaveChangesAsync();
            }
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Download(string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/", fileName);

        if (System.IO.File.Exists(filePath))
        {
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }
        return NotFound();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}