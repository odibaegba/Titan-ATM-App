using System;
using System.Collections.Generic;
using System.Text;
using TitanATMApp.Domain.Enum;

namespace TitanATMApp.Domain.Interfaces
{
    public interface Itransaction
    {
        void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc);
        void ViewTransaction();

    }
}
