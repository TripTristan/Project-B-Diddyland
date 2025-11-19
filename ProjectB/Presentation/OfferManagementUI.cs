using System;
using System.Collections.Generic;
using System.Linq;
using MyProject.BLL;

namespace MyProject.UI
{
    public static class OfferManagementUI
    {
        public static void Start()
        {
            while (true)
            {
                Console.Clear();
                ListAllOffers();

                Console.WriteLine("\n=== Offer Management ===");
                Console.WriteLine("1) Refresh list");
                Console.WriteLine("2) Add offer");
                Console.WriteLine("3) Edit offer");
                Console.WriteLine("4) Activate/Deactivate");
                Console.WriteLine("5) Delete offer");
                Console.WriteLine("0) Back");
                Console.Write("\nChoose: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": break;
                    case "2": AddOffer(); break;
                    case "3": EditOffer(); break;
                    case "4": ToggleOffer(); break;
                    case "5": DeleteOffer(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Invalid option. Press any key...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void ListAllOffers()
        {
            var offers = OfferManagementLogic.GetAllOffers();
            Console.WriteLine($"{"ID",-4}{"Name",-20}{"Discount",-10}{"Active",-8}{"Type",-12}");
            foreach (var o in offers)
            {
                var pct = (o.Discount * 100).ToString("F0") + "%";
                Console.WriteLine($"{o.Id,-4}{o.Name,-20}{pct,-10}{o.IsActive,-8}{o.Type,-12}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void AddOffer()
        {
            var dto = InputOffer();
            var res = OfferManagementLogic.AddOffer(dto);
            Console.WriteLine($"\n{res.Message}");
            Console.ReadKey();
        }

        private static void EditOffer()
        {
            if (!int.TryParse(InputRequired("Id to edit: "), out int id)) return;
            var existing = OfferManagementLogic.GetOfferById(id);
            if (existing == null) { Console.WriteLine("Not found."); Console.ReadKey(); return; }

            var updated = InputOffer(existing);
            var res = OfferManagementLogic.UpdateOffer(id, updated);
            Console.WriteLine($"\n{res.Message}");
            Console.ReadKey();
        }

        private static void ToggleOffer()
        {
            if (!int.TryParse(InputRequired("Id to toggle: "), out int id)) return;
            var res = OfferManagementLogic.ToggleOfferStatus(id);
            Console.WriteLine($"\n{res.Message}");
            Console.ReadKey();
        }

        private static void DeleteOffer()
        {
            if (!int.TryParse(InputRequired("Id to delete: "), out int id)) return;
            var existing = OfferManagementLogic.GetOfferById(id);
            if (existing == null) { Console.WriteLine("Not found."); Console.ReadKey(); return; }
            if (!Confirm($"Delete '{existing.Name}'?")) return;

            var res = OfferManagementLogic.DeleteOffer(id);
            Console.WriteLine($"\n{res.Message}");
            Console.ReadKey();
        }

        private static string InputRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(s)) return s;
                Console.WriteLine("Required field.");
            }
        }

        private static OfferDto InputOffer(OfferDto? current = null)
        {
            bool edit = current != null;
            var dto = current ?? new OfferDto();

            dto.Name = edit ? InputWithDefault("Name: ", current.Name) : InputRequired("Name: ");
            dto.Description = InputWithDefault("Description: ", current.Description ?? "");

            // Discount %
            while (true)
            {
                var raw = edit ? InputWithDefault("Discount %: ", (current.Discount * 100).ToString())
                               : InputRequired("Discount %: ");
                if (decimal.TryParse(raw, out var pct) && pct is >= 0 and <= 100)
                { dto.Discount = pct / 100m; break; }
                Console.WriteLine("0-100");
            }

            dto.StartDate = InputDate("Start date (yyyy-MM-dd): ", current?.StartDate);
            dto.EndDate   = InputDate("End date (yyyy-MM-dd): ", current?.EndDate);

            dto.Type = InputEnum<OfferType>("Type (0=Age,1=Quantity,2=PromoCode,3=Group): ", current?.Type ?? OfferType.Age);
            if (dto.Type == OfferType.Group)
                dto.GroupType = InputEnum<GroupType>("Group type (0=None,1=School,2=Company): ", current?.GroupType ?? GroupType.None);
            else
                dto.GroupType = null;

            dto.Min = int.Parse(InputWithDefault("Min value: ", (current?.Min ?? 0).ToString()));
            dto.Max = int.Parse(InputWithDefault("Max value: ", (current?.Max ?? 0).ToString()));
            dto.IsActive = Confirm("Activate now?", current?.IsActive ?? false);

            return dto;
        }

        private static string InputWithDefault(string prompt, string current)
        {
            Console.Write($"{prompt}(current: {current}): ");
            var s = Console.ReadLine()?.Trim();
            return string.IsNullOrWhiteSpace(s) ? current : s;
        }

        private static DateTime InputDate(string prompt, DateTime? current)
        {
            while (true)
            {
                var raw = InputWithDefault(prompt, current?.ToString("yyyy-MM-dd") ?? "");
                if (DateTime.TryParse(raw, out var d)) return d;
                Console.WriteLine("yyyy-MM-dd");
            }
        }

        private static T InputEnum<T>(string prompt, T current) where T : struct, Enum
        {
            var values = Enum.GetValues<T>();
            while (true)
            {
                Console.WriteLine($"{prompt}");
                for (int i = 0; i < values.Length; i++)
                    Console.WriteLine($"{i}. {values[i]}");
                var raw = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(raw)) return current;
                if (int.TryParse(raw, out int idx) && idx >= 0 && idx < values.Length)
                    return values[idx];
                Console.WriteLine("Invalid selection.");
            }
        }

        private static bool Confirm(string message, bool current = false)
        {
            Console.Write($"{message} (y/n): ");
            var input = Console.ReadLine()?.Trim().ToLower();
            return input == "y" ? !current : current;
        }
    }
}