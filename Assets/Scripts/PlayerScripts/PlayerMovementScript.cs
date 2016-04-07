using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMovementScript : MonoBehaviour {
    public float shiftTime;

    public List<Transform> lanes;
    public int startLane;

    public float minShiftRatio;

    public float minSwipeSpeed;

    public ParticleSystem leftJet;
    public ParticleSystem rightJet;

    PlayerBreaksControl brakes;

    float averageMovementSpeed;
    float laneDistance;
    int targetLane;

    void Start () {
        lanes = new List<Transform>();
        GameObject legalLanes = GameObject.FindGameObjectWithTag("LaneTag");
        for (int i = 0; i < legalLanes.transform.childCount; i++)
        {
            lanes.Add(legalLanes.transform.GetChild(i));
        }
        laneDistance = Vector3.Distance(lanes[0].transform.position, lanes[1].transform.position);
        averageMovementSpeed = laneDistance / shiftTime;
        brakes = GetComponent<PlayerBreaksControl>();

        Messenger.AddListener("GameOver", StopMovement);
        ResetPlayer();
    }
	
	void Update () {
        if (brakes.IsGameOver()) return;
        if (IsShifting())
        {
            transform.position += FindShiftSpeed() * Vector3.right;
        }
        else
        {
            foreach(Touch t in Input.touches)
            {
                if(t.phase == TouchPhase.Moved)
                {
                    if(Mathf.Abs(t.deltaPosition.x) > minSwipeSpeed * Time.deltaTime)
                    {
                        if (t.deltaPosition.x < 0)
                        {
                            int currentLane = Mathf.RoundToInt(transform.position.x / laneDistance) + 1;
                            if (currentLane == 0) return;
                            targetLane = currentLane - 1;
                            leftJet.Play();
                            StartCoroutine("StartLaneShift");
                            return;
                        }
                        else if (t.deltaPosition.x > 0)
                        {
                            int currentLane = Mathf.RoundToInt(transform.position.x / laneDistance) + 1;
                            if (currentLane == lanes.Count - 1) return;
                            targetLane = currentLane + 1;
                            rightJet.Play();
                            StartCoroutine("StartLaneShift");
                            return;
                        }
                    }
                }
            }

            /*
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                int currentLane = Mathf.RoundToInt(transform.position.x / laneDistance) + 1;
                if (currentLane == 0) return;
                targetLane = currentLane - 1;
                StartCoroutine("StartLaneShift");
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                int currentLane = Mathf.RoundToInt(transform.position.x / laneDistance) + 1;
                if (currentLane == lanes.Count - 1) return;
                targetLane = currentLane + 1;
                StartCoroutine("StartLaneShift");
            }
            */

        }
    }

    void StopMovement()
    {
        StopAllCoroutines();
    }

    IEnumerator StartLaneShift()
    {
        transform.position += FindShiftSpeed() * Vector3.right;
        yield return new WaitForSeconds(shiftTime);
        transform.position = new Vector3(lanes[targetLane].localPosition.x, transform.position.y);
    }

    void ResetPlayer()
    {
        //transform.position = new Vector3(lanes[startLane].position.x, transform.position.y);
        targetLane = Mathf.CeilToInt(lanes.Count);
    }

    float FindShiftSpeed()
    {
        float positionFactor = Mathf.PingPong(transform.position.x, laneDistance * 0.5f) * 2 / laneDistance;
        float speedFactor = Mathf.Lerp(minShiftRatio, 2f - minShiftRatio * .25f, positionFactor);
        return Mathf.Sign(lanes[targetLane].position.x - transform.position.x) * speedFactor * averageMovementSpeed * Time.deltaTime;
    }

    public bool IsShifting()
    {
        return transform.position.x % laneDistance != 0f;
    }

    public float ShiftProgress()
    {
        return Vector3.Distance(transform.position, lanes[targetLane].transform.position) % laneDistance;
    }
}
