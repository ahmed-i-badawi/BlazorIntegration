using System.Collections.Generic;

namespace BlazorServer.Extensions;

public static class ReflectionExtensions
{
    public static object GetPropValue(this object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }

    public static bool SetPropValue(this object src, string propName, object value)
    {
        var prop = src.GetType().GetProperty(propName);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(src, value, null);
            return true;
        }
        return false;
    }

    public static bool MapObj(this object destination, object src)
    {
        try
        {
            if (src != null && destination != null)
            {
                foreach (var prop in src.GetType().GetProperties())
                {
                    destination.SetPropValue(prop.Name, src.GetPropValue(prop.Name));
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            return false;
        }
       
    }

    public static bool MergeObj(this object destination, object src)
    {
        try
        {
            foreach (var prop in src.GetType().GetProperties())
            {
                if (
                    src.GetPropValue(prop.Name) != null &&
                    src.GetPropValue(prop.Name) != destination.GetPropValue(prop.Name)
                )
                {
                    destination.SetPropValue(prop.Name, src.GetPropValue(prop.Name));
                }
            }

            return true;

        }
        catch (Exception e)
        {
            return false;
        }
        
    }
}
