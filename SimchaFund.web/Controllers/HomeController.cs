using SimchaFund.data;
using SimchaFund.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimchaFund.web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            IEnumerable<Simcha> simchas = db.GetSimchas();
            IndexViewModel vm = new IndexViewModel();
            vm.Simchas = simchas.Select(s => new SimchaWithCount
            {
                Simcha = s,
                ContributerCount = db.GetContrCountforSimcha(s.Id),
                ContributionTotal = db.GetContributionTotalForSimcha(s.Id)
            }); 
            vm.ContributerCount = db.GetContrCount();
            return View(vm);
        }

        public ActionResult NewSimcha()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddSimcha(Simcha simcha)
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            db.AddSimcha(simcha);
            return Redirect("/Home/Index");
        }

        public ActionResult Contributors()
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            IEnumerable<Contributor> contributors = db.GetContributors();
            List<ContributorWithBalance> contrib = new List<ContributorWithBalance>();
            decimal total = 0;
            foreach(Contributor c in contributors)
            {
                IEnumerable<Contribution> contributions = db.GetContributionsByContributor(c.Id);
                IEnumerable<Deposit> deposits = db.GetDepositsByContributor(c.Id);
                decimal balance = deposits.Sum(dep => dep.Amount) - contributions.Sum(contr => contr.Amount);
                total += balance;
                contrib.Add(new ContributorWithBalance
                {
                    Contributor = c,
                    Balance = balance
                });

            }
            ContributorViewModel vm = new ContributorViewModel();
            vm.Contributors = contrib;
            vm.Total = total;
            return View(vm);
        }

        public ActionResult NewContributor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddContributor(string firstName, string lastName, string cell, bool alwaysInclude, DateTime dateAdded, decimal amount)
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            Contributor c = new Contributor
            {
                FirstName = firstName,
                LastName = lastName,
                Cell = cell,
                AlwaysInclude = alwaysInclude,
                DateAdded = dateAdded
            };
            int id = db.AddContributor(c);
            Deposit d = new Deposit
            {
                Date = dateAdded,
                Amount = amount,
                ContributorId = id
            };
            db.AddDeposit(d);
            return Redirect("/Home/Contributors/");
        }

        public ActionResult NewContributions(int simchaId)
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            NewContributionsViewModel vm = new NewContributionsViewModel();
            var Contributors = db.GetContributors();
            List<SimchaContributor> SimchaContributors = new List<SimchaContributor>();
            foreach (Contributor c in Contributors)
            {
                var sc = new SimchaContributor{
                    Contributor = c,
                    Contributions = db.GetContibutorsContributionsForSimcha(simchaId, c.Id)
                };
                SimchaContributors.Add(sc);
  
            }
            vm.Contributors = SimchaContributors;
            vm.Simcha = db.GetSimchaById(simchaId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddContributions(int simchaId, List<Contribution>contributions)
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            db.DeleteSimchaContributions(simchaId);
            foreach(Contribution c in contributions)
            {
                if(c.ContributorId != 0)
                {
                    db.AddContribution(c);
                }
            }
            return Redirect("/Home/Index/");
        }

        public ActionResult NewDeposit(int id)
        {
            DepositViewModel vm = new DepositViewModel();
            vm.ContributorId = id;
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddDeposit(Deposit deposit)
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            db.AddDeposit(deposit);
            return Redirect("/Home/Contributors/");
        }

        public ActionResult History(int contributorId, string name, decimal balance)
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            
            List<Transaction> trans = new List<Transaction>();
            IEnumerable<Contribution> c = db.GetContributionsByContributor(contributorId);
            foreach (Contribution cont in c )
            {
                Simcha s = db.GetSimchaById(cont.SimchaId);
                trans.Add(new Transaction
                {
                    Action = $"Contribution to the {s.Name} simcha",
                    Date = cont.Date,
                    Amount = cont.Amount
                });
            }
            IEnumerable<Deposit> deposits = db.GetDepositsByContributor(contributorId);
            foreach (Deposit d in deposits)
            {
                trans.Add(new Transaction
                {
                    Action = "Deposit",
                    Date = d.Date,
                    Amount = d.Amount
                });
            }
            //trans.Sort((x, y) => x.Date.CompareTo(y.Date));
            HistoryViewModel vm = new HistoryViewModel
            {
                Transactions = trans,
                Name = name,
                Balance = balance
            };
            return View(vm);

        }

        public ActionResult EditContributor(int id)
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            EditContributorViewModel vm = new EditContributorViewModel();
            vm.Contributor = db.GetContributorById(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateContributer(Contributor contributer)
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            db.UpdateContributor(contributer);
            return Redirect("/Home/Contributors");
        }

        public ActionResult ContributorList(int id, string name)
        {
            SimchaFundDb db = new SimchaFundDb(Properties.Settings.Default.ConStr);
            ContributorListViewModel vm = new ContributorListViewModel();
            vm.Contributors = db.GetContributorsBySimchaId(id);
            vm.SimchaName = name;
            return View(vm);
        }
    }
}