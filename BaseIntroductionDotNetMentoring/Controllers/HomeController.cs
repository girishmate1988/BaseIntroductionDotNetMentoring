using BaseIntroductionDotNetMentoring.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BaseIntroductionDotNetMentoring.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            _logger = logger;
        }


        [ValidateAntiForgeryToken]
        public IActionResult Index()
        {
            _logger.LogInformation("Home Index page accessed at {DateTime}", DateTime.UtcNow);
            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult Privacy()
        {
            _logger.LogInformation("Privacy page accessed at {DateTime}", DateTime.UtcNow);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature?.Error is { } exception)
                _logger.LogError(exception, "An error occurred. RequestId: {RequestId}, Path: {Path}",
                    requestId, exceptionFeature.Path);
            else
                _logger.LogError("An error occurred. RequestId: {RequestId}", requestId);

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
