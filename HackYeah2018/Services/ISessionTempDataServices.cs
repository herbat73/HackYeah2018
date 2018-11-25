using HackYeah2018.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackYeah2018.Services
{
    public interface ISessionTempDataServices
    {
        void SetCurrentCustomer(SelectedCustomer customer);
        void Clear();
        SelectedCustomer GetCurrentCustomer();

        string GetToken();

        void SetToken(string token);
    }
}
