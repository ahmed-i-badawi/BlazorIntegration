using System;

namespace BlazorServer.Extensions;

public static class DateExtensions
{

    public static string ToShortDateWithValidation(this DateTime date)
    {
        return date.IsNotNullOrEmpty() ? date.ToShortDateString() : "";
    }
    public static bool IsNotNullOrEmpty(this DateTime? date)
    {
        return (date.HasValue && date != new DateTime().Date);
    }
    public static bool IsNotNullOrEmpty(this DateTime date)
    {
        return (date.Date != new System.DateTime().Date);
    }
}
