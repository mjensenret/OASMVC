using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using OASMVC.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.Repository
{
    public class OASRepository : IOASRepository
    {
        public OASData.Data oasData = new OASData.Data();
        private OASConfig.Config oasConfig = new OASConfig.Config();

        private IHubContext<OasTagHub> _hubContext;

        public OASRepository(IHubContext<OasTagHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public int GetOASVersion(string networkNode)
        {
            OASVersion version = new OASVersion();
            version.Version = oasConfig.GetVersion(networkNode);

            _hubContext.Clients.All.SendAsync("displayVersion", version.Version);

            return version.Version;
        }

        public IEnumerable<TagList> GetTagList(string networkNode)
        {
            List<TagList> tagList = new List<TagList>();
            var tags = oasConfig.GetTagNames("", networkNode);

            foreach(var t in tags)
            {
                tagList.Add(new TagList { TagName = t });
            }

            _hubContext.Clients.All.SendAsync("updateTagList", tagList);

            return tagList;
        }
    }
}
