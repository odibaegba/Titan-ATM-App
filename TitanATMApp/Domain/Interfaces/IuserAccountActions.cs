using System;
using System.Collections.Generic;
using System.Text;

namespace TitanATMApp.Domain.Interfaces
{
    public interface IuserAccountActions
    {
        void CheckBalance();
        void PlaceDeposit();
        void MakeWithdrawal();
    }
}
