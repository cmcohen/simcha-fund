using SimchaFund.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.web.Models
{
    public class ContributorViewModel
    {
        public IEnumerable<ContributorWithBalance> Contributors { get; set; }

        public decimal Total { get; set; }


    }
}