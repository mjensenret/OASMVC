using Microsoft.AspNetCore.SignalR;
using OASMVC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.Hubs
{
    public class OasTagHub : Hub
    {
        private readonly IOASRepository oASRepository;

        //public IEnumerable<TagList> GetTags(string networkNode)
        //{
        //    return oASRepository.GetTagList(networkNode);
        //}
    }
}
