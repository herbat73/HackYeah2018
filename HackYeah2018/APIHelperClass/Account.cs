namespace HackYeah2018.APIHelperClass
{
    public class Account
    {
        public string accountNumber { get; set; }

        public NameAddress nameAddress { get; set; }

        public string accountTypeName { get; set; }

        public string accountHolderType { get; set; }

        public string accountNameClient { get; set; }

        public string currency_pair { get; set; }

        public decimal bookingBalance { get; set; }

        public decimal availableBalance { get; set; }

        public AccountType accountType { get; set; }

        public Bank bank { get; set; }
    }
}
