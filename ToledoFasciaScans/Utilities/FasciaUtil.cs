using System.Text;
using ToledoFasciaScans.Enumerations;

namespace ToledoFasciaScans.Utilities;

public static class FasciaUtil
{
    public static bool IsStaurdayOrSunday(this DateTime today)
    {
        return today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }
    
    public static DaysToSubtract GetDaysToSubtract(this DateTime today)
    {
        if (today.DayOfWeek == DayOfWeek.Monday && today.Day <= 7)
        {
            return DaysToSubtract.Monday;
        }

        return DaysToSubtract.PreviousDay;
    }

    public static string InClause(this List<string> items)
    {
        StringBuilder sb = new();
        items.ForEach(p =>
        {
            sb.Append('@').Append(p).Append(',');
        });
        string inClause = sb.ToString();
        return inClause.TrimEnd(',');
    }
}