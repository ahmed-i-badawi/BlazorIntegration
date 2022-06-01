namespace SharedLibrary.Extensions;
public static class NumbersExtensions
{
    public static bool IsNotNullOrEmpty(this int? _number)
    {
        return _number != null && _number != 0;
    }

    public static bool IsNotNullOrEmpty(this int _number)
    {
        return _number != 0;
    }
}
