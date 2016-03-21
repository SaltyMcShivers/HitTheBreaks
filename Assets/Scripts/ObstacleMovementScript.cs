using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleMovementScript : MonoBehaviour {
    public Transform obstacle;
    public List<Vector3> waypoints;
    public float baseSpeed;
    public float baseWaitTime;

    int currentTarget;
    float startedWaitTime;
	// Use this for initialization
	void Start () {
        float randomPos = Random.Range(0f, 1f);
        int prevTarget = Mathf.FloorToInt(randomPos * waypoints.Count);
        currentTarget = (prevTarget + 1) % waypoints.Count;
        obstacle.localPosition = Vector3.Lerp(waypoints[prevTarget], waypoints[currentTarget], randomPos % (1f / waypoints.Count));
        //startedWaitTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (waypoints[currentTarget] == obstacle.localPosition)
        {
            if (Time.time - startedWaitTime > baseWaitTime)
            {
                if (currentTarget == waypoints.Count - 1) currentTarget = 0;
                else currentTarget++;
            }
            else return;
        }
        float currentDistance = Vector3.Distance(obstacle.localPosition, waypoints[currentTarget]);
        if (currentDistance <= baseSpeed * Time.deltaTime)
        {
            startedWaitTime = Time.time;
            obstacle.localPosition = waypoints[currentTarget];
        }
        else
        {
            obstacle.localPosition += Vector3.Normalize(waypoints[currentTarget] - obstacle.localPosition) * baseSpeed * Time.deltaTime;
        }
	}
}
