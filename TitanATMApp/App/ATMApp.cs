using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TitanATMApp.Domain.Entities;
using TitanATMApp.Domain.Enum;
using TitanATMApp.Domain.Interfaces;
using TitanATMApp.UserInterface;

namespace TitanATMApp.App
{
    public class ATMApp : IuserLogin, IuserAccountActions, Itransaction
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal minimuKeptAmount = 500;
        private readonly AppScreen screen;

        public ATMApp()
        {
            screen = new AppScreen();
        }

        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            while(true)
            {
                AppScreen.DisplayAppMenue();
                ProcessMenueOption();
            }
           
        }

        public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
               new UserAccount{Id=1, FullName = "Steve Odiba", Accountnumber = 123456, CardNumber = 321321, CardPin = 123123, AccountBalance = 5000.00m, IsLocked = false},
               new UserAccount{Id=2, FullName = "Mark Adakole", Accountnumber = 456789, CardNumber = 654654, CardPin = 456456, AccountBalance = 4000.00m, IsLocked = false},
               new UserAccount{Id=3, FullName = "Fred Amadeh", Accountnumber = 123555, CardNumber = 987987, CardPin = 789789, AccountBalance = 2000.00m, IsLocked = true}
            };

            _listOfTransactions = new List<Transaction>();

        }

        public void CheckUserCardNumAndPassword()
        {
            bool isCorrectLogin = false;

            while (isCorrectLogin == false)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();

                AppScreen.LoginProgress();
                foreach (UserAccount account in userAccountList)
                {
                    selectedAccount = account;
                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogin++;

                        if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                        {
                            selectedAccount = account;

                            if (selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                            {
                                //print a lock message
                                AppScreen.PrintLockscreen();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }

                    }
                    if (isCorrectLogin == false)
                    {
                        Utility.PrintMessage("\nInvalid card PIN.", false);
                        selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                        if (selectedAccount.IsLocked)
                        {
                            AppScreen.PrintLockscreen();
                        }
                    }
                    Console.Clear();



                }
            }
            

        }

        private void ProcessMenueOption()
        {
            switch (Validator.Convert<int>("an opton:"))
            {
                case (int) AppMenu.CheckBalance:
                    CheckBalance();
                    break;

                case (int)AppMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;

                case (int)AppMenu.MakeWithdrawal:
                   MakeWithdrawal();
                    break;

                case (int)AppMenu.InternalTransfer:
                    var internalTransfer = screen.InternalTransferForm();
                    ProcessInternalTrasfer(internalTransfer);
                    break;

                case (int)AppMenu.ViewTransaction:
                    ViewTransaction();
                    break;

                case (int)AppMenu.Logout:
                    AppScreen.LogoutProgress();
                    Utility.PrintMessage("You have successfully logged out. Please collect you ATM card.", true);
                    Run();
                    break;

                default:
                    Utility.PrintMessage("Invalid Option.", false);
                    break;
            }
        }

        public void CheckBalance()
        {
            Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}", true);
        }

        public void PlaceDeposit()
        {
            Console.WriteLine("\nOnly multiples of 50 and 100 dollar allowed.\n");
            var transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");

            //simulate counting
            Console.WriteLine("\nChecking and Counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            //some guard clause
            if(transaction_amt <= 0)
            {
                Utility.PrintMessage("amount needs to be greater than Zero", false);
                return;
            }
            if(transaction_amt % 50 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount in multiple of 50 or 100. try again", false);
                return;
            }
            
            if(PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
                return;
            }

            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt, "");

            //update account balance
            selectedAccount.AccountBalance += transaction_amt;

            //print success message to the screen
            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was successful.", true);

        }

        public void MakeWithdrawal()
        {
            var transaction_amt = 0;
            int selectedAmount = AppScreen.SelectAmount();
            if(selectedAmount == -1)
            {
                MakeWithdrawal();
                return;
            }
            else if(selectedAmount != 0)
            {
                transaction_amt = selectedAmount;
            }
            else
            {
                transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
            }

            //input validation
            if(transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than zero. Please try again.", false);
                return;
            }
            if(transaction_amt % 50 != 0)
            {
                Utility.PrintMessage("you can only withdraw amount in multiples of 50 or 100 dollar. Try again. ", false);
                return;
            }
            //Business logic validation
            if(transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account balance is too low to withdraw {Utility.FormatAmount(transaction_amt)}", false);
                return;
            }

            if((selectedAccount.AccountBalance - transaction_amt) < minimuKeptAmount)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have minimu {Utility.FormatAmount(minimuKeptAmount)}", false);
                return;
            }
            //Bind withdrawal details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal, -transaction_amt, "");
            //update account balance
            selectedAccount.AccountBalance -= transaction_amt;
            //success message
            Utility.PrintMessage($"You have successfully withdrawn" + $"{Utility.FormatAmount(transaction_amt)}.", true);
           
        }

        private bool PreviewBankNotesCount(int amount)
        {
            int hundredNotesCount = amount / 100;
            int fiftyNotesCount = (amount % 100) / 50;

            Console.WriteLine("\nSummary");
            Console.WriteLine("- - - - - -");
            Console.WriteLine($"{AppScreen.cur}100 x {hundredNotesCount} = {100 * hundredNotesCount}");
            Console.WriteLine($"{AppScreen.cur}50 x {fiftyNotesCount} = {50 * fiftyNotesCount}");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);
        }

        public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Description = _desc
            };

            //add transaction object to the list
            _listOfTransactions.Add(transaction);
        }
        

        public void ViewTransaction()
        {
           var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();
            //check if there is a transaction
            if(filteredTransactionList.Count <= 0)
            {
                Utility.PrintMessage("You have no transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Descriptions", "Amount" + AppScreen.cur);
                foreach (var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);
            }
        }

        private void ProcessInternalTrasfer(InternalTransfer internalTransfer)
        {
            if(internalTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }
            //check sender's account balance
            if(internalTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Transfer failed. You do not have sufficient balance to transfer {Utility.FormatAmount(internalTransfer.TransferAmount)}", false);
                return;
            }
            //check the minimum kept amount
            if((selectedAccount.AccountBalance - internalTransfer.TransferAmount) < minimuKeptAmount)
            {
                Utility.PrintMessage($"Transfered failed. Your account needs to have a minimum {Utility.FormatAmount(minimuKeptAmount)}", false);
                return;
            }
            //check reciever's account number is valid
            var selectedBankAccountReciever = (from userAcc in userAccountList 
                                               where userAcc.Accountnumber == internalTransfer.ReciepientBankAccountNumber 
                                               select userAcc).FirstOrDefault();
            if(selectedBankAccountReciever == null)
            {
                Utility.PrintMessage("Transfer failed. Reciever bank account number is invalid.", false);
                return;
            }
            //check reciever's name
            if(selectedBankAccountReciever.FullName != internalTransfer.RecipientBankAccountName)
            {
                Utility.PrintMessage("Transfer Failed. Recipient's bank account name does not match.", false);
                return;
            }

            //add transaction record- sender
            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, -internalTransfer.TransferAmount, $"Transfer to {selectedBankAccountReciever.Accountnumber} ({selectedBankAccountReciever.FullName})");
            //update sender's account balance
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            //add transaction record-reciever
            InsertTransaction(selectedBankAccountReciever.Id, TransactionType.Transfer, internalTransfer.TransferAmount, $"Transfered from {selectedAccount.Accountnumber} ({selectedAccount.FullName})");

            //update reciever's account balance
            selectedBankAccountReciever.AccountBalance += internalTransfer.TransferAmount;
            //print success message
            Utility.PrintMessage($"You have successfully transfered" + $"{Utility.FormatAmount(internalTransfer.TransferAmount)} to {internalTransfer.RecipientBankAccountName}", true); 
            return;

        }
    }
}