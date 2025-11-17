public class GroupReservationUI
{
    private readonly GroupReservationService _service;

    public GroupReservationUI(GroupReservationService service)
    {
        _service = service;
    }

    public void ShowReservationFlow()
    {
        Console.Clear();
        Console.WriteLine("=== Group booking system ===\n");


        var groupType = SelectGroupType();
        if (groupType == GroupType.None) return;

        var (orgName, contactPerson, contactEmail, contactPhone, groupSize) = GetGroupInformation();

        var validation = _service.ValidateGroupSize(groupSize);
        if (!validation.IsValid)
        {
            Console.WriteLine($"\nError: {validation.ErrorMessage}");
            Console.WriteLine("Please try again.");
            Console.ReadKey();
            return;
        }

        var showtime = SelectShowtime(groupSize);
        if (showtime == null) return;

        var reservation = _service.CreateReservation(
            groupType,
            orgName,
            contactPerson,
            groupSize,
            showtime.Id,
            contactEmail,
            contactPhone
        );

        // 6. Show reservation details
        DisplayReservationDetails(reservation);

        // 7. Confirm and pay
        if (ConfirmPayment(reservation.TotalPrice))
        {
            var paymentResult = _service.ProcessPayment("Credit Card", reservation.TotalPrice);
            if (paymentResult.Success)
            {
                Console.WriteLine("\nPayment successful!");
                Console.WriteLine($"Transaction ID: {paymentResult.TransactionId}");
                Console.WriteLine($"Amount paid: €{paymentResult.AmountPaid:N2}");

                // 8. Show final confirmation
                DisplayFinalConfirmation(reservation);
            }
            else
            {
                Console.WriteLine("\nPayment failed. Please try again later.");
            }
        }
        else
        {
            Console.WriteLine("\nYou have cancelled the reservation.");
        }
    }

    private GroupType SelectGroupType()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("Please select group type:");
            Console.WriteLine("1. School Group");
            Console.WriteLine("2. Company Group");
            Console.WriteLine("0. Back");
            Console.Write("\nPlease select (0-2): ");

            var input = Console.ReadLine();
            var (isValid, groupType) = _service.ValidateGroupTypeChoice(input);

            if (isValid)
            {
                if (groupType == GroupType.None) return GroupType.None;

                var terms = _service.GetDisclaimerTerms(groupType);
                if (terms.Any())
                {
                    Console.WriteLine("\n=== Disclaimer ===");
                    foreach (var term in terms)
                    {
                        Console.WriteLine($"- {term}");
                    }

                    Console.Write("\nDo you accept these terms? (Y/N): ");
                }
                else
                {
                    return groupType;
                }
            }

            Console.WriteLine("\nInvalid selection, please try again.");
        }
    }

    private (string orgName, string contactPerson, string email, string phone, int groupSize) GetGroupInformation()
    {
        Console.Clear();
        Console.WriteLine("=== Enter Group Information ===\n");

        string orgName, contactPerson, email, phone;
        int groupSize;

        do { Console.Write("Organization/Company Name: "); }
        while (string.IsNullOrWhiteSpace(orgName = Console.ReadLine()));

        do { Console.Write("Contact Person: "); }
        while (string.IsNullOrWhiteSpace(contactPerson = Console.ReadLine()));

        do { Console.Write("Email: "); }
        while (string.IsNullOrWhiteSpace(email = Console.ReadLine()) || !email.Contains("@"));

        do { Console.Write("Phone Number: "); }
        while (string.IsNullOrWhiteSpace(phone = Console.ReadLine()));

        do
        {
            Console.Write($"Group size (minimum {GroupReservationService.MIN_GROUP_SIZE} people): ");
        } while (!int.TryParse(Console.ReadLine(), out groupSize) || groupSize < GroupReservationService.MIN_GROUP_SIZE);

        return (orgName, contactPerson, email, phone, groupSize);
    }

    private Showtime SelectShowtime(int groupSize)
    {
        var showtimes = _service.GetAvailableShowtimes(groupSize);

        if (!showtimes.Any())
        {
            Console.WriteLine("\nSorry, there are currently no available sessions suitable for your group.");
            Console.WriteLine("If you would like to know more, please contact our customer service directly.");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            return null;
        }

        Console.Clear();
        Console.WriteLine("=== Available Showtimes ===\n");
        Console.WriteLine($"Group size: {groupSize} people\n");

        Console.WriteLine("\n0. Back");

        while (true)
        {
            Console.Write("\nPlease select a showtime (0-{0}): ", showtimes.Count);
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= showtimes.Count)
            {
                if (choice == 0) return null;
                return showtimes[choice - 1];
            }
            Console.WriteLine("Invalid selection. Please try again.");
        }
    }

    private void DisplayReservationDetails(GroupReservationDetails details)
    {
        Console.Clear();
        Console.WriteLine("=== Reservation Details ===\n");
        Console.WriteLine($"Reservation ID: {details.Id}");
        Console.WriteLine($"Organization Name: {details.OrganizationName}");
        Console.WriteLine($"Contact Person: {details.ContactPerson}");
        Console.WriteLine($"Contact Phone: {details.ContactPhone}");
        Console.WriteLine($"Contact Email: {details.ContactEmail}");
        Console.WriteLine($"Group Type: {details.GroupType}");
        Console.WriteLine($"Group Size: {details.GroupSize} people");
        Console.WriteLine($"Showtime: {details.Showtime:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Base Price per Person: €{details.BasePricePerPerson:N2}/person");

        if (details.Discount > 0)
        {
            Console.WriteLine($"Discount: {details.Discount}%");
        }

        Console.WriteLine($"\nTotal Price: €{details.TotalPrice:N2}");
        Console.WriteLine("\n" + "-".PadRight(40, '-'));
    }

    private bool ConfirmPayment(decimal amount)
    {
        Console.Write($"\nConfirm payment of €{amount:N2} and complete reservation? (Y/N): ");
        return Console.ReadLine()?.Trim().ToUpper() == "Y";
    }

    private void DisplayFinalConfirmation(GroupReservationDetails details)
    {
        Console.Clear();
        Console.WriteLine("=== Reservation Successful! ===\n");
        Console.WriteLine("Thank you for your reservation! Here are your booking details:\n");

        Console.WriteLine($"Reservation ID: {details.Id}");
        Console.WriteLine($"Organization: {details.OrganizationName}");
        Console.WriteLine($"Showtime: {details.Showtime:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Group Size: {details.GroupSize} people");
        Console.WriteLine($"Total Amount: €{details.TotalPrice:N2}");

        Console.WriteLine("\nImportant Information:");
        Console.WriteLine("- Please arrive 30 minutes before the showtime");
        Console.WriteLine("- Bring a valid ID with you");
        Console.WriteLine("- Contact customer service at least 24 hours in advance for any changes or cancellations");

        Console.WriteLine("\nA confirmation email has been sent to your email address.");
        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    private void ShowMenu()
    {
        Console.WriteLine("\n=== Group Ticket Reservation ===");
        Console.WriteLine($"Minimum {_service.ValidateGroupSize(0)} people required");
        Console.WriteLine("1. School Group");
        Console.WriteLine("2. Company Group");
        Console.WriteLine("0. Go Back");
        Console.Write("Please select: ");
    }

    private void Run()
    {
        while (true)
        {
            ShowMenu();
            var input = Console.ReadLine();

            var (isValid, type) = _service.ValidateGroupTypeChoice(input);
            if (!isValid)
            {
                Console.WriteLine("Invalid input, please try again");
                continue;
            }

            if (type == GroupType.None) return; // Go back

            try
            {
                var details = CollectGroupInformation(type);
                if (ConfirmReservation(details))
                {
                    var paymentMethod = GetPaymentMethod();
                    if (_service.ProcessPayment(paymentMethod))
                    {
                        ShowConfirmation(details);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operation failed: {ex.Message}");
            }
        }
    }

    private GroupReservationDetails CollectGroupInformation(GroupType type)
    {
        Console.Write($"Please enter {(type == GroupType.School ? "school" : "company")} name: ");
        var orgName = Console.ReadLine();

        Console.Write("Contact person: ");
        var contactPerson = Console.ReadLine();

        Console.Write("Phone number: ");
        var contactNumber = Console.ReadLine();

        Console.Write($"Group size (minimum {30} people): ");
        if (!int.TryParse(Console.ReadLine(), out int size) || !_service.ValidateGroupSize(size))
        {
            throw new Exception($"Group size cannot be less than 30 people");
        }

        // Create reservation
        return _service.CreateReservation(type, orgName, contactPerson, contactNumber, size);
    }

    private bool ConfirmReservation(GroupReservationDetails details)
    {
        Console.WriteLine("\n=== Reservation Confirmation ===");
        Console.WriteLine($"Group Type: {details.GroupType}");
        Console.WriteLine($"Name: {details.OrganizationName}");
        Console.WriteLine($"Contact: {details.ContactPerson}");
        Console.WriteLine($"Number of people: {details.GroupSize}");
        Console.WriteLine($"Total Price: {details.TotalPrice:C2}");

        // Show terms and conditions
        var terms = _service.GetDisclaimerTerms(details.GroupType);
        if (terms.Count > 0)
        {
            Console.WriteLine("\n=== Terms and Conditions ===");
            terms.ForEach(Console.WriteLine);
            Console.Write("\nDo you accept these terms? (Y/N): ");
        }
        else
        {
            Console.Write("\nConfirm reservation? (Y/N): ");
        }

        return Console.ReadLine().Trim().ToUpper() == "Y";
    }

    private string GetPaymentMethod()
    {
        Console.WriteLine("\n=== Payment Method ===");
        Console.WriteLine("1. Bank Transfer");
        Console.WriteLine("2. Online Payment");
        Console.WriteLine("3. Pay on Site");
        Console.Write("Please select: ");

        return Console.ReadLine() switch
        {
            "1" => "Bank Transfer",
            "2" => "Online Payment",
            "3" => "Pay on Site",
            _ => "Online Payment"
        };
    }

    private void ShowConfirmation(GroupReservationDetails details)
    {
        Console.WriteLine("\n=== Reservation Successful! ===");
        Console.WriteLine($"Reservation ID: GRP-{details.Id:D4}");
        Console.WriteLine($"Date: {details.ReservationDate:yyyy-MM-dd HH:mm}");

        if (details.GroupType == GroupType.School)
        {
            Console.WriteLine("\n=== Next Steps ===");
            Console.WriteLine("Please submit required documents within 10 days to: group@example.com");
        }
    }
}