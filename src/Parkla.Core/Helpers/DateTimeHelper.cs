namespace Parkla.Core.Helpers;

public static class DateTimeHelper
{
    public static bool IsTimeRangesIntercept(DateTime start1, DateTime end1, DateTime start2, DateTime end2) {
        return (start1 >= start2 && start1 <= end2) ||
            (end1 >= start2 && end1 <= end2) ||
            (start1 <= start2 && end1 >= end2);
    }

    public static bool IsTimeRangeIncludeWeekday(DateTime start, DateTime end, DayOfWeek dayOfWeek) {
        if(start.DayOfWeek == dayOfWeek || end.DayOfWeek == dayOfWeek) return true;
        var diffDay = (end-start).TotalDays;
        
        for(var i = 1; i < diffDay; i++) {
            var iterDay = start.AddDays(i);
            if(iterDay.DayOfWeek == dayOfWeek) return true;
        }

        return false;
    }
}