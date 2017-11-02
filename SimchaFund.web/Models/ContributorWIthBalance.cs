using SimchaFund.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.web.Models
{
    public class ContributorWithBalance
    {
        public Contributor Contributor { get; set; }
        public decimal Balance { get; set; }
    }
}