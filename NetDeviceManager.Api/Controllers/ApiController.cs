using Microsoft.AspNetCore.Mvc;
using NetDeviceManager.Database;
using FileInfo = System.IO.FileInfo;

namespace NetDeviceManager.Api.Controllers;

public class ApiController : Controller
{
    private readonly ILogger<ApiController> _logger;
    private readonly string _path = string.Empty;
    private readonly IHostEnvironment _environment;
    private readonly ApplicationDbContext _database;

    public ApiController(ApplicationDbContext context, ILogger<ApiController> logger, IConfiguration configurationManager,
        IHostEnvironment environment)
    {
        _logger = logger;
        _path = Path.Combine(environment.ContentRootPath, "reports");
        
        //todo: remove for production
        if(environment.IsDevelopment())
            _path = Path.Combine("C:\\Users\\mortar\\RiderProjects\\NetDeviceManager", "reports");
        
        _environment = environment;
        Console.WriteLine(environment.ContentRootPath);
        _database = context;
    }

    public IActionResult GetReportsList()
    {
        if (Directory.Exists(_path))
        {
            var list = Directory.GetFiles(_path);
            List<string> output = new List<string>();
            foreach (var item in list)
            {
                var info = new FileInfo(item);
                output.Add(info.Name);
            }
            
            return Json(output.ToArray());
        }

        return Json(Array.Empty<string>());
    }

    public IActionResult GetReport(string id)
    {
        Console.WriteLine($"Id: {id}");
        var path = Path.Combine(_path, id);
        if (System.IO.File.Exists(path))
            return File(System.IO.File.ReadAllBytes(path), "application/zip", System.IO.Path.GetFileName(path));
        return BadRequest();
    }
}