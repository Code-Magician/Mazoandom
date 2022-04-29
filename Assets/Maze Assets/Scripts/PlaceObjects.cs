using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjects : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int percentageProbablity;
    [SerializeField] Maze.PieceType pieceType;



    public void Place()
    {
        Maze maze = GetComponent<Maze>();

        for (int z = 0; z < maze.lenZ; z++)
            for (int x = 0; x < maze.lenX; x++)
            {
                if (maze.piecePlaces[x, z].piece == pieceType)
                {
                    int rand = Random.Range(1, 101);
                    if (rand <= percentageProbablity)
                    {
                        Transform tr = maze.piecePlaces[x, z].model.transform;
                        Instantiate(prefab, new Vector3(tr.position.x, tr.position.y, tr.position.z), tr.rotation, this.transform);
                    }
                }
            }
    }
}
