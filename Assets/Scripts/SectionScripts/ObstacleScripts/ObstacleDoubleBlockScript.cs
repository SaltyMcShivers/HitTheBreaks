using UnityEngine;
using System.Collections;

public class ObstacleDoubleBlockScript : ObstacleScript
{
    public GameObject obstaclePrefab;
    public Transform spawnLocation;
    // Use this for initialization
    void Start()
    {
        SpawnBlock();
    }

    void SpawnBlock()
    {
        GameObject legalLanes = GameObject.FindGameObjectWithTag("LaneTag");
        float gapPosition = legalLanes.transform.GetChild(Mathf.FloorToInt(Random.Range(0f, legalLanes.transform.childCount))).position.x;
        if(gapPosition != 0f && gapPosition == GameObject.FindGameObjectWithTag("Player").transform.position.x * -1f){
            gapPosition = 0f;
        }
        foreach(Transform t in legalLanes.GetComponentInChildren<Transform>())
        {
            if (t.position.x == gapPosition) continue;
            Vector3 newLocation = new Vector3(t.position.x, spawnLocation.position.y, 0f);
            GameObject obs = Instantiate(obstaclePrefab, newLocation, Quaternion.identity) as GameObject;
            obs.transform.SetParent(spawnLocation);
        }
    }
}
