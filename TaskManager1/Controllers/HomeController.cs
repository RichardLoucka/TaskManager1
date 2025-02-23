using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManager1.Models;
using Microsoft.EntityFrameworkCore;
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