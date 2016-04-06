using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleMovementScript : ObstacleScript {
    public Transform obstacle;
    public List<Vector3> waypoints;
    public float baseSpeed;
    public float baseWaitTime;

    public float speedDeviation;
    public float waitDeviation;

    public float directBlockChance;

    public Transform pointer;

    float speed;
    float waitTime;

    int currentTarget;
    float startedWaitTime;

	// Use this for initialization
	void Start () {
        float rando = Random.Range(0f, 1f);
        speed = Mathf.Lerp(baseSpeed - speedDeviation, baseSpeed + speedDeviation, rando);
        waitTime = Mathf.Lerp(baseWaitTime - waitDeviation, baseWaitTime + waitDeviation, rando);
        StartSetUp();
	}

    void StartSetUp()
    {

        //Block the player
        if (obstructPlayer)
        {
            currentTarget = 1;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerBreaksControl pbScript = player.GetComponent<PlayerBreaksControl>();
            //How long does it take to travel from the player's current position to the obstacle's current position?
            float playerTravelTime = Mathf.Abs(transform.position.y - player.transform.position.y) / pbScript.defaultMoveSpeed;
            //How much time passes between when the blocks starts to leave one waypoint and when it leaves the next waypoint
            float legTime = Vector3.Distance(waypoints[0], waypoints[1]) / speed + waitTime;
            //Eliminate loop
            float offsetTime = playerTravelTime % legTime;

            //Find wether or not player should be waiting
            float timeLeft = offsetTime;
            float travelTime = Vector3.Distance(waypoints[currentTarget], new Vector3(player.transform.position.x, 0f)) / speed;
            bool isWaiting = true;
            bool flipPos = false;
            do
            {
                isWaiting = !isWaiting;
                if (isWaiting)
                {
                    timeLeft -= travelTime;
                    travelTime = legTime - waitTime;
                    flipPos = !flipPos;
                }
                else
                {
                    timeLeft -= baseWaitTime;
                }
            } while (timeLeft >= 0);

            //In the middle of waiting
            if (isWaiting)
            {
                startedWaitTime = Time.time + timeLeft;
            }
            else
            {
                if (flipPos)
                {
                    obstacle.localPosition = Vector3.Lerp(waypoints[currentTarget], waypoints[1 - currentTarget], Mathf.Abs(timeLeft) / travelTime);
                    currentTarget = 0;
                }
                else
                {
                    obstacle.localPosition = Vector3.Lerp(waypoints[1 - currentTarget], waypoints[currentTarget], Mathf.Abs(timeLeft) / travelTime);
                }
            }
        }
        else
        {
            float randomPos = Random.Range(0f, 1f);
            int prevTarget = Mathf.FloorToInt(randomPos * waypoints.Count);
            currentTarget = (prevTarget + 1) % waypoints.Count;

            float legTime = (Vector3.Distance(waypoints[0], waypoints[1]) / baseSpeed + baseWaitTime);
            float totalTravelTime = legTime * waypoints.Count;
            float finalPos = randomPos * totalTravelTime % legTime;
            if (finalPos < legTime - baseWaitTime)
            {
                obstacle.localPosition = Vector3.Lerp(waypoints[prevTarget], waypoints[currentTarget], finalPos / (legTime - baseWaitTime));
            }
            else
            {
                obstacle.localPosition = waypoints[currentTarget];
                startedWaitTime = Time.time - legTime + finalPos;
            }
        }
        pointer.eulerAngles = new Vector3(0f, 0f, Vector3.Angle(Vector3.right, obstacle.transform.localPosition - waypoints[currentTarget]));
    }
	
	// Update is called once per frame
	void Update () {
        if (waypoints[currentTarget] == obstacle.localPosition)
        {
            if (Time.time - startedWaitTime > waitTime)
            {
                if (currentTarget == waypoints.Count - 1) currentTarget = 0;
                else currentTarget++;
                pointer.eulerAngles = new Vector3(0f, 0f, Vector3.Angle(Vector3.right, obstacle.transform.localPosition - waypoints[currentTarget]));
            }
            else
            {
                pointer.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(Vector3.Angle(Vector3.right, waypoints[1 - currentTarget] - waypoints[currentTarget]), Vector3.Angle(Vector3.right, waypoints[currentTarget] - waypoints[1 - currentTarget]), (Time.time - startedWaitTime) / waitTime));
                return;
            }
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
