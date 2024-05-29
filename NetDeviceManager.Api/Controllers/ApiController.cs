using Microsoft.AspNetCore.Mvc;
using NetDeviceManager.Database;
using FileInfo = System.IO.FileInfo;

namespace NetDeviceManager.Api.Controllers;

public class ApiController : Controller
{
    private readonly ILogger<ApiController> _logger;
    private readonly string _path = string.Empty;
    private readonly ApplicationDbContext _database;

    public ApiController(ApplicationDbContext context, ILogger<ApiController> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _path = "reports";
        Console.WriteLine(environment.ContentRootPath);
        _database = context;
    }

    public IActionResult GetReportsList(string key)
    {
        try
        {
            if (_database.Users.All(x => x.ApiKey != key))
                return Unauthorized();
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
        }
        catch (Exception e)
        {
            return BadRequest();
        }

        return BadRequest();
    }

    public IActionResult GetReport(string id, string key)
    {
        try
        {
            if (_database.Users.All(x => x.ApiKey != key))
                return Unauthorized();
            Console.WriteLine($"Id: {id}");
            var path = Path.Combine(_path, id);
            if (System.IO.File.Exists(path))
                return File(System.IO.File.ReadAllBytes(path), "application/zip", id);
        }
        catch (Exception e)
        {
            return BadRequest();
        }

        return BadRequest();
    }
}