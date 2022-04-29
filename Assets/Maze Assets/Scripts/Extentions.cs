using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    // Generates List of Random Indexes from [0, Length-1]
    public static List<int> GenerateRandomIndexes(int Length)
    {
        List<int> indexes = new List<int>();

        while (indexes.Count != Length)
        {
            int idx = Random.Range(0, Length);
            if (!indexes.Contains(idx))
                indexes.Add(idx);
        }

        return indexes;
    }


    // Shuffles a List.
    private static System.Random rng = new System.Random();
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
}
