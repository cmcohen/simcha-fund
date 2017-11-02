using SimchaFund.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.web.Models
{
    public class NewContributionsViewModel
    {
        public IEnumerable<SimchaContributor> Contributors { get; set; }
        public Simcha Simcha { get; set; }
    }
}