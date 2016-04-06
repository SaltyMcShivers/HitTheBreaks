using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleDriveThroughScript : ObstacleScript {
    public GameObject obstaclePrefab;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float baseSpeed;
    public float spawnTime;
    public float speedDeviation;

    public float directBlockChance;

    public Transform arrowsContainer;

    float blockSpeed;
    float lastSpawnTime;
    List<GameObject> obstacles;
    float obstacleRoatation;
    
	void Start () {
        if (Random.Range(0f, 1f) < 0.5f)
        {
            Vector3 temp = startPoint;
            startPoint = endPoint;
            endPoint = temp;
        }
        obstacleRoatation = 90f - (90f * Mathf.Sign(endPoint.x - startPoint.x));
        arrowsContainer.localScale = new Vector3(Mathf.Sign(endPoint.x - startPoint.x), 1f);
        blockSpeed = Mathf.Lerp(baseSpeed - speedDeviation, baseSpeed + speedDeviation, Random.Range(0f, 1f));
        SetUpStart();
	}
	
	void Update () {
        GameObject toRemove = null;
        foreach (GameObject obj in obstacles)
        {
            if (Vector3.Distance(obj.transform.localPosition, endPoint) < blockSpeed * Time.deltaTime)
            {
                toRemove = obj;
            }
            else
            {
                obj.transform.localPosition += Vector3.Normalize(endPoint - startPoint) * blockSpeed * Time.deltaTime;
            }
        }
        if (toRemove != null)
        {
            obstacles.Remove(toRemove);
            Destroy(toRemove);
        }
	}

    void SetUpStart()
    {
        obstacles = new List<GameObject>();
        float travelBetweenSpawn = blockSpeed * spawnTime / Vector3.Distance(startPoint, endPoint);
        float offset;
        if (obstructPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerBreaksControl pbScript = player.GetComponent<PlayerBreaksControl>();
            //How long does it take to travel from the player's current position to the obstacle's current position?
            float playerTravelTime = Mathf.Abs(transform.position.y - player.transform.position.y) / pbScript.defaultMoveSpeed;
            float finalXPos = player.transform.position.x - playerTravelTime * blockSpeed * Mathf.Sign(-startPoint.x);
            while(Mathf.Abs(finalXPos) > Mathf.Abs(startPoint.x))
            {
                finalXPos += travelBetweenSpawn * Vector3.Distance(startPoint, endPoint) * Mathf.Sign(-startPoint.x);
            }
            offset = Mathf.InverseLerp(startPoint.x, endPoint.x, finalXPos) % travelBetweenSpawn;
        }
        else
        {
            offset = Random.Range(0f, travelBetweenSpawn);
        }
        float startDelay = Mathf.Lerp(spawnTime, 0f, offset / travelBetweenSpawn);
        while (offset < 1.0f)
        {
            GameObject newObstacle = Instantiate(obstaclePrefab, transform.position, Quaternion.identity) as GameObject;
            newObstacle.transform.SetParent(transform);
            newObstacle.transform.localPosition = Vector3.Lerp(startPoint, endPoint, offset);
            newObstacle.transform.Rotate(Vector3.back, obstacleRoatation);
            obstacles.Add(newObstacle);
            offset += travelBetweenSpawn;
        }
        StartCoroutine(SpawnCoroutine(startDelay));
    }

    IEnumerator SpawnCoroutine(float f)
    {
        yield return new WaitForSeconds(f);
        SpawnAtStart();
        StartCoroutine(SpawnCoroutine(spawnTime));
    }



    void SpawnAtStart()
    {
        GameObject newObstacle = Instantiate(obstaclePrefab, transform.position, Quaternion.identity) as GameObject;
        newObstacle.transform.SetParent(transform);
        newObstacle.transform.localPosition = startPoint;
        newObstacle.transform.Rotate(Vector3.back, obstacleRoatation);
        obstacles.Add(newObstacle);
    }
}
