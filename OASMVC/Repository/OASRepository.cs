using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using OASMVC.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OASMVC.Models.OASTagModels;
using System.Diagnostics;
using OASMVC.Infrastructure;

namespace OASMVC.Repository
{
    public class OASRepository : IOASRepository
    {
        private IHubContext<OasTagHub> _hubContext;
        private IOpenAutomationSoftware _oasComponents;

        public OASRepository(IHubContext<OasTagHub> hubContext, IOpenAutomationSoftware oas)
        {
            _hubContext = hubContext;
            _oasComponents = oas;          
        }

    }
}
