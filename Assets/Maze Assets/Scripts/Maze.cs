using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocation
{
    public int x;
    public int z;

    public MapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, z);
    }

    public static MapLocation operator +(MapLocation a, MapLocation b)
       => new MapLocation(a.x + b.x, a.z + b.z);

    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
            return false;
        else
        {
            return (x == ((MapLocation)obj).x && z == ((MapLocation)obj).z);
        }
    }

    public override int GetHashCode()
    {
        return 0;
    }
}


public class Maze : MonoBehaviour
{
    public enum Type
    {
        Maze, Dungeon
    }

    public Type It_is_a;

    [Header("Maze Properties")]
    public int lenX = 30;
    public int lenZ = 30;


    // Square Directions.
    public List<MapLocation> directions = new List<MapLocation>() {
        new MapLocation(1, 0),
        new MapLocation(-1, 0),
        new MapLocation(0, 1),
        new MapLocation(0, -1)
    };

    // Contains all the locations which are not walls...
    public List<MapLocation> locations = new List<MapLocation>();

    // entry and exit point in a maze, Dungeon or a Corridor...
    public MapLocation entryPoint;
    public MapLocation exitPoint;

    // it tells us if at position map[x, z] it is wall or it is a corridor
    // if map[x, z] = 1 then it's a wall if it's 0 then it's a corridor.
    public byte[,] map;

    // Scale of maze
    public int scale = 6;
    public int level = 0;
    public float levelMultiplier = 2f;
    public float xOffset = 0, zOffset = 0;

    [System.Serializable]
    public struct Module
    {
        public GameObject prefab;
        public Vector3 rotation;
    }

    [Header("Maze Pieces Prefabs")]
    public Module straightH;
    public Module straightV;
    public Module straightLightH;
    public Module straightLightV;

    public Module deadEnd_Up;
    public Module deadEnd_Down;
    public Module deadEnd_Left;
    public Module deadEnd_Right;

    public Module crossRoad_;

    public Module tJunction_DeadUp;
    public Module tJunciton_DeadDown;
    public Module tJunction_DeadLeft;
    public Module tJunction_DeadRight;

    public Module right_Up_Corner;
    public Module right_Down_Corner;
    public Module left_Up_Corner;
    public Module left_Down_Corner;

    public Module floorPiece;
    public Module ceilingPiece;

    public Module topWall;
    public Module downWall;
    public Module leftWall;
    public Module rightWall;

    public Module topDoor;
    public Module downDoor;
    public Module leftDoor;
    public Module rightDoor;

    public Module cornerPiller;







    [Header("Player")]
    public GameObject FPC;

    [Header("Room Properties")]
    public int room;
    public int minSize;
    public int MaxSize;

    private List<MapLocation> pillerLocations = new List<MapLocation>();


    public enum PieceType
    {
        StraightH,
        StratightV,
        Right_Up_Corner,
        Right_Down_Corner,
        Left_Up_Corner,
        Left_Down_Corner,
        TJunction_DeadDown,
        TJunction_DeadUp,
        TJunction_DeadLeft,
        TJunction_DeadRight,
        DeadEnd_Up,
        DeadEnd_Down,
        DeadEnd_Right,
        DeadEnd_Left,
        Wall,
        CrossRoads,
        Room,
        Manhole,
        StairWell,
        StairWell_Up,
        StairWell_Down
    }


    public struct Pieces
    {
        public PieceType piece;
        public GameObject model;

        public Pieces(PieceType p, GameObject m)
        {
            piece = p;
            model = m;
        }
    }

    // Same as Map which is used to tell if at any position there is wall or corridor by returning 1 and 0 respectively.
    // this will give us the the piece type and model of that piece that is at a position (x, z)
    public Pieces[,] piecePlaces;




    private void Start()
    {
        // Build();
    }




    public void Build()
    {
        DrawMap();
        PlaceFPC();
    }




    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     foreach (Transform child in transform)
        //     {
        //         Destroy(child.gameObject);
        //     }
        //     DrawMap();
        //     PlaceFPC();
        // }
    }




    // Makes all the locations which is the part of the x*z matrix the corridor 
    // by setting the map to 1.
    public virtual void InitializeMap()
    {
        map = new byte[lenX, lenZ];
        piecePlaces = new Pieces[lenX, lenZ];

        for (int z = 0; z < lenZ; z++)
        {
            for (int x = 0; x < lenX; x++)
            {
                map[x, z] = 1;
            }
        }
    }




    // Generates the corridors in the map.
    // This method does not work because we are using the Other scripts(Crawler, Prims, Wilsons, Recursive) to override this method.
    public virtual void Generate()
    {
        for (int z = 0; z < lenZ; z++)
        {
            for (int x = 0; x < lenX; x++)
            {
                if (Random.Range(0, 1000) < 500)
                    map[x, z] = 0;
            }
        }
    }




    // This method draws the map on the Positions Based on map that we created.
    public virtual void DrawMap()
    {
        InitializeMap();
        Generate();
        // Make Rooms in the Existing maze that we generated using Above Generate Function.
        MakeRooms(room, minSize, MaxSize);

        AdditionalChanges();

        // it's like first maze is at height 0 then there is one maze gap then other maze is at height 2 ans so on;
        float height = levelMultiplier * level * scale;
        GameObject temp;
        bool TogglePlaceLight = true;

        for (int z = 0; z < lenZ; z++)
        {
            for (int x = 0; x < lenX; x++)
            {
                // Current position at which we are placing objects.
                Vector3 pos = new Vector3(x * scale, height, z * scale);

                if (map[x, z] == 1) // not maze part
                {
                    // GameObject basicElement = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // basicElement.transform.parent = this.transform;

                    // basicElement.transform.localScale = new Vector3(scale, scale, scale);

                    // basicElement.transform.position = new Vector3(x * scale, 0, z * scale);

                    piecePlaces[x, z].piece = PieceType.Wall;
                    // Because there is empty space when map[x, y] gives us 1.
                    piecePlaces[x, z].model = null;
                }
                else if (Search2D(x, z, "X1X001X1X")) // rest all are maze part which are not room
                {
                    temp = Instantiate(deadEnd_Right.prefab, pos, Quaternion.Euler(deadEnd_Right.rotation), this.transform);
                    temp.name = "DeadEnd_Right";

                    piecePlaces[x, z].piece = PieceType.DeadEnd_Right;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "X0X101X1X"))
                {
                    temp = Instantiate(deadEnd_Down.prefab, pos, Quaternion.Euler(deadEnd_Down.rotation), this.transform);
                    temp.name = "DeadEnd_Down";

                    piecePlaces[x, z].piece = PieceType.DeadEnd_Down;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "X1X101X0X"))
                {
                    temp = Instantiate(deadEnd_Up.prefab, pos, Quaternion.Euler(deadEnd_Up.rotation), this.transform);
                    temp.name = "DeadEnd_Up";

                    piecePlaces[x, z].piece = PieceType.DeadEnd_Up;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "X1X100X1X"))
                {
                    temp = Instantiate(deadEnd_Left.prefab, pos, Quaternion.Euler(deadEnd_Left.rotation), this.transform);
                    temp.name = "DeadEnd_Left";

                    piecePlaces[x, z].piece = PieceType.DeadEnd_Left;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "X0X101X0X"))
                {
                    if (TogglePlaceLight && straightLightV.prefab != null)
                    {
                        temp = Instantiate(straightLightV.prefab, pos, Quaternion.Euler(straightLightV.rotation), this.transform);
                        temp.name = "StraightLightV";
                    }
                    else
                    {
                        temp = Instantiate(straightV.prefab, pos, Quaternion.Euler(straightV.rotation), this.transform);
                        temp.name = "StraightV";
                    }
                    TogglePlaceLight = !TogglePlaceLight;

                    piecePlaces[x, z].piece = PieceType.StratightV;
                    piecePlaces[x, z].model = temp;

                }
                else if (Search2D(x, z, "X1X000X1X"))
                {
                    if (TogglePlaceLight && straightLightH.prefab != null)
                    {
                        temp = Instantiate(straightLightH.prefab, pos, Quaternion.Euler(straightLightH.rotation), this.transform);
                        temp.name = "StraightLightH";
                    }
                    else
                    {
                        temp = Instantiate(straightH.prefab, pos, Quaternion.Euler(straightH.rotation), this.transform);
                        temp.name = "StraightH";
                    }
                    TogglePlaceLight = !TogglePlaceLight;

                    piecePlaces[x, z].piece = PieceType.StraightH;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "10X00110X"))
                {
                    temp = Instantiate(tJunction_DeadRight.prefab, pos, Quaternion.Euler(tJunction_DeadRight.rotation), this.transform);
                    temp.name = "TJunction_DeadRight";

                    piecePlaces[x, z].piece = PieceType.TJunction_DeadRight;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "X01100X01"))
                {
                    temp = Instantiate(tJunction_DeadLeft.prefab, pos, Quaternion.Euler(tJunction_DeadLeft.rotation), this.transform);
                    temp.name = "TJunction_DeadLeft";

                    piecePlaces[x, z].piece = PieceType.TJunction_DeadLeft;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "X1X000101"))
                {
                    temp = Instantiate(tJunction_DeadUp.prefab, pos, Quaternion.Euler(tJunction_DeadUp.rotation), this.transform);
                    temp.name = "TJunction_DeadUp";
                    piecePlaces[x, z].piece = PieceType.TJunction_DeadUp;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "101000X1X"))
                {
                    temp = Instantiate(tJunciton_DeadDown.prefab, pos, Quaternion.Euler(tJunciton_DeadDown.rotation), this.transform);
                    temp.name = "TJunction_DeadDown";

                    piecePlaces[x, z].piece = PieceType.TJunction_DeadDown;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "X01100X1X"))
                {
                    temp = Instantiate(left_Down_Corner.prefab, pos, Quaternion.Euler(left_Down_Corner.rotation), this.transform);
                    temp.name = "Left_Down_Corner";

                    piecePlaces[x, z].piece = PieceType.Left_Down_Corner;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "X1X100X01"))
                {
                    temp = Instantiate(left_Up_Corner.prefab, pos, Quaternion.Euler(left_Up_Corner.rotation), this.transform);
                    temp.name = "Left_Up_Corner";

                    piecePlaces[x, z].piece = PieceType.Left_Up_Corner;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "10X001X1X"))
                {
                    temp = Instantiate(right_Down_Corner.prefab, pos, Quaternion.Euler(right_Down_Corner.rotation), this.transform);
                    temp.name = "Right_Down_Corner";

                    piecePlaces[x, z].piece = PieceType.Right_Down_Corner;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "X1X00110X"))
                {
                    temp = Instantiate(right_Up_Corner.prefab, pos, Quaternion.Euler(right_Up_Corner.rotation), this.transform);
                    temp.name = "Right_Up_Corner";

                    piecePlaces[x, z].piece = PieceType.Right_Up_Corner;
                    piecePlaces[x, z].model = temp;
                }
                else if (Search2D(x, z, "101000101"))
                {
                    temp = Instantiate(crossRoad_.prefab, pos, Quaternion.Euler(crossRoad_.rotation), this.transform);
                    temp.name = "CrossRoad";
                    piecePlaces[x, z].piece = PieceType.CrossRoads;
                    piecePlaces[x, z].model = temp;
                }
                else if (map[x, z] == 0 && (CountSquareNeighbours(x, z) > 1 && CountDiagonalNeighbours(x, z) >= 1) ||
                                          CountSquareNeighbours(x, z) >= 1 && CountDiagonalNeighbours(x, z) > 1) // all the places in the room where we can put a floor
                {
                    temp = Instantiate(floorPiece.prefab, pos, Quaternion.Euler(floorPiece.rotation), this.transform);
                    temp.name = "Floor";
                    piecePlaces[x, z].piece = PieceType.Room;
                    piecePlaces[x, z].model = temp;

                    temp = Instantiate(ceilingPiece.prefab, pos, Quaternion.Euler(ceilingPiece.rotation), this.transform);
                    temp.name = "Ceiling";

                    LocateWalls(x, z);

                    GameObject piller;
                    if (top)
                    {
                        temp = Instantiate(topWall.prefab, pos, Quaternion.Euler(topWall.rotation), this.transform);
                        temp.name = "Top Wall";

                        // we know that top of middle location is wall. so we just need to set pillers at it's corners if it's an outside edge.
                        // Y -> it doesn't concert the current piller.
                        // Piller is like this if you place it at pos (x, z) then it will be TopRight piller.
                        // S P
                        // S S
                        // P -> pilller, S -> Space

                        // Top Walls Right Piller.
                        // Y 1 0
                        // Y 0 0
                        // Y Y Y
                        if (!pillerLocations.Contains(new MapLocation(x, z)) && map[x + 1, z] == 0 && map[x + 1, z + 1] == 0)
                        {
                            piller = Instantiate(cornerPiller.prefab, this.transform);
                            piller.transform.position = new Vector3(x * scale, height, z * scale);
                            pillerLocations.Add(new MapLocation(x, z));
                            piller.name = "Top Walls - Right Piller";

                            // To Remove the Z-Fighting on Pillers(There was flickering effect because we were rendering 2 objects in same plane).
                            piller.transform.localScale = new Vector3(1.01f, 1, 1.01f);
                        }

                        // Top Walls Left Piller.
                        // 0 1 Y
                        // 0 0 Y
                        // Y Y Y
                        if (!pillerLocations.Contains(new MapLocation(x - 1, z)) && map[x - 1, z] == 0 && map[x - 1, z + 1] == 0)
                        {
                            piller = Instantiate(cornerPiller.prefab, this.transform);
                            piller.transform.position = new Vector3((x - 1) * scale, height, z * scale);
                            pillerLocations.Add(new MapLocation(x - 1, z));
                            piller.name = "Top Walls - Left Piller";

                            // To Remove the Z-Fighting on Pillers(There was flickering effect because we were rendering 2 objects in same plane).
                            piller.transform.localScale = new Vector3(1.01f, 1, 1.01f);
                        }
                    }
                    if (bottom)
                    {
                        temp = Instantiate(downWall.prefab, pos, Quaternion.Euler(downWall.rotation), this.transform);
                        temp.name = "Botton Wall";

                        // Y Y Y
                        // 0 0 Y
                        // 0 1 Y
                        if (!pillerLocations.Contains(new MapLocation(x - 1, z - 1)) && map[x - 1, z] == 0 && map[x - 1, z - 1] == 0)
                        {
                            piller = Instantiate(cornerPiller.prefab, this.transform);
                            piller.transform.position = new Vector3((x - 1) * scale, height, (z - 1) * scale);
                            pillerLocations.Add(new MapLocation(x - 1, z - 1));
                            piller.name = "Bottom Walls - Left Piller";

                            // To Remove the Z-Fighting on Pillers(There was flickering effect because we were rendering 2 objects in same plane).
                            piller.transform.localScale = new Vector3(1.01f, 1, 1.01f);
                        }

                        // Y Y Y
                        // Y 0 0 
                        // Y 1 0
                        if (!pillerLocations.Contains(new MapLocation(x, z - 1)) && map[x + 1, z] == 0 && map[x + 1, z - 1] == 0)
                        {
                            piller = Instantiate(cornerPiller.prefab, this.transform);
                            piller.transform.position = new Vector3(x * scale, height, (z - 1) * scale);
                            pillerLocations.Add(new MapLocation(x, z - 1));
                            piller.name = "Bottom Walls - Right Piller";

                            // To Remove the Z-Fighting on Pillers(There was flickering effect because we were rendering 2 objects in same plane).
                            piller.transform.localScale = new Vector3(1.01f, 1, 1.01f);
                        }
                    }
                    if (left)
                    {
                        temp = Instantiate(leftWall.prefab, pos, Quaternion.Euler(leftWall.rotation), this.transform);
                        temp.name = "Left Wall";

                        // 0 0 Y
                        // 1 0 Y 
                        // Y Y Y
                        if (!pillerLocations.Contains(new MapLocation(x - 1, z)) && map[x, z + 1] == 0 && map[x - 1, z + 1] == 0)
                        {
                            piller = Instantiate(cornerPiller.prefab, this.transform);
                            piller.transform.position = new Vector3((x - 1) * scale, height, z * scale);
                            pillerLocations.Add(new MapLocation(x - 1, z));
                            piller.name = "Left Walls - Top Piller";

                            // To Remove the Z-Fighting on Pillers(There was flickering effect because we were rendering 2 objects in same plane).
                            piller.transform.localScale = new Vector3(1.01f, 1, 1.01f);
                        }

                        // Y Y Y
                        // 1 0 Y
                        // 0 0 Y
                        if (!pillerLocations.Contains(new MapLocation(x - 1, z - 1)) && map[x - 1, z - 1] == 0 && map[x, z - 1] == 0)
                        {
                            piller = Instantiate(cornerPiller.prefab, this.transform);
                            piller.transform.position = new Vector3((x - 1) * scale, height, (z - 1) * scale);
                            pillerLocations.Add(new MapLocation(x - 1, z - 1));
                            piller.name = "Left Walls - Bottom Piller";

                            // To Remove the Z-Fighting on Pillers(There was flickering effect because we were rendering 2 objects in same plane).
                            piller.transform.localScale = new Vector3(1.01f, 1, 1.01f);
                        }
                    }
                    if (right)
                    {
                        temp = Instantiate(rightWall.prefab, pos, Quaternion.Euler(rightWall.rotation), this.transform);
                        temp.name = "Right Wall";

                        // Y 0 0 
                        // Y 0 1
                        // Y Y Y 
                        if (!pillerLocations.Contains(new MapLocation(x, z)) && map[x, z + 1] == 0 && map[x + 1, z + 1] == 0)
                        {
                            piller = Instantiate(cornerPiller.prefab, this.transform);
                            piller.transform.position = new Vector3(x * scale, height, z * scale);
                            pillerLocations.Add(new MapLocation(x, z));
                            piller.name = "Right Walls - Top Piller";

                            // To Remove the Z-Fighting on Pillers(There was flickering effect because we were rendering 2 objects in same plane).
                            piller.transform.localScale = new Vector3(1.01f, 1, 1.01f);
                        }

                        // Y Y Y 
                        // Y 0 1
                        // Y 0 0 
                        if (!pillerLocations.Contains(new MapLocation(x, z - 1)) && map[x, z - 1] == 0 && map[x + 1, z - 1] == 0)
                        {
                            piller = Instantiate(cornerPiller.prefab, this.transform);
                            piller.transform.position = new Vector3(x * scale, height, (z - 1) * scale);
                            pillerLocations.Add(new MapLocation(x, z - 1));
                            piller.name = "Right Walls - Bottom Piller";

                            // To Remove the Z-Fighting on Pillers(There was flickering effect because we were rendering 2 objects in same plane).
                            piller.transform.localScale = new Vector3(1.01f, 1, 1.01f);
                        }
                    }
                }
                else
                {
                    // GameObject basicElement = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    // basicElement.transform.parent = this.transform;

                    // basicElement.transform.localScale = new Vector3(scale, scale, scale);

                    // basicElement.transform.position = new Vector3(x * scale, 0, z * scale);
                }
            }
        }

        // As we can see that piecePlaces is begin set in above nested for loop so we can't use new LocateDoors method in those loops
        // That's why we have to declare new nested for loop after piecePlaces has been completely set.
        for (int z = 0; z < lenZ; z++)
        {
            for (int x = 0; x < lenX; x++)
            {
                Vector3 pos = new Vector3(x * scale, height, z * scale);

                // Checking if the current (x, z) position is Room.
                if (piecePlaces[x, z].piece == PieceType.Room)
                {
                    LocateDoors(x, z);

                    if (top)
                    {
                        temp = Instantiate(topDoor.prefab, pos, Quaternion.Euler(topDoor.rotation), this.transform);
                        temp.name = "Top 2 Way Door";

                        // There was Z-Fighting so to remove it we just move the door on it's local axis a little bit.
                        temp.transform.Translate(0, 0, 0.01f);
                    }

                    if (bottom)
                    {
                        temp = Instantiate(downDoor.prefab, pos, Quaternion.Euler(downDoor.rotation), this.transform);
                        temp.name = "Bottom 2 Way Door";

                        // There was Z-Fighting so to remove it we just move the door on it's local axis a little bit.
                        temp.transform.Translate(0, 0, 0.01f);
                    }

                    if (left)
                    {
                        temp = Instantiate(leftDoor.prefab, pos, Quaternion.Euler(leftDoor.rotation), this.transform);
                        temp.name = "Left 2 Way Door";

                        // There was Z-Fighting so to remove it we just move the door on it's local axis a little bit.
                        temp.transform.Translate(0, 0, 0.01f);
                    }

                    if (right)
                    {
                        temp = Instantiate(rightDoor.prefab, pos, Quaternion.Euler(rightDoor.rotation), this.transform);
                        temp.name = "Right 2 Way Door";

                        // There was Z-Fighting so to remove it we just move the door on it's local axis a little bit.
                        temp.transform.Translate(0, 0, 0.01f);
                    }

                }
            }
        }

        PlaceObjects placeObjs = GetComponent<PlaceObjects>();
        if (placeObjs != null)
        {
            placeObjs.Place();
        }


        for (int z = 0; z < lenZ; z++)
            for (int x = 0; x < lenX; x++)
            {
                temp = piecePlaces[x, z].model;
                if (temp != null)
                {
                    temp.GetComponent<MapLoc>().location = new MapLocation(x, z);
                }
                if (map[x, z] != 1)
                    locations.Add(new MapLocation(x, z));
            }
    }




    // counts How many Neighbours are corridors at given x, z
    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;

        if (x <= 0 || z <= 0 || x >= lenX - 1 || z >= lenZ - 1)
            return 5;

        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        if (map[x, z + 1] == 0) count++;

        return count;
    }



    public int CountDiagonalNeighbours(int x, int z)
    {
        int count = 0;

        if (x <= 0 || z <= 0 || x >= lenX - 1 || z >= lenZ - 1)
            return 5;

        if (map[x - 1, z - 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;

        return count;
    }



    public int CountAllNeighbours(int x, int z)
    {
        return CountSquareNeighbours(x, z) + CountDiagonalNeighbours(x, z);
    }



    // X means it can be any (wall or corridor)
    // 1 means it's a wall
    // 0 means it's a corridor
    // What this function do is that it takes a position (x, z) and a string pattern and checks if the neighbours of this position matches the pattern of not
    // if they match the pattern then it return true else it returns false.
    // pattern is nothing but a string of length 9. In which each character represents the neighbour of(x, z) or (x, z) itself
    // for example if pattern is X0X101X01. then it can be written as 
    // X 0 X
    // 1 0 1
    // X 0 X
    // we are using pattern because each part of maze has it's unique pattern for example
    // for staight piece which is verticle there will be the pattern which is given above in example.
    // for any other piece like a deadEnd there will be differect pattern.
    // So we pass a position of the piece and a pattern if the pattern matches then we place that piece at (x, z) position.
    public bool Search2D(int mx, int mz, string pattern = "000000000")
    {
        int count = 0;
        int idx = 0;
        for (int z = 1; z >= -1; z--)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (pattern[idx] - '0' == map[mx + x, mz + z] || pattern[idx] == 'X')
                    count++;
                idx++;
            }
        }

        return (count == 9);
    }




    // Places FPC at the random location in the maze which is not a wall.
    public void PlaceFPC()
    {
        if (FPC == null)
            return;

        int x = Random.Range(1, lenX - 1);
        int z = Random.Range(1, lenZ - 1);

        while (map[x, z] == 1)
        {
            x = Random.Range(1, lenX - 1);
            z = Random.Range(1, lenZ - 1);
        }

        FPC.transform.position = new Vector3(x * scale, 2, z * scale);
    }




    // It takes 3 arguments 
    // count -> number of rooms you want in your maze.
    // minSize and maxSize -> range of depth and widths of the room.
    // it chooses a random start position for the maze and from (st, en) to (st+depth, en+width) it makes all the map location equal to 0.
    public virtual void MakeRooms(int count, int minSize, int maxSize)
    {
        if (It_is_a == Type.Maze)
        {
            room = 0;
            count = 0;
        }

        for (int i = 1; i <= count; i++)
        {
            int stX = Random.Range(1, lenX - 1);
            int stZ = Random.Range(1, lenZ - 1);

            int roomWidth = Random.Range(minSize, maxSize);
            int roomDepth = Random.Range(minSize, maxSize);

            for (int x = stX; x < lenX - 1 && x < stX + roomWidth; x++)
            {
                for (int z = stZ; z < lenZ - 1 && z < stZ + roomDepth; z++)
                {
                    map[x, z] = 0;
                }
            }
        }
    }




    // we pass a location in it and it tells us which square neighbour is a wall.
    // top means just above is the wall, bottom means just below is the wall and so on...
    bool top, bottom, left, right;
    public void LocateWalls(int x, int z)
    {
        top = bottom = left = right = false;

        if (x <= 0 || z <= 0 || x >= lenX - 1 || z >= lenZ - 1) return;

        if (map[x, z + 1] == 1) top = true;
        if (map[x, z - 1] == 1) bottom = true;
        if (map[x - 1, z] == 1) left = true;
        if (map[x + 1, z] == 1) right = true;
    }




    // Simiar to Locate Walls but in this we check extra something and that is if walls are present at both the other corners of the doorways 
    // lets take an example of Top doorway
    // 1 0 1
    // Y Y Y
    // Y Y Y
    // the 1 0 1 we check that if it is true then we place a door between middle Y and 0.
    public void LocateDoors(int x, int z)
    {
        top = bottom = left = right = false;

        if (x <= 0 || z <= 0 || x >= lenX - 1 || z >= lenZ - 1) return;

        // if (map[x, z + 1] == 0 && map[x - 1, z + 1] == 1 && map[x + 1, z + 1] == 1) top = true;
        // if (map[x, z - 1] == 0 && map[x - 1, z - 1] == 1 && map[x + 1, z - 1] == 1) bottom = true;
        // if (map[x - 1, z] == 0 && map[x - 1, z - 1] == 1 && map[x - 1, z + 1] == 1) left = true;
        // if (map[x + 1, z] == 0 && map[x + 1, z - 1] == 1 && map[x + 1, z + 1] == 1) right = true;

        // New Method to see if the square neighbour positions are door or not.
        // But it can be used only after all the piecePlaces positions have been set
        // In case of map it was set in Generate Method before the Draw Map but piecePlaces is being set in Draw Map
        // so we first need to set it completely to use this method.
        if (piecePlaces[x, z + 1].piece != PieceType.Room && piecePlaces[x, z + 1].piece != PieceType.Wall) top = true;
        if (piecePlaces[x, z - 1].piece != PieceType.Room && piecePlaces[x, z - 1].piece != PieceType.Wall) bottom = true;
        if (piecePlaces[x - 1, z].piece != PieceType.Room && piecePlaces[x - 1, z].piece != PieceType.Wall) left = true;
        if (piecePlaces[x + 1, z].piece != PieceType.Room && piecePlaces[x + 1, z].piece != PieceType.Wall) right = true;
    }




    public void AdditionalChanges()
    {
        int oldLenX = lenX;
        int oldLenZ = lenZ;
        byte[,] oldMap = map;

        lenX += 6;
        lenZ += 6;

        map = new byte[lenX, lenZ];
        InitializeMap();

        for (int z = 0; z < oldLenZ; z++)
            for (int x = 0; x < oldLenX; x++)
                map[x + 3, z + 3] = oldMap[x, z];

        int xPos, zPos;

        AStarPathFinding aStar = GetComponent<AStarPathFinding>();
        if (aStar != null)
        {
            aStar.Build();

            if (aStar.start.location.x < aStar.goal.location.z)
            {
                xPos = aStar.start.location.x;
                zPos = aStar.start.location.z;

                while (xPos > 1)
                {
                    map[xPos, zPos] = 0;
                    xPos--;
                }


                xPos = aStar.goal.location.x;
                zPos = aStar.goal.location.z;

                while (xPos < lenX - 1)
                {
                    map[xPos, zPos] = 0;
                    xPos++;
                }
            }
            else
            {
                xPos = aStar.start.location.x;
                zPos = aStar.start.location.z;

                while (xPos < lenX - 1)
                {
                    map[xPos, zPos] = 0;
                    xPos++;
                }


                xPos = aStar.goal.location.x;
                zPos = aStar.goal.location.z;

                while (xPos > 1)
                {
                    map[xPos, zPos] = 0;
                    xPos--;
                }
            }
        }
        else
        {
            // Upper DeadEnd...
            xPos = Random.Range(5, lenX - 5);
            zPos = lenZ - 2;
            while (map[xPos, zPos] == 0 && zPos > 1)
            {
                map[xPos, zPos] = 0;
                zPos--;
            }

            // Lower DeadEnd...
            xPos = Random.Range(5, lenX - 5);
            zPos = 1;
            while (map[xPos, zPos] == 0 && zPos < lenZ - 1)
            {
                map[xPos, zPos] = 0;
                zPos++;
            }

            // Left DeadEnd...
            xPos = 1;
            zPos = Random.Range(5, lenZ - 5);
            while (map[xPos, zPos] == 0 && xPos < lenX - 1)
            {
                map[xPos, zPos] = 0;
                xPos++;
            }

            // Right DeadEnd...
            xPos = lenX - 2;
            zPos = Random.Range(5, lenZ - 5);
            while (map[xPos, zPos] == 0 && xPos > 1)
            {
                map[xPos, zPos] = 0;
                xPos--;
            }
        }
    }
}
