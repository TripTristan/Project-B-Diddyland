public static class OfferManagementUI
{
    private static OfferAccess _repo = new();

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
            Console.WriteLine("0) Back");
            Console.Write("Choose: ");
            var c = Console.ReadLine();
            switch (c)
            {
                case "1":             break;
                case "2": Add();      break;
                case "3": Edit();     break;
                case "4": Toggle();   break;
                case "0": return;
            }
        }
    }

    private static void List(bool pause = true)
    {
        var list = _repo.GetAll().ToList();
        Console.WriteLine($"{"ID",-4}{"Name",-20}{"Discount",-10}{"Active",-8}{"CustomerOnly",-12}");
        foreach (var o in list)
            Console.WriteLine($"{o.Id,-4}{o.Name,-20}{o.Discount*100+"%",-10}{o.IsActive,-8}{o.TargetOnlyCustomers,-12}");

        if (pause) { Console.WriteLine("\nPress any key..."); Console.ReadKey(); }
    }

    private static void Add()
    {
        var o = InputOffer();
        _repo.Insert(o);
        Console.WriteLine("Added.");
        Console.ReadKey();
    }

    private static void Edit()
    {
        Console.Write("Id to edit: ");
        var id = int.Parse(Console.ReadLine()!);
        var o = _repo.GetById(id);
        if (o == null) { Console.WriteLine("Not found"); Console.ReadKey(); return; }
        var n = InputOffer(o);
        n.Id = id;
        _repo.Update(n);
        Console.WriteLine("Updated.");
        Console.ReadKey();
    }

    private static void Toggle()
    {
        Console.Write("Id to toggle: ");
        var id = int.Parse(Console.ReadLine()!);
        var o = _repo.GetById(id);
        if (o == null) { Console.WriteLine("Not found"); Console.ReadKey(); return; }
        _repo.SetActive(id, !o.IsActive);
        Console.WriteLine("Toggled.");
        Console.ReadKey();
    }

    // input offer of quantity rule only
    private static OfferModel InputOffer(OfferModel? existing = null)
    {
        var o = existing ?? new OfferModel();
        Console.Write($"Name ({o.Name}): "); var name = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(name)) o.Name = name;
        Console.Write($"Description ({o.Description}): "); var desc = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(desc)) o.Description = desc;
        Console.Write($"Discount % ({o.Discount * 100}): "); var d = Console.ReadLine(); if (decimal.TryParse(d, out var dd)) o.Discount = dd / 100m;
        Console.Write($"StartDate yyyy-MM-dd ({o.StartDate:yyyy-MM-dd}): "); var s = Console.ReadLine(); if (DateTime.TryParse(s, out var sd)) o.StartDate = sd;
        Console.Write($"EndDate yyyy-MM-dd ({o.EndDate:yyyy-MM-dd}): "); var e = Console.ReadLine(); if (DateTime.TryParse(e, out var ed)) o.EndDate = ed;
        Console.Write($"CustomerOnly y/n ({"yn"[o.TargetOnlyCustomers ? 1 : 0]}): "); var c = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(c)) o.TargetOnlyCustomers = c == "y";
        Console.Write("Min quantity in same order: "); var q = Console.ReadLine(); if (int.TryParse(q, out var qq)) o.Rules = new List<OfferRuleModel> { new() { RuleType = "Quantity", RuleValue = qq } };
        return o;
    }
    


}