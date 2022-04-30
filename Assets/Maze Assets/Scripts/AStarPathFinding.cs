using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker
{
    public MapLocation location;
    public float H;
    public float G;
    public float F;
    public PathMarker parent;

    public PathMarker(MapLocation l, float f, float g, float h, PathMarker p)
    {
        location = l;
        H = h;
        G = g;
        F = f;
        parent = p;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
            return false;
        else
        {
            PathMarker temp = (PathMarker)(obj);
            bool eq = location.Equals(temp.location);
            return eq;
        }
    }

    public override int GetHashCode()
    {
        return 0;
    }
}



public class AStarPathFinding : MonoBehaviour
{
    [Header("Properties")]
    public Maze maze;

    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();
    public List<PathMarker> finalPath = new List<PathMarker>();
    public PathMarker start;
    public PathMarker goal;
    PathMarker weAreAt;
    PathMarker lastPos;
    bool done = false;
    // bool started = false;


    public void Build()
    {
        BeginSearch();
        while (!done)
            FindPath(weAreAt);
        StorePath();
        MarkCorridorLocations();
    }

    public PathMarker Build(Maze m, MapLocation start, MapLocation end)
    {
        maze = m;

        BeginSearch(start, end);
        while (!done)
            FindPath(weAreAt);
        StorePath();

        return lastPos;
    }

    public void BeginSearch()
    {
        done = false;

        List<MapLocation> allCorridors = new List<MapLocation>();
        for (int z = 0; z < maze.lenZ; z++)
            for (int x = 0; x < maze.lenX; x++)
                if (maze.map[x, z] != 1)
                    allCorridors.Add(new MapLocation(x, z));

        MapLocation startLocation = allCorridors[Random.Range(0, allCorridors.Count)];
        MapLocation goalLocation = allCorridors[Random.Range(0, allCorridors.Count)];
        while (goalLocation.Equals(start))
            goalLocation = allCorridors[Random.Range(0, allCorridors.Count)];

        start = new PathMarker(startLocation, 0, 0, 0, null);
        goal = new PathMarker(goalLocation, 0, 0, 0, null);

        open.Clear();
        closed.Clear();
        open.Add(start);
        weAreAt = start;
    }

    public void BeginSearch(MapLocation startLoc, MapLocation endLoc)
    {
        done = false;

        start = new PathMarker(startLoc, 0, 0, 0, null);
        goal = new PathMarker(endLoc, 0, 0, 0, null);

        open.Clear();
        closed.Clear();
        open.Add(start);
        weAreAt = start;
    }


    public void FindPath(PathMarker thisNode)
    {
        if (thisNode.Equals(goal))
        {
            done = true;
            lastPos = weAreAt;
            return;
        }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbourLocation = thisNode.location + dir;

            if (maze.map[neighbourLocation.x, neighbourLocation.z] == 1)
                continue;

            if (neighbourLocation.x <= 0 || neighbourLocation.x >= maze.lenX ||
                neighbourLocation.z <= 0 || neighbourLocation.z >= maze.lenZ)
                continue;

            if (IsInClosedList(neighbourLocation))
                continue;

            float G = Vector2.Distance(thisNode.location.ToVector(), neighbourLocation.ToVector()) + thisNode.G;
            float H = Vector2.Distance(neighbourLocation.ToVector(), goal.location.ToVector());
            float F = H + G;

            PathMarker tempMarker = new PathMarker(neighbourLocation, F, G, H, thisNode);

            if (!UpdateMarker(neighbourLocation, F, G, H, thisNode))
                open.Add(tempMarker);
        }
        open = open.OrderBy(p => p.F).ToList<PathMarker>();
        PathMarker next = open.ElementAt(0);
        closed.Add(next);
        open.RemoveAt(0);

        weAreAt = next;
    }


    public bool UpdateMarker(MapLocation loc, float f, float g, float h, PathMarker thisNode)
    {
        foreach (PathMarker p in open)
        {
            if (p.location.Equals(loc))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = thisNode;
                return true;
            }
        }
        return false;
    }

    public bool IsInClosedList(MapLocation neighbourLocation)
    {
        foreach (PathMarker p in closed)
        {
            if (p.location.Equals(neighbourLocation))
                return true;
        }
        return false;
    }


    public void StorePath()
    {
        finalPath.Clear();
        PathMarker curr = weAreAt;
        while (weAreAt != null)
        {
            finalPath.Add(weAreAt);
            weAreAt = weAreAt.parent;
        }
        finalPath.Reverse();
    }

    public void MarkCorridorLocations()
    {
        for (int z = 0; z < maze.lenZ; z++)
            for (int x = 0; x < maze.lenX; x++)
            {
                maze.map[x, z] = 1;
            }

        foreach (PathMarker p in finalPath)
        {
            maze.map[p.location.x, p.location.z] = 0;
        }
    }

}
