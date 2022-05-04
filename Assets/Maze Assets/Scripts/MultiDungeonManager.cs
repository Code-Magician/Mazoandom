using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDungeonManager : MonoBehaviour
{


    [Header("MultiStories Properties")]
    [SerializeField] GameObject maze;
    [SerializeField] GameObject path;
    [SerializeField] GameObject FPC;
    [SerializeField] GameObject FinsihLine;

    public int totalLevels = 3;


    [Header("Single Maze Properties")]
    public int width = 10;
    public int depth = 10;
    public int scale = 6;
    // levelMultiplier*scale  equals distance between 2 mazes.
    public float levelMultiplier = 1.5f;


    [Header("Maze Pieces")]
    public GameObject stairWell;


    List<MapLocation> lowerLevel = new List<MapLocation>();
    List<MapLocation> upperLevel = new List<MapLocation>();
    Maze[] dungeons;

    private void Start()
    {

    }


    public void Build()
    {
        int level = 0;
        dungeons = new Maze[totalLevels];

        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject temp;
            if (i == 1 || i == totalLevels)
                temp = Instantiate(path, this.transform);
            else
                temp = Instantiate(maze, this.transform);

            Maze m = temp.GetComponent<Maze>();
            dungeons[i - 1] = m;

            m.lenX = width;
            m.lenZ = depth;
            m.scale = scale;
            m.level = level++;
            m.levelMultiplier = levelMultiplier;
            m.Build();
        }

        width += 6;
        depth += 6;
        ConnectMazesNew();
        PlaceFinishLine();
        // PlaceTelePort();
    }


    void PlaceFinishLine()
    {
        // Place FPC
        bool placed = false;
        Maze m = dungeons[0];
        for (int z = 0; z < m.lenZ; z++)
        {
            for (int x = 0; x < m.lenX; x++)
            {
                if (m.piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Down || m.piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Up ||
                m.piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Left || m.piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Right)
                {
                    Debug.Log("Placed");
                    FPC.transform.position = m.piecePlaces[x, z].model.transform.position + new Vector3(0, 2f, 0);
                    m.entryPoint = new MapLocation(x, z);
                    placed = true;
                    break;
                }
            }
            if (placed)
                break;
        }


        // Set FinishLine
        placed = false;
        m = dungeons[totalLevels - 1];
        for (int z = 0; z < m.lenZ; z++)
        {
            for (int x = 0; x < m.lenX; x++)
            {
                if (m.piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Down || m.piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Up ||
                m.piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Left || m.piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Right)
                {
                    Debug.Log("Finished");
                    FinsihLine.transform.position = m.piecePlaces[x, z].model.transform.position;
                    FinsihLine.transform.rotation = m.piecePlaces[x, z].model.transform.rotation;
                    m.exitPoint = new MapLocation(x, z);
                    placed = true;
                    break;
                }
            }
            if (placed)
                break;
        }

        FPC.SetActive(true);


        //Place Zombies, Ammo and Med
        foreach (Maze maze in dungeons)
        {
            PlaceObjects[] placeObjs = maze.GetComponents<PlaceObjects>();
            foreach (PlaceObjects x in placeObjs)
            {
                if (x != null)
                {
                    x.Place();
                }
            }
        }


        GameStats.totalZombiesSpawned = GameStats.totalZombiesInCurrentLevel;
    }


    public void PlaceTelePort()
    {
        Teleporter tele = GetComponent<Teleporter>();
        if (tele != null)
        {
            tele.Add(dungeons[tele.startMaze], dungeons[tele.endMaze]);
        }
        else
        {
            Debug.Log("Teleporter is Null...");
        }
    }


    public void ConnectMazesNew()
    {
        for (int i = 0; i < dungeons.Length - 1; i++)
        {
            if (PlaceStairs(i, 0, Maze.PieceType.DeadEnd_Left, Maze.PieceType.DeadEnd_Right, stairWell)) continue;
            if (PlaceStairs(i, 180, Maze.PieceType.DeadEnd_Right, Maze.PieceType.DeadEnd_Left, stairWell)) continue;
            if (PlaceStairs(i, 90, Maze.PieceType.DeadEnd_Up, Maze.PieceType.DeadEnd_Down, stairWell)) continue;
            PlaceStairs(i, -90, Maze.PieceType.DeadEnd_Down, Maze.PieceType.DeadEnd_Up, stairWell);
        }

        TranslateDungeonsByOffset();
    }



    public void TranslateDungeonsByOffset()
    {
        for (int i = 0; i < dungeons.Length - 1; i++)
        {
            dungeons[i + 1].transform.Translate(dungeons[i + 1].xOffset * scale, 0, dungeons[i + 1].zOffset * scale);
        }
    }


    bool PlaceStairs(int mazeIndex, float rotAngle, Maze.PieceType bottomType, Maze.PieceType upperType, GameObject stairPrefab)
    {
        List<MapLocation> startingLocations = new List<MapLocation>();
        List<MapLocation> endingLocations = new List<MapLocation>();

        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (dungeons[mazeIndex].piecePlaces[x, z].piece == bottomType)
                    startingLocations.Add(new MapLocation(x, z));

                if (dungeons[mazeIndex + 1].piecePlaces[x, z].piece == upperType)
                    endingLocations.Add(new MapLocation(x, z));
            }

        if (startingLocations.Count == 0 || endingLocations.Count == 0) return false;

        MapLocation bottomOfStairs = startingLocations[UnityEngine.Random.Range(0, startingLocations.Count)];
        MapLocation topOfStairs = endingLocations[UnityEngine.Random.Range(0, endingLocations.Count)];

        dungeons[mazeIndex + 1].xOffset = bottomOfStairs.x - topOfStairs.x + dungeons[mazeIndex].xOffset;
        dungeons[mazeIndex + 1].zOffset = bottomOfStairs.z - topOfStairs.z + dungeons[mazeIndex].zOffset;

        Vector3 stairPosBottom = new Vector3(bottomOfStairs.x * dungeons[mazeIndex].scale,
                                                    dungeons[mazeIndex].scale * dungeons[mazeIndex].level
                                                            * dungeons[mazeIndex].levelMultiplier,
                                                    bottomOfStairs.z * dungeons[mazeIndex].scale);
        Vector3 stairPosTop = new Vector3(topOfStairs.x * dungeons[mazeIndex + 1].scale,
                                                    dungeons[mazeIndex + 1].scale * dungeons[mazeIndex + 1].level
                                                            * dungeons[mazeIndex + 1].levelMultiplier,
                                                    topOfStairs.z * dungeons[mazeIndex + 1].scale);

        Destroy(dungeons[mazeIndex].piecePlaces[bottomOfStairs.x, bottomOfStairs.z].model);
        Destroy(dungeons[mazeIndex + 1].piecePlaces[topOfStairs.x, topOfStairs.z].model);

        GameObject stairs = Instantiate(stairPrefab, stairPosBottom, Quaternion.identity);
        stairs.transform.Rotate(0, rotAngle, 0);
        dungeons[mazeIndex].piecePlaces[bottomOfStairs.x, bottomOfStairs.z].model = stairs;
        dungeons[mazeIndex].piecePlaces[bottomOfStairs.x, bottomOfStairs.z].piece = Maze.PieceType.StairWell_Up;
        dungeons[mazeIndex].exitPoint = bottomOfStairs;

        dungeons[mazeIndex + 1].piecePlaces[topOfStairs.x, topOfStairs.z].model = null;
        dungeons[mazeIndex + 1].piecePlaces[topOfStairs.x, topOfStairs.z].piece = Maze.PieceType.StairWell_Down;
        dungeons[mazeIndex + 1].entryPoint = topOfStairs;

        stairs.transform.SetParent(dungeons[mazeIndex].gameObject.transform);
        return true;
    }


    public void ConnectMazesOLD()
    {
        // iterate over all mazes
        for (int i = 0; i < dungeons.Length - 1; i++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    // stairwell can be placed only if the below level dungeon have deadend to right then upper level must have deadend to left they are opposite of each other or we can say difference of angle between them is 180 degree.
                    if (dungeons[i].piecePlaces[x, z].model != null && dungeons[i + 1].piecePlaces[x, z].model != null)
                    {
                        bool atLower = (dungeons[i].piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Down ||
                                        dungeons[i].piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Up ||
                                        dungeons[i].piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Right ||
                                        dungeons[i].piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Left);

                        bool atUpper = (dungeons[i + 1].piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Down ||
                                        dungeons[i + 1].piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Up ||
                                        dungeons[i + 1].piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Right ||
                                        dungeons[i + 1].piecePlaces[x, z].piece == Maze.PieceType.DeadEnd_Left);

                        float rotB = ((Mathf.Abs(dungeons[i].piecePlaces[x, z].model.gameObject.transform.eulerAngles.y -
                                        dungeons[i + 1].piecePlaces[x, z].model.gameObject.transform.eulerAngles.y)));
                        bool rotationFine = (rotB >= 175 && rotB <= 185);

                        float ri = dungeons[i].piecePlaces[x, z].model.gameObject.transform.eulerAngles.y;
                        float ri_1 = dungeons[i + 1].piecePlaces[x, z].model.gameObject.transform.eulerAngles.y;

                        if (atLower && atUpper)
                            Debug.Log(dungeons[i].piecePlaces[x, z].model.name + " " + dungeons[i + 1].piecePlaces[x, z].model.name + " " + ri + " " + ri_1);

                        GameObject temp;
                        if (atLower && atUpper && rotationFine)
                        {
                            Vector3 pos = new Vector3(x * scale, dungeons[i].level * scale * levelMultiplier, z * scale);
                            Quaternion rot = dungeons[i].piecePlaces[x, z].model.gameObject.transform.rotation;

                            Destroy(dungeons[i].piecePlaces[x, z].model);
                            Destroy(dungeons[i + 1].piecePlaces[x, z].model);

                            temp = Instantiate(stairWell, pos, rot, this.transform);
                            temp.name = "StairWell";

                            dungeons[i].piecePlaces[x, z].model = temp;
                            dungeons[i + 1].piecePlaces[x, z].model = temp;
                            dungeons[i].piecePlaces[x, z].piece = Maze.PieceType.StairWell;
                            dungeons[i + 1].piecePlaces[x, z].piece = Maze.PieceType.StairWell;

                        }
                    }
                }
            }
        }
    }
}
