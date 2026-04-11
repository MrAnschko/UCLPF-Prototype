using System;
using System.Collections;
using System.Collections.Generic;


public static class ExtensionMethods
{
    private static Random rng = new Random();

    // Fisher yates shuffle taken from: https://stackoverflow.com/questions/273313/randomize-a-listt
    public static void Shuffle<T>(this IList<T> list)
    {

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Shuffle(this int[] list)
    {

        int n = list.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // below taken from here https://stackoverflow.com/questions/872323/method-call-if-not-null-in-c-sharp
    public static void SafeInvoke(this Action action)
    {
        if (action != null) action();
    }

    public static void SafeInvoke<T>(this Action<T> action, T value)
    {
        if (action != null) action(value);
    }
}
