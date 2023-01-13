using System;
using System.Collections.Generic;
using System.Text;
using TitanATMApp.Domain.Enum;

namespace TitanATMApp.Domain.Entities
{
    public class Transaction
    {
        public long TransactionId { get; set; }
        public long UserBankAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}
