using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackYeah2018.APIHelperClass
{
    public class Bank
    {
        public string bicOrSwift { get; set; }
        public string name { get; set; }

        public string code { get; set; }
        public string countryCode { get; set; }
        public string[] address { get; set; }
    }
}
