using System.Collections.Generic;
using System.IO;

namespace HackYeah2018.APIHelperClass
{
    public static class ExternalDBHelper
    {
        public static List<ClientAccount> LoadExternalData(string fullPath)
        {
            var ret = new List<ClientAccount>();
            var skipLine = true;

            using (var reader = new StreamReader(fullPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!skipLine)
                    {
                        var values = line.Split(';');
                        var clientAccount = new ClientAccount();
                        clientAccount.BankId = values[0];
                        clientAccount.ClientId = values[1].PadLeft(8, '0'); ;
                        clientAccount.AccountNumber = values[2];
                        ret.Add(clientAccount);
                    }
                    skipLine = false;
                }
            }

            return ret;
        }
    }
}
