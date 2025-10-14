public class Program
{
    public static void Main()
    {
        var menusAccess = new MenusAccess();
        var menuLogic = new MenuLogic(menusAccess);
        var orderLogic = new OrderLogic(menuLogic);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== THEME PARK ===");
            Console.WriteLine("[1] Order (customer)");
            Console.WriteLine("[2] Manage Menu (admin)");
            Console.WriteLine("[0] Exit");
            Console.Write("Your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    OrderForm.Run(orderLogic);
                    break;
                case "2":
                    MenuForm.Run(menuLogic);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Unknown option. Press any key...");
                    Console.ReadKey(true);
                    break;
            }
        }
    }
}
