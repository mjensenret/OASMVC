﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.Repository
{
    public interface IOASRepository
    {
        int GetOASVersion(string networkNode);

        IEnumerable<TagList> GetTagList(string networkNode);
    }
}