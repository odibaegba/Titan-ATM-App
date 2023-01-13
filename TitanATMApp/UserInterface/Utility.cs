using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace TitanATMApp.UserInterface
{
    public static class Utility
    {
        private static long tranId;
        private static CultureInfo culture = new CultureInfo("en-US");

        public static long GetTransactionId()
        {
            return ++tranId;
        }
        public static string GetSecretInput(string prompt)
        {
            bool isprompt = true;
            string asterics = "";

            StringBuilder input = new StringBuilder();

            while(true)
            {
                if(isprompt)
                    Console.WriteLine(prompt);
                isprompt = false;


                ConsoleKeyInfo inputKey = Console.ReadKey(true);

                if(inputKey.Key == ConsoleKey.Enter)
                {
                    if(input.Length == 6)
                    {
                        break;
                    }
                    else
                    {
                        PrintMessage("\nPlease enter 6 digits.", false);
                        input.Clear();
                        isprompt = true;
                        continue;
                        
                    }
                }
                if(inputKey.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                }
                else if(inputKey.Key != ConsoleKey.Backspace)
                {
                    input.Append(inputKey.KeyChar);
                    Console.Write(asterics + "*");
                }
            }
            return input.ToString();
        }

        public static void PrintMessage(string msg, bool success)
        {
            if(success)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Green;
            PressEnterToContinue();
        }
        public static string GetUserInput(string prompt)
        {
            Console.WriteLine($"Enter {prompt}");
            return Console.ReadLine();
        }
        public static void PrintDotAnimation(int timer = 10)
        {
          
            for (int i = 0; i < timer; i++)
            {
                Console.Write(".");
                Thread.Sleep(200);
            }
            Console.Clear();
        }
        public static void PressEnterToContinue()
        {
            Console.WriteLine("\n\nPress Enter to continue...\n");
            Console.ReadLine();
        }

        public static string FormatAmount(decimal amt)
        {
            return string.Format(culture, "{0:C2}", amt);
        }
    }
}
