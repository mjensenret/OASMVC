using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagAPIController : ControllerBase
    {
        private OASConfig.Config oasConfig = new OASConfig.Config();

        //[HttpGet(Name ="GetNetworkNodes")]
        public List<OASNetworkNodes> GetNetworkNodes()
        {
            List<OASNetworkNodes> nodes = new List<OASNetworkNodes>()
            {
                new OASNetworkNodes {NodeName = "localhost"},
                new OASNetworkNodes {NodeName = "ops-dev.savageservices.com"},
                new OASNetworkNodes {NodeName = "13-01-0110.ssc.savageservices.com"}
            };

            return nodes;
        }

        //[HttpGet(Name ="GetTagList")]
        //public List<TagList> GetTagList(string networkNode)
        //{
        //    List<TagList> tagList = new List<TagList>();
        //    var tags = oasConfig.GetTagNames();

        //    foreach(var t in tags)
        //    {
        //        tagList.Add(new TagList { TagName = t });
        //    }

        //    return tagList;
        //}
    }
}