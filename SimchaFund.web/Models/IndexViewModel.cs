using SimchaFund.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.web.Models
{
    public class IndexViewModel
    {
        public IEnumerable<SimchaWithCount> Simchas { get; set; }

        public int ContributerCount { get; set; }
    }
}