using System.Collections;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private int currWaypointIndex;
    private bool isWaiting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waypoints = new Transform[waypointParent.childCount];

        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseController.IsGamePaused || isWaiting)
        {
            return;
        }

        // Move to waypoint
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currWaypointIndex];

        transform.position = Vector3.MoveTowards(transform.position, target.position, (moveSpeed * Time.deltaTime)/2);

        if (Vector3.Distance(transform.position, target.position) < 1.0f)
        {
            // Start waiting at waypoint
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        // if looping --> increment 1 and wrap to beginning if needed
        // if not looping --> increment curr Index but don't need to exceed last waypoint
        currWaypointIndex = loopWaypoints ? (currWaypointIndex + 1) % waypoints.Length : Mathf.Min(currWaypointIndex + 1, waypoints.Length - 1);

        isWaiting = false;
    }
}
