using System;
using System.Collections.Generic;
using System.Text;
using TitanATMApp.UserInterface;

namespace TitanATMApp.App
{
    public class Entry
    {
        static void Main(string[] args)
        {
            
            ATMApp atmApp = new ATMApp();
            atmApp.InitializeData();
            atmApp.Run();
            

           // Utility.PressEnterToContinue();
        }
    }
}
