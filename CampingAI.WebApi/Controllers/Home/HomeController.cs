using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.Home;
public class HomeController : Controller {
    #region Dependencias
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    #endregion

    public HomeController(IWebHostEnvironment environment, IConfiguration configuration) {
        _environment = environment;
        _configuration = configuration;
    }

    public IActionResult Index() {
        return View();
    }

    [HttpGet]
    public IActionResult DownloadApk() {
        var relativePath = _configuration["AndroidApp:ApkRelativePath"];
        var downloadFileName = _configuration["AndroidApp:DownloadFileName"] ?? "camping-ai.apk";

        if (string.IsNullOrWhiteSpace(relativePath))
            return NotFound();

        var physicalPath = System.IO.Path.Combine(_environment.ContentRootPath, relativePath);

        if (!System.IO.File.Exists(physicalPath))
            return NotFound();

        return PhysicalFile(physicalPath, "application/vnd.android.package-archive", downloadFileName);
    }
}