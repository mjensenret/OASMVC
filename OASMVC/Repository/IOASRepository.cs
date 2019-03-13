using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.Repository
{
    public interface IOASRepository
    {
        int GetOASVersion(string networkNode);

        List<TagList> GetTagList(string networkNode, string groupName);

        List<TagList> GetTagsAndValues(string networkNode, string groupName);
    }
}
