using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static int NextIndex<T>(this List<T> list, int currentIndex)
    {
        return (currentIndex + 1) % list.Count;
    }

    public static int PreviousIndex<T>(this List<T> list, int currentIndex)
    {
        return (currentIndex - 1 + list.Count) % list.Count;
    }
}