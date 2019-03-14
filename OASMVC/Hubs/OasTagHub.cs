using Microsoft.AspNetCore.SignalR;
using OASMVC.Infrastructure;
using OASMVC.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.Hubs
{
    public class OasTagHub : Hub
    {
        private IOpenAutomationSoftware _oas;

        public OasTagHub(IOpenAutomationSoftware oas)
        {
            _oas = oas;
        }

        public List<TagList> GetTagLists(string nodeName, string groupName)
        {
            return _oas.GetTagList(nodeName, groupName);
        }
    }
}
