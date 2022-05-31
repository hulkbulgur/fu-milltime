namespace FuMilltime2
{
    using System;

    internal static class DateManager
    {
        public static DateOnly CurrentDate { get; private set; } = DateOnly.FromDateTime(DateTime.Today);

        public static event EventHandler? DateChanged;
        public static void ChangeDate(DateOnly newDate)
        {
            CurrentDate = newDate;
            DateChanged?.Invoke(new object(), new EventArgs());
        }
    }
}
