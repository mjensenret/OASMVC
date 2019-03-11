using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OASMVC.Models;
using OASMVC.Repository;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.Controllers
{
    public class HomeController : Controller
    {
        private OASData.Data oasData = new OASData.Data();
        private OASConfig.Config oasConfig = new OASConfig.Config();
        private IOASRepository _oasRepository;

        public HomeController(IOASRepository oasRepository)
        {
            _oasRepository = oasRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost("{nodeName}", Name ="ChangeNetworkNode")]
        public IActionResult ChangeNetworkNode([FromRoute] string nodeName)
        {
            _oasRepository.GetOASVersion(nodeName);
            _oasRepository.GetTagList(nodeName);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
