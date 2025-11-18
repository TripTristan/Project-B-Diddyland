using System;
using ProjectB.Admin.Logic;
using ProjectB.Customer.DataAccess;

namespace ProjectB.Admin.Presentation
{
    public class AdminLoginUI
    {
        private readonly AdminLoginLogic _loginLogic;

        public AdminLoginUI()
        {
            _loginLogic = new AdminLoginLogic(new UserAccess());
        }

        public void Show()
        {
            Console.Clear();
            Console.WriteLine("=== Admin Login ===");
            
            while (true)
            {
                Console.Write("\nUsername: ");
                string? username = Console.ReadLine();
                
                Console.Write("Password: ");
                string? password = ReadPassword();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Username and password cannot be empty!");
                    continue;
                }

                var result = _loginLogic.Login(username, password);
                if (result.Success)
                {
                    Console.WriteLine("\nLogin successful!");
                    Thread.Sleep(1000);
                    new AdminMenuUI().Show();
                    break;
                }
                else
                {
                    Console.WriteLine($"\nLogin failed: {result.Message}");
                    Console.WriteLine("Press any key to try again, or ESC to exit...");
                    
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        break;
                }
            }
        }

        private static string? ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;
            
            do
            {
                key = Console.ReadKey(true);
                
                if (key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            while (key.Key != ConsoleKey.Enter);
            
            Console.WriteLine();
            return password;
        }
    }
}
