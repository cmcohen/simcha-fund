using SimchaFund.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.web.Models
{
    public class SimchaWithCount
    {
       public Simcha Simcha { get; set; }
       public int ContributerCount { get; set; }
       public decimal ContributionTotal { get; set; }
    }
}