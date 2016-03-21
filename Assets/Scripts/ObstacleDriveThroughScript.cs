using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleDriveThroughScript : MonoBehaviour {
    public GameObject obstaclePrefab;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float baseSpeed;
    public float spawnTime;

    float lastSpawnTime;
    List<GameObject> obstacles;

	// Use this for initialization
	void Start () {
        if (Random.Range(0f, 1f) < 0.5f)
        {
            Vector3 temp = startPoint;
            startPoint = endPoint;
            endPoint = temp;
        }
        SetUpStart();
	}
	
	// Update is called once per frame
	void Update () {
        GameObject toRemove = null;
        foreach (GameObject obj in obstacles)
        {
            if (Vector3.Distance(obj.transform.localPosition, endPoint) < baseSpeed * Time.deltaTime)
            {
                toRemove = obj;
            }
            else
            {
                obj.transform.localPosition += Vector3.Normalize(endPoint - startPoint) * baseSpeed * Time.deltaTime;
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
        float travelBetweenSpawn = baseSpeed * spawnTime / Vector3.Distance(startPoint, endPoint);
        float rando = Random.Range(0f, travelBetweenSpawn);
        float startDelay = Mathf.Lerp(spawnTime, 0f, rando / travelBetweenSpawn);
        while (rando < 1.0f)
        {
            GameObject newObstacle = Instantiate(obstaclePrefab, transform.position, Quaternion.identity) as GameObject;
            newObstacle.transform.SetParent(transform);
            newObstacle.transform.localPosition = Vector3.Lerp(startPoint, endPoint, rando);
            obstacles.Add(newObstacle);
            rando += travelBetweenSpawn;
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
        obstacles.Add(newObstacle);
    }
}
