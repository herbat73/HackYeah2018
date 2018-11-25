using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HackYeah2018.Models;
using System.Net.Http;
using System.Text;
using HackYeah2018.APIHelperClass;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using HackYeah2018.Services;
using System.Linq;
using System.Collections.Generic;

namespace HackYeah2018.Controllers
{
    public class HomeController : Controller
    {
        private readonly APISettingsConfig _APISettingsConfig;
        private readonly IHostingEnvironment _environment;
        private readonly ISessionTempDataServices _sessionTempDataServices;

        public HomeController(IConfiguration config, 
                             IHostingEnvironment environment,
                             ISessionTempDataServices sessionTempDataServices)
        {
            _environment = environment;
            _sessionTempDataServices = sessionTempDataServices;

            _APISettingsConfig = new APISettingsConfig
            {
                SERVER_PATH = config["ApiSettings:SERVER_PATH"],
                TOKEN_PATH = config["ApiSettings:TOKEN_PATH"],
                ACCOUNTS_PATH = config["ApiSettings:ACCOUNTS_PATH"],
                ACCOUNT_PATH = config["ApiSettings:ACCOUNT_PATH"],
                TRANSACTIONS_PATH = config["ApiSettings:TRANSACTIONS_PATH"],
                API_KEY = config["ApiSettings:API_KEY"],
                AUTH_BARERE_PREFIX = config["ApiSettings:AUTH_BARERE_PREFIX"],
                X_JWS_SIGNATURE = config["ApiSettings:X_JWS_SIGNATURE"]
            };
        }

        public IActionResult Index()
        {
            var selectedCustomer = _sessionTempDataServices.GetCurrentCustomer();

            return View(selectedCustomer);
        }

        public IActionResult ClearCustomerData()
        {
            _sessionTempDataServices.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult SelectCustomer()
        {
            var selectedCustomer = _sessionTempDataServices.GetCurrentCustomer();

            return View(selectedCustomer);
        }

        private bool IsCustomerInDB(SelectedCustomer selectedCustomer)
        {
            var ret = false;

            string[] pathParams = { @"ExternalData", "clients.csv" };
            var fileFullPath = Path.Combine(_environment.WebRootPath, Path.Combine(pathParams));
            var externalData = ExternalDBHelper.LoadExternalData(fileFullPath);

            IEnumerable<ClientAccount> results = externalData.Where(x => x.BankId == selectedCustomer.Bank.ToString() && x.ClientId == selectedCustomer.CustomerNumber.PadLeft(8,'0'));
            ret = results.Count() != 0;

            return ret;
        }

        public async Task<IActionResult> CheckCustomer(SelectedCustomer selectedCustomer)
        {
            if (!IsCustomerInDB(selectedCustomer))
                return RedirectToAction("NoSuchAsCustomer", new { customerNumber = selectedCustomer.CustomerNumber, bank = selectedCustomer.Bank });

            var client = new HttpClient();
            var token = await APIHelper.GetTokenAsync(_APISettingsConfig, client, selectedCustomer.CustomerNumber.PadLeft(8, '0'), selectedCustomer.Bank.ToString());

            _sessionTempDataServices.SetToken(token);
            _sessionTempDataServices.SetCurrentCustomer(selectedCustomer);

            return RedirectToAction("ShowAccounts", "Home");
        }


        public async Task<IActionResult> ShowAccounts()
        {
            var token = _sessionTempDataServices.GetToken();
            var client = new HttpClient();
            var selectedCustomer = _sessionTempDataServices.GetCurrentCustomer();

            var accounts = await APIHelper.GetAccountsAsync(_APISettingsConfig, client, token, selectedCustomer.CustomerNumber.PadLeft(8, '0'), selectedCustomer.Bank.ToString());

            return View(accounts);
        }

        public IActionResult ClearSelectedCustomer()
        {
            _sessionTempDataServices.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NoSuchAsCustomer(string customerNumber, string bank)
        {
            var selectedCustomer = new SelectedCustomer();
            selectedCustomer.Bank = Int32.Parse(bank);
            selectedCustomer.CustomerNumber = customerNumber;

            return View(selectedCustomer);
        }

        public async Task<IActionResult> ShowTransactions(string accountNumber)
        {
            var token = _sessionTempDataServices.GetToken();
            var client = new HttpClient();
            var selectedCustomer = _sessionTempDataServices.GetCurrentCustomer();

            var transactions = await APIHelper.GetTrasactionsAsync(_APISettingsConfig, client, token, selectedCustomer.CustomerNumber.PadLeft(8, '0'), selectedCustomer.Bank.ToString(), accountNumber);

            return View(transactions);
        }

        public async Task<IActionResult> ShowTransactionsChart(string accountNumber)
        {
            var token = _sessionTempDataServices.GetToken();
            var client = new HttpClient();
            var selectedCustomer = _sessionTempDataServices.GetCurrentCustomer();

            var transactions = await APIHelper.GetTrasactionsAsync(_APISettingsConfig, client, token, selectedCustomer.CustomerNumber.PadLeft(8, '0'), selectedCustomer.Bank.ToString(), accountNumber);

            var grouped = (from p in transactions
                           group p by new { month = p.bookingDate.Month, year = p.bookingDate.Year } into d
                           select new { dt = string.Format("{1}/{0}", d.Key.month, d.Key.year), sum = d.Sum(x=>x.amount) }).OrderBy(g => g.dt);

            var lables = "";
            var data = ""; //12, 19, 3, 5, 2, 3

            foreach (var group in grouped) {
                lables += "'" + group.dt + "',";
                data += group.sum + ",";
            };

            lables = lables.Remove(lables.Length - 1);
            data = data.Remove(data.Length - 1);

            ViewData["chartTransactionLabels"] = lables;
            ViewData["chartTransactionData"] = data;

            return View(transactions);
        }

        public async Task<IActionResult> ShowAccountInfo(string accountNumber)
        {
            var token = _sessionTempDataServices.GetToken();
            var client = new HttpClient();
            var selectedCustomer = _sessionTempDataServices.GetCurrentCustomer();

            var accountInfo = await APIHelper.GetAccountAsync(_APISettingsConfig, client, token, selectedCustomer.CustomerNumber.PadLeft(8, '0'), selectedCustomer.Bank.ToString(), accountNumber);
            return View(accountInfo);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
