using System.Diagnostics;
using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContainerService _containerService;
        private readonly ILogger _logger;
        public HomeController(ILogger<HomeController> logger,IContainerService containerService)
        {
            _logger = logger;
            _containerService = containerService;
        }
        public IActionResult Index()
        {
            return View(_containerService.GetAllContainerAndBlobs().GetAwaiter().GetResult());
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
}
