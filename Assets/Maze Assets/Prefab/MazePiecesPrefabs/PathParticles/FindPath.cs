using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPath : MonoBehaviour
{
    public GameObject particles;
    GameObject magic;
    Maze thisMaze;
    AStarPathFinding aStar;
    PathMarker destination;

    private void Start()
    {
        aStar = GetComponent<AStarPathFinding>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (aStar != null)
            {
                RaycastHit hit;
                Ray ray = new Ray(this.transform.position, -Vector3.up);
                if (Physics.Raycast(ray, out hit))
                {
                    thisMaze = hit.collider.gameObject.GetComponentInParent<Maze>();
                    AStarPathFinding hasAstar = thisMaze.gameObject.GetComponent<AStarPathFinding>();
                    if (hasAstar != null)
                        return;

                    MapLocation currLocation = hit.collider.gameObject.GetComponent<MapLoc>().location;
                    MapLocation exitLocation = thisMaze.exitPoint;

                    destination = aStar.Build(thisMaze, currLocation, exitLocation);

                    magic = Instantiate(particles, this.gameObject.transform.position, this.gameObject.transform.rotation);
                    StopCoroutine(DisplayMagicPath());
                    StartCoroutine(DisplayMagicPath());
                }
            }
        }
    }

    IEnumerator DisplayMagicPath()
    {
        List<MapLocation> magicPath = new List<MapLocation>();
        while (destination != null)
        {
            magicPath.Add(new MapLocation(destination.location.x, destination.location.z));
            destination = destination.parent;
        }

        magicPath.Reverse();

        foreach (MapLocation loc in magicPath)
        {
            magic.transform.LookAt(thisMaze.piecePlaces[loc.x, loc.z].model.transform.position + new Vector3(0, 1, 0));

            int loopTimeOut = 0;
            while (Vector2.Distance(new Vector2(magic.transform.position.x, magic.transform.position.z),
                    new Vector2(thisMaze.piecePlaces[loc.x, loc.z].model.transform.position.x,
                     thisMaze.piecePlaces[loc.x, loc.z].model.transform.position.z)) > 2
                     && loopTimeOut < 100)
            {
                magic.transform.Translate(0, 0, 10f * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
                loopTimeOut++;
            }
        }

        Destroy(magic, 10);
    }
}
