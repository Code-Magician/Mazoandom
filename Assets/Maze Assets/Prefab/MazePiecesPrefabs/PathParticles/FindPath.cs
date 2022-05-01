using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindPath : MonoBehaviour
{
    public GameObject particles;

    GameObject magic;
    [SerializeField] float relapseTime = 100f;
    [SerializeField] Scrollbar magicPathBar;
    [SerializeField] Image magicPathHandle;
    float timePassed = 110;
    Maze thisMaze;
    AStarPathFinding aStar;
    PathMarker destination;

    private void Start()
    {
        aStar = GetComponent<AStarPathFinding>();
    }

    private void Update()
    {
        if (timePassed <= relapseTime + 1f)
        {
            timePassed += Time.deltaTime;
            magicPathBar.size = (timePassed) / 100;
        }
        else
        {
            magicPathHandle.color = Color.green;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && (timePassed >= relapseTime))
        {

            if (aStar != null)
            {
                RaycastHit hit;
                Ray ray = new Ray(this.transform.position, Vector3.up);
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject hitObj = hit.collider.gameObject;
                    if (hitObj == null)
                        return;

                    Debug.Log("Hitted Gameoject : " + hitObj.name);
                    thisMaze = hitObj.transform.root.gameObject.transform.GetChild(GameStats.currLevel - 1).GetComponent<Maze>();
                    Debug.Log("Maze is  : " + thisMaze.gameObject);
                    if (thisMaze == null)
                        return;

                    AStarPathFinding hasAstar = thisMaze.gameObject.GetComponent<AStarPathFinding>();
                    if (hasAstar != null)
                        return;

                    int cnt = 0;
                    GameObject temp = hitObj;
                    MapLocation currLocation = new MapLocation(0, 0);
                    while (cnt < 100 && temp.tag != "DungeonManager")
                    {
                        if (temp.GetComponent<MapLoc>() != null)
                        {
                            currLocation = temp.GetComponent<MapLoc>().location;
                            break;
                        }
                        temp = temp.transform.parent.gameObject;
                        cnt++;
                    }
                    if (currLocation.Equals(new MapLocation(0, 0)))
                        return;

                    MapLocation exitLocation = thisMaze.exitPoint;

                    Debug.Log("Current Location : " + currLocation.x + " " + currLocation.z);
                    Debug.Log("Exit Location : " + exitLocation.x + " " + exitLocation.z);
                    destination = aStar.Build(thisMaze, currLocation, exitLocation);

                    magic = Instantiate(particles, this.gameObject.transform.position, this.gameObject.transform.rotation);
                    StopCoroutine(DisplayMagicPath());
                    StartCoroutine(DisplayMagicPath());

                    magicPathBar.size = 0;
                    timePassed = 0;
                    magicPathHandle.color = Color.yellow;
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
