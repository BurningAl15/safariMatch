using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static List<T> ResetIfNull<T>(this List<T> list) where T : new()
    {
        if (list == null)
        {
            list = new List<T>();
        }

        return list;
    }
}