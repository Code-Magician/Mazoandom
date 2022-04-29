using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveAlgorithm : Maze
{
    public override void Generate()
    {
        this.transform.position = Vector3.zero;
        Generate(Random.Range(1, lenX - 1), Random.Range(1, lenZ - 1));
    }

    private void Generate(int x, int z)
    {
        if (CountSquareNeighbours(x, z) >= 2)
            return;

        map[x, z] = 0;


        List<int> indexes = Extentions.GenerateRandomIndexes(directions.Count);

        for (int i = 0; i < directions.Count; i++)
        {
            int nx = x + directions[indexes[i]].x;
            int nz = z + directions[indexes[i]].z;

            Generate(nx, nz);
        }
    }

    private List<int> GenerateRandomIndexes(int Length)
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
}
