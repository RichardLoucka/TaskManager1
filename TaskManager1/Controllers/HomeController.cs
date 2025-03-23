using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager1.Models;
using Microsoft.EntityFrameworkCore;
using Task = TaskManager1.Models.Task;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager1.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string sortOrder, int? categoryId, string searchString)
    {
        ViewData["SortOrder"] = sortOrder == "asc" ? "desc" : "asc";
        var tasks = _context.Tasks.Include(t => t.Category).AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            tasks = tasks.Where(t => t.Title.Contains(searchString));
        }

        if (categoryId.HasValue)
        {
            tasks = tasks.Where(t => t.CategoryId == categoryId.Value);
        }
        tasks = sortOrder == "asc" ? tasks.OrderBy(t => t.Priority) : tasks.OrderByDescending(t => t.Priority);
        ViewData["Categories"] = await _context.Categories.ToListAsync();
        return View(await tasks.ToListAsync());
    }

    public IActionResult Create()
    {
        ViewData["Categories"] = _context.Categories.ToList();
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

    public async Task<IActionResult> ExportToJson()
    {
        var tasks = await _context.Tasks.ToListAsync();
        var taskExportList = tasks.Select(t => new TaskToExport
        {
            Title = t.Title,
            Priority = t.Priority,
            Description = t.Description
        }).ToList();
        
        var json = JsonConvert.SerializeObject(taskExportList, Newtonsoft.Json.Formatting.Indented);
        var fileName = "tasks.json";
        var fileBytes = Encoding.UTF8.GetBytes(json);
        return File(fileBytes, "application/json", fileName);
    }

    [HttpPost]
    public async Task<IActionResult> ImportFromJson(IFormFile jsonFile)
    {
        if (jsonFile == null || jsonFile.Length == 0)
        {
            return RedirectToAction(nameof(Index));
        }

        using (var stream = new StreamReader(jsonFile.OpenReadStream()))
        {
            var jsonString = await stream.ReadToEndAsync();
            var importedTasks = JsonConvert.DeserializeObject<List<Task>>(jsonString);

            if (importedTasks != null && importedTasks.Any())
            {
                foreach (var task in importedTasks)
                {
                    if (string.IsNullOrEmpty(task.FileName))
                    {
                        task.FileName = "";
                    }
                }
                _context.Tasks.AddRange(importedTasks);
                await _context.SaveChangesAsync();
            }
        }
        return RedirectToAction(nameof(Index));
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