using System.Collections.Generic;

namespace SharedLibrary.Extensions;

public static class CollectionsExtensions
{
    public static bool IsNotNullOrEmpty<T>(this List<T> list)
    {
        return list?.Any() ?? false;
    }
}
