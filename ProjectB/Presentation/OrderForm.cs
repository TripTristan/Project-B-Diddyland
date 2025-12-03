using System.Text;

public class OrderForm
{
    private readonly OrderLogic _orderLogic;
    private readonly MenuForm _menuForm;

    public OrderForm(OrderLogic orderLogic, MenuForm menuForm)
    {
        _orderLogic = orderLogic;
        _menuForm = menuForm;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(_menuForm.FormatMenu(_orderLogic.GetAllMenuItems()));
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
                    AddItemUI();
                    break;
                case "2":
                    ViewCartUI();
                    break;
                case "3":
                    RemoveItemUI();
                    break;
                case "4":
                    FinalizeUI();
                    break;
                case "0":
                    return;
                default:
                    Pause("Unknown option. Press any key...");
                    break;
            }
        }
    }

    private void AddItemUI()
    {
        Console.Clear();
        Console.WriteLine("=== Add item to cart ===");
        var id = PromptInt("Enter Menu ID: ");
        var qty = PromptInt("Quantity: ");

        var result = _orderLogic.AddToCart(id, qty);
        Pause(result + " Press any key...");
    }

    private void RemoveItemUI()
    {
        Console.Clear();
        Console.WriteLine("=== Remove item from cart ===");

        if (_orderLogic.IsCartEmpty())
        {
            Pause("Cart is empty. Press any key...");
            return;
        }

        PrintCart();
        var id = PromptInt("Enter Menu ID to remove: ");
        var result = _orderLogic.RemoveFromCart(id);

        Pause(result + " Press any key...");
    }

    private void ViewCartUI()
    {
        Console.Clear();
        Console.WriteLine("=== Cart ===");

        if (_orderLogic.IsCartEmpty())
        {
            Pause("Your cart is empty. Press any key...");
            return;
        }

        PrintCart();
        Console.WriteLine();
        Console.WriteLine("[1] Change quantity");
        Console.WriteLine("[0] Back");
        Console.Write("Choice: ");

        var ch = Console.ReadLine();
        if (ch == "1")
        {
            var id = PromptInt("Menu ID to update: ");
            var qty = PromptInt("New quantity: ");

            var msg = _orderLogic.UpdateQuantity(id, qty);
            Pause(msg + " Press any key...");
        }
    }

    private void FinalizeUI()
    {
        Console.Clear();
        Console.WriteLine("=== Finalize Order ===");

        if (_orderLogic.IsCartEmpty())
        {
            Pause("Your cart is empty. Add items before finalizing. Press any key...");
            return;
        }

        PrintCart();
        Console.WriteLine();
        Console.Write("Confirm order? (Y/N): ");

        var confirm = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
        if (confirm != "Y")
        {
            Pause("Order not finalized. Press any key...");
            return;
        }

        var summary = _orderLogic.FinalizeOrder();

        Console.Clear();
        Console.WriteLine("=== Order Confirmed ===");
        Console.WriteLine($"Time: {summary.Timestamp:G}");
        Console.WriteLine($"Pickup Time: {summary.PickupTime:G}");
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

    private void PrintCart()
    {
        var cart = _orderLogic.GetCart();

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
        sb.AppendLine($"TOTAL: €{_orderLogic.GetTotal():0.00}");

        Console.WriteLine(sb.ToString());
    }

    private string TrimPad(string s, int len)
    {
        if (s.Length == len) return s;
        if (s.Length < len) return s.PadRight(len);
        return s.Substring(0, len - 1) + "…";
    }

    private int PromptInt(string label)
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

    private void Pause(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey(true);
    }
}
