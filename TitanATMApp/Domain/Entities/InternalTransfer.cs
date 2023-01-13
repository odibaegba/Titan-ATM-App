using System;
using System.Collections.Generic;
using System.Text;

namespace TitanATMApp.Domain.Entities
{
    public class InternalTransfer
    {
        public decimal TransferAmount { get; set; }
        public long ReciepientBankAccountNumber { get; set; }
        public string RecipientBankAccountName { get; set; }

    }
}
