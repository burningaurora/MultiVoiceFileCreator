using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public static class IListExtensions
{
    /// <summary>
    /// First check if the list is actually a null object, then an Any check
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    public static bool SafeAny<T>(this IList<T> query, Predicate<T> predicate = null)
    {
        if (query == null)
            return false;

        return predicate != null ? query.Any(e => predicate(e)) : query.Any();
    }

    /// <summary>
    /// Return a count of a list, 0 when null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    public static int SafeCount<T>(this IList<T> query, Predicate<T> predicate = null)
    {
        if (query == null)
            return 0;

        return predicate != null ? query.Count(e => predicate(e)) : query.Count;
    }

    /// <summary>
    /// Checks if a list of strings contains a string;
    /// Trims both strings to remove any leading or trailing white spaces prior to comparison;
    /// Ignores string case;
    /// Method called xxx.Contains(yyy)
    /// Returns the string list value for the compared string
    /// </summary>
    /// <param name="str">Original string list</param>
    /// <param name="containsStr">String to check if it is part of the original string list</param>
    /// <param name="ignoreCase"></param>
    /// <returns>The original string list value if the string is int he list
    /// </returns>
    public static string ExistInList(this List<string> strList, string containsStr, bool ignoreCase = true)
    {
        if (ignoreCase)
            return strList.FirstOrDefault(f => f.Compare(containsStr));
        return strList.FirstOrDefault(f => f == containsStr);
    }

    /// <summary>
    /// Returns if a string is within a list of strings
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool InList(this object obj, params object[] list)
    {
        return list.Contains(obj);
    }
}