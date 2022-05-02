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
                            Vector3 trPos = maze.piecePlaces[x, z].model.transform.position;
                            Quaternion trRot = maze.piecePlaces[x, z].model.transform.rotation;


                            float height = maze.scale * maze.level * maze.levelMultiplier;
                            GameObject temp = Instantiate(randPrefab, trPos, trRot, this.transform);
                            if (temp.tag == "Zombie")
                            {
                                GameObject fpc = GameObject.FindGameObjectWithTag("Player").gameObject;
                                if (Vector3.Distance(trPos, fpc.transform.position) <= 5)
                                {
                                    Destroy(temp);
                                }
                                else
                                    GameStats.totalZombiesInCurrentLevel++;
                            }
                        }
                    }
                }
            }
    }
}
