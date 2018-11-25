using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackYeah2018.APIHelperClass
{
    public class Transaction
    {
        public int itemId { get; set; }
        public decimal amount { get; set; }

        public string currency_pair { get; set; }
        public string description { get; set; }
        public DateTime tradeDate { get; set; }
        public string transactionCategory { get; set; }
        //public decimal transactionStatus { get; set; }
        public Account sender { get; set; }
        public Account recipient { get; set; }

        public DateTime bookingDate { get; set; }

        public decimal postTransactionBalance { get; set; }
    }
}
