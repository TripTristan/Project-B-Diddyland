using System;
using System.Collections.Generic;
using System.Linq;

public class AdminReservationUI
{
    private readonly AdminReservationLogic _logic;
    private readonly DatePickerUI _datePicker;
    private readonly UserReservation _userReservation;

    public AdminReservationUI(
        AdminReservationLogic logic,
        DatePickerUI datePicker,
        UserReservation userReservation)
    {
        _logic = logic;
        _datePicker = datePicker;
        _userReservation = userReservation;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();

            var options = new List<List<string>>
            {
                new() { "View ALL bookings" },
                new() { "View by USER" },
                new() { "View by DATE" },
                new() { "Create booking for USER" },
                new() { "Back" }
            };

            var menu = new MainMenu(options, "Admin – Reservations");
            int[] pick = menu.Run();

            switch (pick[0])
            {
                case 0:
                    PickFromList(_logic.GetAll());
                    break;

                case 1:
                    var user = PickUser();
                    PickFromList(_logic.GetByUser(user.Id));
                    break;

                case 2:
                    var date = _datePicker.PickDate();
                    PickFromList(_logic.GetBySessionDate(date.Ticks));
                    break;

                case 3:
                    var u = PickUser();
                    _userReservation.StartReservationForUser(u);
                    break;

                default:
                    return;
            }

            UiHelpers.Pause();
        }
    }

    private void PickFromList(List<BookingModel> bookings)
    {
        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings found.");
            return;
        }

        var rows = bookings
            .Select(b => new List<string> { $"{b.OrderNumber} | Qty:{b.Quantity} | {b.Type}" })
            .ToList();

        var menu = new MainMenu(rows, "Select Booking");
        int[] pick = menu.Run();

        BookingActions(bookings[pick[0]]);
    }

    private void BookingActions(BookingModel b)
    {
        while (true)
        {

            var options = new List<List<string>>
            {
                new() { "Change quantity" },
                new() { "Change date and session" },
                new() { "Delete" },
                new() { "Back" }
            };

            string header =
                $"Current Reservation\n\n" +
                $"Order   : {b.OrderNumber}\n" +
                $"User ID : {b.CustomerId}\n" +
                $"Session : {b.SessionId}\n" +
                $"Qty     : {b.Quantity}\n" +
                $"Type    : {b.Type}\n" +
                $"Price   : {b.Price:C}\n\n" +
                "Edit Reservation";

            var menu = new MainMenu(options, header);
            int[] pick = menu.Run();

            switch (pick[0])
            {
                case 0:
                    ChangeQuantity(b);
                    b = _logic.GetByOrder(b.OrderNumber);
                    break;

                case 1:
                    ChangeDate(b);
                    b = _logic.GetByOrder(b.OrderNumber);
                    break;

                case 2:
                    _logic.DeleteReservation(b.OrderNumber);
                    Console.WriteLine("Reservation deleted.");
                    UiHelpers.Pause();
                    return;

                default:
                    return;
            }
        }
    }

    private void ShowBookingDetails(BookingModel b)
    {
        Console.WriteLine("Current Reservation\n");
        Console.WriteLine($"Order   : {b.OrderNumber}");
        Console.WriteLine($"User ID : {b.CustomerId}");
        Console.WriteLine($"Session : {b.SessionId}");
        Console.WriteLine($"Qty     : {b.Quantity}");
        Console.WriteLine($"Type    : {b.Type}");
        Console.WriteLine($"Price   : {b.Price:C}\n");
    }

    private void ChangeSession(BookingModel b)
    {
        DateTime date = _datePicker.PickDate();
        var session = _userReservation.PickSessionForDate(date);

        int newSessionId = checked((int)session.Id); // FIX long -> int

        _logic.EditReservation(
            b.OrderNumber,
            newSessionId,
            b.Quantity,
            b.Price,
            b.Type == "FastPass"
        );

        Console.WriteLine("Session updated.");
        UiHelpers.Pause();
    }

    private void ChangeQuantity(BookingModel b)
    {
        var guests = _userReservation.PickGuestQuantities();
        int total = guests.Sum();

        ReservationType type = total >= 10 ? ReservationType.Group : ReservationType.Normal;

        double newPrice = _userReservation.CalculatePriceFromGuests(guests, type, null);

        _logic.EditReservation(
            b.OrderNumber,
            b.SessionId,
            total,
            (decimal)newPrice,
            b.Type == "FastPass"
        );

        Console.WriteLine("Quantity updated.");
        UiHelpers.Pause();
    }

    private void ChangeDate(BookingModel b)
    {
        DateTime date = _datePicker.PickDate();
        var session = _userReservation.PickSessionForDate(date);

        int newSessionId = checked((int)session.Id); // FIX long -> int

        _logic.EditReservation(
            b.OrderNumber,
            newSessionId,
            b.Quantity,
            b.Price,
            b.Type == "FastPass"
        );

        Console.WriteLine("Date updated.");
        UiHelpers.Pause();
    }

    private UserModel PickUser()
    {
        var users = _logic.GetAllUsers();

        if (users.Count == 0)
            throw new Exception("No users found.");

        var rows = users
            .Select(u => new List<string> { $"{u.Id} | {u.Name} | {u.Email}" })
            .ToList();

        var menu = new MainMenu(rows, "Select User");
        int[] pick = menu.Run();

        return users[pick[0]];
    }
}
