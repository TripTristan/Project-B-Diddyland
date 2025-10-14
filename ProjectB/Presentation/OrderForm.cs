using System.Text;

public static class OrderForm
{
    public static void Run(OrderLogic logic)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(MenuForm.FormatMenu(logic.GetAllMenuItems()));
            Console.WriteLine("Choose an order action:");
            Console.WriteLine("[1] Add item to cart");
            Console.WriteLine("[2] View cart");
            Console.WriteLine("[3] Remove item from cart");
            Console.WriteLine("[4] Finalize order");
            Console.WriteLine("[0] Back");
            Console.Write("Your choice: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddItemUI(logic);
                    break;
                case "2":
                    ViewCartUI(logic);
                    break;
                case "3":
                    RemoveItemUI(logic);
                    break;
                case "4":
                    FinalizeUI(logic);
                    break;
                case "0":
                    return;
                default:
                    Pause("Unknown option. Press any key...");
                    break;
            }
        }
    }

    private static void AddItemUI(OrderLogic logic)
    {
        Console.Clear();
        Console.WriteLine("=== Add item to cart ===");
        var id = PromptInt("Enter Menu ID: ");
        var qty = PromptInt("Quantity: ");
        var result = logic.AddToCart(id, qty);
        Pause(result + " Press any key...");
    }

    private static void RemoveItemUI(OrderLogic logic)
    {
        Console.Clear();
        Console.WriteLine("=== Remove item from cart ===");
        if (logic.IsCartEmpty())
        {
            Pause("Cart is empty. Press any key...");
            return;
        }

        PrintCart(logic);
        var id = PromptInt("Enter Menu ID to remove: ");
        var result = logic.RemoveFromCart(id);
        Pause(result + " Press any key...");
    }

    private static void ViewCartUI(OrderLogic logic)
    {
        Console.Clear();
        Console.WriteLine("=== Cart ===");
        if (logic.IsCartEmpty())
        {
            Pause("Your cart is empty. Press any key...");
            return;
        }

        PrintCart(logic);
        Console.WriteLine();
        Console.WriteLine("[1] Change quantity");
        Console.WriteLine("[0] Back");
        Console.Write("Choice: ");
        var ch = Console.ReadLine();
        if (ch == "1")
        {
            var id = PromptInt("Menu ID to update: ");
            var qty = PromptInt("New quantity: ");
            var msg = logic.UpdateQuantity(id, qty);
            Pause(msg + " Press any key...");
        }
    }

    private static void FinalizeUI(OrderLogic logic)
    {
        Console.Clear();
        Console.WriteLine("=== Finalize Order ===");
        if (logic.IsCartEmpty())
        {
            Pause("Your cart is empty. Add items before finalizing. Press any key...");
            return;
        }

        PrintCart(logic);
        Console.WriteLine();
        Console.Write("Confirm order? (Y/N): ");
        var confirm = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
        if (confirm != "Y")
        {
            Pause("Order not finalized. Press any key...");
            return;
        }

        var summary = logic.FinalizeOrder();
        Console.Clear();
        Console.WriteLine("=== Order Confirmed ===");
        Console.WriteLine($"Time: {summary.Timestamp:G}");
        Console.WriteLine();

        var sb = new StringBuilder();
        sb.AppendLine("Items:");
        foreach (var line in summary.Lines)
            sb.AppendLine($"- {line.Quantity} × {line.Label} @ €{line.Price:0.00} = €{line.Subtotal:0.00}");
        sb.AppendLine("---------------------------------");
        sb.AppendLine($"TOTAL: €{summary.Total:0.00}");
        Console.WriteLine(sb.ToString());
        Pause("Thank you! Press any key...");
    }

    private static void PrintCart(OrderLogic logic)
    {
        var cart = logic.GetCart();
        var lines = cart.Select(c =>
        {
            var label = CartLine.BuildLabel(c.Item);
            return new
            {
                c.Item.ID,
                Label = label,
                Unit = c.Item.Price,
                c.Quantity,
                Subtotal = c.LineTotal
            };
        }).ToList();

        var sb = new StringBuilder();
        sb.AppendLine("ID   Item                           Qty   Unit      Subtotal");
        sb.AppendLine("---- ------------------------------ ---- --------- ---------");
        foreach (var l in lines)
            sb.AppendLine($"{l.ID,-4} {TrimPad(l.Label, 30),-30} {l.Quantity,4} €{l.Unit,7:0.00} €{l.Subtotal,7:0.00}");
        sb.AppendLine("------------------------------------------------------------");
        sb.AppendLine($"TOTAL: €{logic.GetTotal():0.00}");
        Console.WriteLine(sb.ToString());
    }

    private static string TrimPad(string s, int len)
    {
        if (s.Length == len) return s;
        if (s.Length < len) return s.PadRight(len);
        return s.Substring(0, len - 1) + "…";
    }

    private static int PromptInt(string label)
    {
        while (true)
        {
            Console.Write(label);
            var input = (Console.ReadLine() ?? "").Trim();
            if (int.TryParse(input, out var value) && value >= 0)
                return value;
            Console.WriteLine("Please enter a valid non-negative integer.");
        }
    }

    private static void Pause(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey(true);
    }
}
