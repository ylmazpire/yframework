using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SslWatchman.Data;
using SslWatchman.Models;

namespace SslWatchman.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
            return View("Welcome");

        var domainler = await _context.IzlenenDomainler
            .OrderBy(d => d.SertifikaBitisUtc ?? DateTime.MaxValue)
            .ThenBy(d => d.Host)
            .ToListAsync();

        return View(new PanoViewModel { Domainler = domainler });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
