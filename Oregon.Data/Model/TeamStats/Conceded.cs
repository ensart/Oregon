﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oregon.Data.Model.TeamStats
{
    public class Conceded : SimpleViewModel
    {
        public int total { get; set; }
        public List<Period> period { get; set; }
    }
}
