public static class DateSelection
{
    public static int monthMenu()
    {
        List<List<string>> Months = new List<List<string>>
        {
            new List<string> { "January", "February", "  March  " },
            new List<string> { " April ", "   May"  , "   June  " },
            new List<string> { " July  ", " August ", "September" },
            new List<string> { "October", "November", "December " }
        };

        List<List<int>> MonthsNRS = new List<List<int>>
        {
            new List<int> { 1, 2, 3 },
            new List<int> { 4, 5, 6 },
            new List<int> { 7, 8 , 9 },
            new List<int> { 10, 11, 12 }
        };

        MainMenu monthMenu = new MainMenu(Months, "Select the month you want to view");
        int[] selectedMonth = monthMenu.Run();
        int month = MonthsNRS[selectedMonth[0]][selectedMonth[1]];
        return month;
    }

    public static List<string> Months = new() {"January", "February", "  March  ", " April ", "   May"  , "   June  ", " July  ", " August ", "September", "October", "November", "December "};

    public static List<List<string>> DaysInSelectedMonth(int month)
    {
        List<List<string>> Options = new();

        int year = 2026;
        int daysInMonth = DateTime.DaysInMonth(year, month);
        List<string> currentWeek = new List<string>();

        for (int day = 1; day <= daysInMonth; day++)
        {
            if (day < 10) currentWeek.Add(" " + day.ToString());
            else currentWeek.Add(day.ToString());

            if (currentWeek.Count == 7)
            {
                Options.Add(currentWeek);
                currentWeek = new List<string>();
            }
        }
        if (currentWeek.Count > 0) Options.Add(currentWeek);

        return Options;
    }

    public static DateTime GetDateFromCoordinate(int[] coord, int year, int month)
    {
        int row = coord[0];
        int col = coord[1];
        int day = row * 7 + col + 1;
        int daysInMonth = DateTime.DaysInMonth(year, month);
        if (day > daysInMonth) day = daysInMonth;
        return new DateTime(year, month, day);
    }

    public static DateTime DatePicker()
    {
        int month = monthMenu();
        MainMenu dayMenu = new(DaysInSelectedMonth(month), Months[month - 1]);

        int year = DateTime.Now.Year;
        return DateSelection.GetDateFromCoordinate(dayMenu.Run(), year, month);

    }
}