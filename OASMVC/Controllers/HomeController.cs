using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OASMVC.Hubs;
using OASMVC.Infrastructure;
using OASMVC.Models;
using OASMVC.Repository;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.Controllers
{
    public class HomeController : Controller
    {
        private OASData.Data oasData = new OASData.Data();
        private OASConfig.Config oasConfig = new OASConfig.Config();
        private IHubContext<OasTagHub> _oasHub;
        private IOASRepository _oasRepository;
        private IOpenAutomationSoftware _oasComponent;

        public HomeController(IOASRepository oasRepository, IOpenAutomationSoftware oasComponents, IHubContext<OasTagHub> oasHub)
        {
            _oasHub = oasHub;
            _oasRepository = oasRepository;
            _oasComponent = oasComponents;

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
            _oasComponent.GetOASVersion(nodeName);
            return RedirectToAction("Index");
        }

        [HttpPost("{groupName}/{nodeName}", Name ="ChangeGroup")]
        public IActionResult ChangeGroup([FromRoute] string groupName, string nodeName)
        {
            if (groupName == "Root")
                groupName = "";
            
            _oasComponent.GetTagList(nodeName, groupName);
            //_oasComponent.AddTags(groupName, nodeName);
            return RedirectToAction("Index");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
