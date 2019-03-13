using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OASMVC.Models
{
    public class OASTagModels
    {
        public class OASNetworkNodes
        {
            public string NodeName { get; set; }
        }

        public class OASVersion
        {
            public int Version { get; set; }
        }

        public class TagList
        {
            public string TagName { get; set; }
            public string Value { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        public class TagsAndValues
        {
            public string TagName { get; set; }
            public string Value { get; set; }
        }
    }

}
