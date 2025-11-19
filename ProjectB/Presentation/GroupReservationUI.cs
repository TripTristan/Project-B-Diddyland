public static class GroupReservationUI
{
    public static async Task ShowReservationFlowAsync(IGroupReservationService service, IPaymentService paymentService)
    {
        if (service == null) throw new ArgumentNullException(nameof(service));
        if (paymentService == null) throw new ArgumentNullException(nameof(paymentService));

        Console.Clear();
        Console.WriteLine("=== Group Booking System ===\n");

        var type = await SelectGroupTypeAsync(service);
        if (type == GroupType.None) return;

        var (orgName, contactPerson, email, phone, groupSize) = await CollectGroupInfoAsync();

        var validation = await service.ValidateGroupSizeAsync(groupSize);
        if (!validation.IsValid)
        {
            Console.WriteLine($"\nError: {validation.ErrorMessage}");
            return;
        }

        var sessions = await service.GetAvailableSessionsAsync(groupSize);
        if (!sessions.Any())
        {
            Console.WriteLine("\nNo available sessions for your group size.");
            return;
        }

        var session = await SelectSessionAsync(sessions);
        if (session == null) return;

        var reservation = await service.CreateReservationAsync(
            type, orgName, contactPerson, email, phone, groupSize, session.Id);

        await DisplayReservationDetailsAsync(reservation);

        if (!PromptForChoice("\nConfirm reservation?", "Yes", "No")) return;

        await GroupPaymentUI.ProcessPaymentAsync(paymentService, reservation);
    }

    private static async Task<GroupType> SelectGroupTypeAsync(IGroupReservationService service)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Select group type:");
            Console.WriteLine("1. School Group");
            Console.WriteLine("2. Company Group");
            Console.WriteLine("0. Back");
            Console.Write("\nSelect (0-2): ");

            var (isValid, type) = await service.ValidateGroupTypeChoiceAsync(Console.ReadLine());
            if (isValid) return type;

            Console.WriteLine("Invalid selection.");
        }
    }

    private static async Task<(string orgName, string contactPerson, string email, string phone, int groupSize)>
        CollectGroupInfoAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Group Information ===\n");

        string orgName, contactPerson, email, phone;
        int groupSize;

        do { Console.Write("Organization Name: "); }
        while (string.IsNullOrWhiteSpace(orgName = Console.ReadLine()));

        do { Console.Write("Contact Person: "); }
        while (string.IsNullOrWhiteSpace(contactPerson = Console.ReadLine()));

        do { Console.Write("Email: "); }
        while (string.IsNullOrWhiteSpace(email = Console.ReadLine()) || !email.Contains("@"));

        do { Console.Write("Phone: "); }
        while (string.IsNullOrWhiteSpace(phone = Console.ReadLine()));

        do
        {
            Console.Write($"Group size (min {GroupReservationService.MIN_GROUP_SIZE}): ");
        } while (!int.TryParse(Console.ReadLine(), out groupSize));

        return (orgName, contactPerson, email, phone, groupSize);
    }

    private static async Task<SessionAvailabilityDto?> SelectSessionAsync(List<SessionAvailabilityDto> sessions)
    {
        Console.WriteLine("\nAvailable Sessions:");
        for (int i = 0; i < sessions.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {sessions[i].Date} - {sessions[i].Time}");
        }

        while (true)
        {
            Console.Write($"\nSelect (1-{sessions.Count}): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= sessions.Count)
                return sessions[choice - 1];

            Console.WriteLine("Invalid selection.");
        }
    }

    private static async Task DisplayReservationDetailsAsync(GroupReservationDto details)
    {
        Console.Clear();
        Console.WriteLine("=== Reservation Details ===\n");
        Console.WriteLine($"Order: {details.OrderNumber}");
        Console.WriteLine($"Organization: {details.OrganizationName}");
        Console.WriteLine($"Contact: {details.ContactPerson}");
        Console.WriteLine($"Group Size: {details.GroupSize}");
        Console.WriteLine($"Discount: {details.Discount:P}");
        Console.WriteLine($"Total: {details.FinalPrice:C}");
    }

    private static bool PromptForChoice(string message, string yes, string no)
    {
        Console.Write($"\n{message} (y/n): ");
        return Console.ReadLine()?.Trim().ToLower() == "y";
    }
}