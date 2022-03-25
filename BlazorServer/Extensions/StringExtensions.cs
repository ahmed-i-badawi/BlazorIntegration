namespace BlazorServer.Extensions;

public static class StringExtensions
{
    public static string TrimAll(this string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            return string.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
        }
        else
        {
            return str;
        }
    }

}