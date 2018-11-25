using System;
using HackYeah2018.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HackYeah2018.Services
{
    public class SessionTempDataServices : ISessionTempDataServices
    {
        private readonly ITempDataDictionary _tempData;

        public SessionTempDataServices(IHttpContextAccessor httpContextAccessor,
                                        ITempDataDictionaryFactory tempDataFactory)
        {
            _tempData = tempDataFactory.GetTempData(httpContextAccessor.HttpContext);
        }

        public void Clear()
        {
            _tempData.Clear();
        }

        public string GetToken()
        {
            string token = null;

            if (_tempData.Peek("Token") != null)
                token = _tempData.Peek("Token").ToString();

            return token;
        }

        public void SetToken(string token)
        {
            _tempData["Token"] = token;
        }

        public SelectedCustomer GetCurrentCustomer()
        {
            var customer = new SelectedCustomer();

            if (_tempData.Peek("SelectedBank") != null)
                customer.Bank = Convert.ToInt32(_tempData.Peek("SelectedBank"));

            if (_tempData.Peek("SelectedCustomerNumber") != null)
                customer.CustomerNumber = _tempData.Peek("SelectedCustomerNumber").ToString();

            return customer;
        }

        public void SetCurrentCustomer(SelectedCustomer customer)
        {
            _tempData["SelectedBank"] = customer.Bank;
            _tempData["SelectedCustomerNumber"] = customer.CustomerNumber;
        }
    }
}
