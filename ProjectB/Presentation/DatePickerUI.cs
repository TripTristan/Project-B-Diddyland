using System;

public class DatePickerUI
{
    private readonly FinancialLogic _financialLogic;
    private readonly ReservationLogic _reservationLogic;

    public DatePickerUI(FinancialLogic financialLogic, ReservationLogic reservationLogic)
    {
        _financialLogic = financialLogic;
        _reservationLogic = reservationLogic;
    }

    public DateTime PickDate()
    {
        int month = DateSelection.monthMenu();

        MainMenu dayMenu = new(
            DateSelection.DaysInSelectedMonth(month),
            DateSelection.Months[month - 1]);

        DateTime date = DateSelection.GetDateFromCoordinate(dayMenu.Run(), 2026, month);

        if (date < DateTime.Now.Date)
            throw new InvalidOperationException("Cannot pick a date in the past.");

        if (!_reservationLogic.CheckForTimeslotsOnDate(date))
            _reservationLogic.PopulateTimeslots(date);

        return date;
    }
}
