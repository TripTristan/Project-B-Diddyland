public class GroupReservationUI
{
    private readonly GroupReservationService _service;

    public GroupReservationUI(GroupReservationService service)
    {
        _service = service;
    }

    public void ShowReservationFlow()
    {
        ShowGroupReservationOptions();
        var choice = Console.ReadLine();
        
        var result = _service.GetGroupInformation(choice);
        
        if (result.IsValid)
        {
            var details = _service.CreateReservation(result.Data);
            DisplayConfirmation(details);
        }
    }

    private void DisplayConfirmation(GroupReservationDetails details)
    {

        Console.WriteLine($"\n=== Reservation Success ===");
        Console.WriteLine($"ID: {details.Id}");
        Console.WriteLine("-".PadLeft(20, '-'));

        Console.WriteLine($"Organization Name: {details.OrganizationName}");
        Console.WriteLine($"Contact Person: {details.ContactPerson}");
        
        Console.WriteLine("-".PadLeft(20, '-'));
        Console.WriteLine($"Reservation Date: {details.ReservationDate}");
        Console.WriteLine($"Group Size: {details.GroupSize}");
        Console.WriteLine($"Discount: {details.Discount}");
        Console.WriteLine($"Total Price: {details.TotalPrice}");

    }
}