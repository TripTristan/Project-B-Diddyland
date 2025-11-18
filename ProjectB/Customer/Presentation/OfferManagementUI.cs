public static class OfferManagementUI
{
    private static OfferService _service = new OfferService();

    public static void Start()
    {
        while (true)
        {
            Console.Clear();
            List(false);

            Console.WriteLine();
            Console.WriteLine("=== Offer Management ===");
            Console.WriteLine("1) Refresh list");
            Console.WriteLine("2) Add offer");
            Console.WriteLine("3) Edit offer");
            Console.WriteLine("4) Activate/Deactivate");
            Console.WriteLine("5) Delete offer");
            Console.WriteLine("0) Back");
            Console.Write("Choose: ");
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1": break;
                case "2": Add(); break;
                case "3": Edit(); break;
                case "4": Toggle(); break;
                case "5": Delete(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid option. Press any key...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static void List(bool pause = true)
    {
        var list = _service.GetAllOffers();
        Console.WriteLine($"{"ID",-4}{"Name",-20}{"Discount",-10}{"Active",-8}{"CustomerOnly",-12}");
        
        foreach (var o in list)
        {
            var discount = (o.Discount * 100).ToString("F0") + "%";
            Console.WriteLine($"{o.Id,-4}{o.Name,-20}{discount,-10}{o.IsActive,-8}{o.TargetOnlyCustomers,-12}");
        }

        if (pause)
        {
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }
    }

    private static void Add()
    {
        var offer = InputOffer();
        var result = _service.AddOffer(offer);
        
        Console.WriteLine(result.Message);
        Console.ReadKey();
    }

    private static void Edit()
    {
        if (!InputInt("Id to edit: ", out var id))
        {
            Console.ReadKey();
            return;
        }

        var existing = _service.GetOfferById(id);
        if (existing == null)
        {
            Console.WriteLine("Offer not found.");
            Console.ReadKey();
            return;
        }

        var updatedOffer = InputOffer(existing);
        var result = _service.UpdateOffer(id, updatedOffer);
        
        Console.WriteLine(result.Message);
        Console.ReadKey();
    }

    private static void Toggle()
    {
        if (!InputInt("Id to toggle: ", out var id))
        {
            Console.ReadKey();
            return;
        }

        var result = _service.ToggleOfferStatus(id);
        Console.WriteLine(result.Message);
        Console.ReadKey();
    }

    private static void Delete()
    {
        if (!InputInt("Id to delete: ", out var id))
        {
            Console.ReadKey();
            return;
        }

        var offer = _service.GetOfferById(id);
        if (offer == null)
        {
            Console.WriteLine("Offer not found.");
            Console.ReadKey();
            return;
        }

        if (!InputYesNo($"Confirm delete offer '{offer.Name}'", false))
        {
            Console.WriteLine("Delete operation cancelled.");
            Console.ReadKey();
            return;
        }

        var result = _service.DeleteOffer(id);
        Console.WriteLine(result.Message);
        Console.ReadKey();
    }

    private static OfferModel InputOffer(OfferModel existing = null)
    {
        bool isEdit = existing != null;
        var offer = existing ?? new OfferModel();

        // Name
        if (isEdit)
        {
            Console.Write($"Name ({offer.Name}): ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) offer.Name = name;
        }
        else
        {
            offer.Name = InputRequired("Name: ");
        }

        // Description
        Console.Write($"Description{(isEdit ? $" ({offer.Description})" : "")}: ");
        var desc = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(desc) || !isEdit)
            offer.Description = desc ?? "";

        // Discount
        while (true)
        {
            Console.Write($"Discount %{(isEdit ? $" ({offer.Discount * 100})" : "")}: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) && isEdit) break;
            
            if (decimal.TryParse(input, out var discount) && discount >= 0 && discount <= 100)
            {
                offer.Discount = discount / 100m;
                break;
            }
            Console.WriteLine("Invalid input. Enter a number between 0 and 100.");
        }

        // StartDate
        while (true)
        {
            Console.Write($"StartDate yyyy-MM-dd{(isEdit ? $" ({offer.StartDate:yyyy-MM-dd})" : "")}: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) && isEdit) break;
            
            if (DateTime.TryParse(input, out var date))
            {
                offer.StartDate = date;
                break;
            }
            Console.WriteLine("Invalid date format. Use yyyy-MM-dd.");
        }

        // EndDate
        while (true)
        {
            Console.Write($"EndDate yyyy-MM-dd{(isEdit ? $" ({offer.EndDate:yyyy-MM-dd})" : "")}: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) && isEdit) break;
            
            if (DateTime.TryParse(input, out var date))
            {
                offer.EndDate = date;
                break;
            }
            Console.WriteLine("Invalid date format. Use yyyy-MM-dd.");
        }

        // CustomerOnly
        offer.TargetOnlyCustomers = isEdit ?
            InputYesNo("CustomerOnly", offer.TargetOnlyCustomers) :
            InputYesNo("CustomerOnly");

        // Min quantity
        while (true)
        {
            Console.Write($"Min quantity{(isEdit && offer.Rules?.Any() == true ? $" ({offer.Rules.First().RuleValue})" : "")}: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) && isEdit) break;
            
            if (int.TryParse(input, out var quantity) && quantity >= 0)
            {
                offer.Rules = new List<OfferRuleModel> { new() { RuleType = "Quantity", RuleValue = quantity } };
                break;
            }
            Console.WriteLine("Invalid input. Enter a non-negative integer.");
        }

        // Activate now
        offer.IsActive = isEdit ?
            InputYesNo("Activate now", offer.IsActive) :
            InputYesNo("Activate now");

        return offer;
    }


    private static string InputRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(input))
                return input;
            Console.WriteLine("This field is required.");
        }
    }

    private static bool InputYesNo(string prompt, bool currentValue)
    {
        while (true)
        {
            Console.Write($"{prompt} y/n (current: {(currentValue ? 'y' : 'n')}): ");
            var input = Console.ReadLine()?.Trim().ToLower();
            if (input == "y") return true;
            if (input == "n") return false;
            if (string.IsNullOrWhiteSpace(input)) return currentValue;
            Console.WriteLine("Enter 'y' or 'n'.");
        }
    }

    private static bool InputYesNo(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} y/n: ");
            var input = Console.ReadLine()?.Trim().ToLower();
            if (input == "y") return true;
            if (input == "n") return false;
            Console.WriteLine("Enter 'y' or 'n'.");
        }
    }

    private static bool InputInt(string prompt, out int value)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out value))
                return true;
            
            Console.WriteLine("Invalid number format.");
            return false;
        }
    }
}