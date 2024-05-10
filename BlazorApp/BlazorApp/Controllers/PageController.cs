using BlazorApp.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Controllers;

public class PageController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Example()
    {
        return View();
    }
    
    [HttpGet("page-dummy")]
    [SkipByAcceptHeader]
    public IActionResult Dummy()
    {
        return View();
    } 
}