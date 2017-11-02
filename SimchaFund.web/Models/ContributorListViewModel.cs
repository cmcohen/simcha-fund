using SimchaFund.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.web.Models
{
    public class ContributorListViewModel
    {
        public IEnumerable<Contributor> Contributors { get; set; }
        public string SimchaName { get; set; }

    }
}