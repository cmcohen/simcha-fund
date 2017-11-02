using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.data
{
    public class SimchaContributor
    {
        public Contributor Contributor { get; set; }
        public IEnumerable<Contribution> Contributions { get; set; }
    }
}
