using Microsoft.AspNetCore.Mvc;
using FileInfo = System.IO.FileInfo;

namespace NetDeviceManager.Api.Controllers;

public class ApiController : Controller
{
    private readonly ILogger<ApiController> _logger;
    private readonly string _path = string.Empty;
    private readonly IHostEnvironment _environment;

    public ApiController(ILogger<ApiController> logger, IConfiguration configurationManager,
        IHostEnvironment environment)
    {
        _logger = logger;
        _path = Path.Combine(environment.ContentRootPath, "reports");
        _environment = environment;
        Console.WriteLine(environment.ContentRootPath);
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

        return Json(_path);
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