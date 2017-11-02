using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.web.Models
{
    public class HistoryViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

    }
}