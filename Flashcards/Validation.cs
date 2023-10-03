using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashcards
{
    internal class Validation
    {
        internal static string GetString(string message)
        {
            Console.WriteLine($"\n{message}");

            string str = Console.ReadLine();

            if (str == "")
            {
                Console.Clear();
                Console.WriteLine("Wrong input. You can't insert a blank space!");
                DatabaseManager.GetUserInput();
            }

            if (str == "0")
            {
                Console.Clear();
                DatabaseManager.GetUserInput();
            }

            return str;
        }
    }
}
