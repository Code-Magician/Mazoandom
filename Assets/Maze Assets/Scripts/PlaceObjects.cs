using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjects : MonoBehaviour
{
    [SerializeField] GameObject[] prefab;
    [SerializeField] int percentageProbablity;



    public void Place()
    {
        Maze maze = GetComponent<Maze>();

        for (int z = 0; z < maze.lenZ; z++)
            for (int x = 0; x < maze.lenX; x++)
            {
                if (maze.map[x, z] != 1)
                {
                    int rand = Random.Range(1, 101);
                    if (rand <= percentageProbablity)
                    {
                        GameObject randPrefab = prefab[Random.Range(0, prefab.Length)];
                        if (maze.piecePlaces[x, z].model != null)
                        {
                            Transform tr = maze.piecePlaces[x, z].model.transform;
                            float height = maze.scale * maze.level * maze.levelMultiplier;
                            GameObject temp = Instantiate(randPrefab, new Vector3(tr.position.x, height, tr.position.z), tr.rotation, this.transform);
                            if (temp.tag == "Zombie")
                                GameStats.totalZombiesInCurrentLevel++;
                        }
                    }
                }
            }
    }
}
