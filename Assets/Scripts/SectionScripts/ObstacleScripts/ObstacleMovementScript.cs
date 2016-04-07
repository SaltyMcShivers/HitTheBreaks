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

    //public Transform pointer;

    public GameObject blockPrefab;
    public float blockMovementSpeed;
    public float transitionWaitTime;
    public Animator arrowAnim;

    float startingPosition;
    GameObject enteringBlock;
    GameObject exitingBlock;

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

            float legTime = (Vector3.Distance(waypoints[0], waypoints[1]) / speed + waitTime);
            float totalTravelTime = legTime * waypoints.Count;
            float finalPos = randomPos * totalTravelTime % legTime;
            if (finalPos < legTime - waitTime)
            {
                obstacle.localPosition = Vector3.Lerp(waypoints[prevTarget], waypoints[currentTarget], finalPos / (legTime - waitTime));
            }
            else
            {
                obstacle.localPosition = waypoints[currentTarget];
                startedWaitTime = Time.time - legTime + finalPos;
            }
        }

        startingPosition = waypoints[0].x - blockMovementSpeed * (waitTime * 2f + Vector3.Distance(waypoints[0], waypoints[1]) / speed - transitionWaitTime);
        exitingBlock = Instantiate(blockPrefab) as GameObject;
        exitingBlock.transform.SetParent(transform.parent);
        exitingBlock.transform.localPosition = Vector3.right * 10f;
        enteringBlock = Instantiate(blockPrefab) as GameObject;
        enteringBlock.transform.SetParent(transform.parent);
        float timeToCheckpoint = 0f;
        if (currentTarget == 1)
        {
            //Waiting at Left
            if (obstacle.transform.localPosition == waypoints[1])
            {
                timeToCheckpoint = 2f * waitTime - Time.time + startedWaitTime + Vector3.Distance(waypoints[0], waypoints[1]) / speed;
            //Moving to Right
            }else{
                enteringBlock.transform.localPosition = obstacle.transform.localPosition;
                return;
            }
        }
        else if (currentTarget == 0)
        {
            //Waiting at Right
            if (obstacle.transform.localPosition == waypoints[0])
            {
                timeToCheckpoint = waitTime - Time.time + startedWaitTime;
            }
            //Moving to Left
            else
            {
                timeToCheckpoint = waitTime  + Vector3.Distance(waypoints[0], obstacle.transform.localPosition) / speed;
            }
        }

        enteringBlock.transform.localPosition = Vector3.left * (Mathf.Abs(waypoints[0].x) + blockMovementSpeed * timeToCheckpoint);

        //pointer.eulerAngles = new Vector3(0f, 0f, Vector3.Angle(Vector3.right, obstacle.transform.localPosition - waypoints[currentTarget]));
    }
	
	// Update is called once per frame
	void Update () {
        if(exitingBlock != null) exitingBlock.transform.position += Vector3.right * blockMovementSpeed * Time.deltaTime;
        if (waypoints[currentTarget] == obstacle.localPosition)
        {
            if (Time.time - startedWaitTime > waitTime)
            {
                if (currentTarget == waypoints.Count - 1) currentTarget = 0;
                else currentTarget++;
                enteringBlock.transform.position += Vector3.right * blockMovementSpeed * Time.deltaTime;
            }
            else
            {
                if (Vector3.Distance(obstacle.transform.position, enteringBlock.transform.position) < blockMovementSpeed * Time.deltaTime) enteringBlock.transform.localPosition = obstacle.transform.localPosition;
                else enteringBlock.transform.localPosition += Vector3.right * blockMovementSpeed * Time.deltaTime;

                arrowAnim.SetFloat("TimeLeft", 1f - (Time.time - startedWaitTime) / waitTime);
                return;
            }
        }
        float currentDistance = Vector3.Distance(obstacle.localPosition, waypoints[currentTarget]);
        if (currentDistance <= baseSpeed * Time.deltaTime)
        {
            arrowAnim.SetFloat("TimeLeft", 1.0f);
            arrowAnim.SetBool("OnRight", currentTarget == 1);
            startedWaitTime = Time.time;
            obstacle.localPosition = waypoints[currentTarget];
            enteringBlock.transform.position += Vector3.right * blockMovementSpeed * Time.deltaTime;
            if (currentTarget == 1)
            {
                Destroy(exitingBlock);
                exitingBlock = enteringBlock;
                enteringBlock = Instantiate(blockPrefab) as GameObject;
                enteringBlock.transform.SetParent(transform);
                enteringBlock.transform.localPosition = Vector3.right * startingPosition;
            }
        }
        else
        {
            obstacle.localPosition += Vector3.Normalize(waypoints[currentTarget] - obstacle.localPosition) * baseSpeed * Time.deltaTime;
            if (currentTarget == 1) enteringBlock.transform.localPosition = obstacle.localPosition;
            else enteringBlock.transform.position += Vector3.right * blockMovementSpeed * Time.deltaTime;
        }
	}
}
