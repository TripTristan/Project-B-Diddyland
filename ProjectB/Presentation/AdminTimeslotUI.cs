using System;
using System.Linq;

public class AdminTimeslotUI
{
    private readonly AdminSessionLogic _logic;
    private readonly DatePickerUI _datePicker;

    public AdminTimeslotUI(AdminSessionLogic logic, DatePickerUI datePicker)
    {
        _logic = logic;
        _datePicker = datePicker;
    }

    public void Run()
    {
        DateTime date = _datePicker.PickDate();
        var sessions = _logic.GetSessionsByDate(date);

        var options = sessions.Select(s =>
            new List<string> { $"Session {s.Id} | Time:{s.Time} | Capacity:{s.Capacity}" }
        ).ToList();

        var menu = new MainMenu(options, "Select session");
        int[] pick = menu.Run();

        Console.Write("New capacity: ");
        long cap = long.Parse(Console.ReadLine()!);

        _logic.SetCapacity(sessions[pick[0]].Id, cap);
    }
}
