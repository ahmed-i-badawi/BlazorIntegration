using System.Text.RegularExpressions;

namespace SharedLibrary.Extensions;
public static class ValidatorsExtensions
{
    public static bool IsValidEmailAddress(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }
        var regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
        return regex.IsMatch(s);
    }

    public static bool IsValidPassword(this string pw)
    {
        if (string.IsNullOrWhiteSpace(pw))
        {
            return false;
        }
        var lowercase = new Regex("[a-z]+");
        var uppercase = new Regex("[A-Z]+");
        var digit = new Regex("(\\d)+");
        var symbol = new Regex("(\\W)+");

        return (lowercase.IsMatch(pw) && uppercase.IsMatch(pw) && digit.IsMatch(pw) && symbol.IsMatch(pw));
    }
}
