using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.Infrastructure
{
    public interface IOpenAutomationSoftware
    {
        void GetOASVersion(string networkNode);
        List<TagList> GetTagList(string nodeName, string groupName);
        void AddTags(string nodeName, string groupName);
        void RemoveAllTags();
        List<ChartData> LoadChartData();
    }
}
