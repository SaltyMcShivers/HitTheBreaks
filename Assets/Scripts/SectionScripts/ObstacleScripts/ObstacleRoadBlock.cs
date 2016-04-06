using UnityEngine;
using System.Collections;

public class ObstacleRoadBlock : ObstacleScript{
    public GameObject obstaclePrefab;
    public Transform spawnLocation;

    public float directBlockChance;
	// Use this for initialization
	void Start () {
        SpawnBlock();
    }

    void SpawnBlock()
    {
        GameObject legalLanes = GameObject.FindGameObjectWithTag("LaneTag");
        //Block the player
        Vector3 newLocation;
        if (obstructPlayer)
        {
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            int closestPosition = 0;
            float closestDistance = 0;
            for(int i = 0; i < legalLanes.transform.childCount; i++)
            {
                if(i == 0)
                {
                    closestDistance = Vector3.Distance(legalLanes.transform.GetChild(0).position, playerPos);
                }
                else
                {
                    if(Vector3.Distance(legalLanes.transform.GetChild(i).position, playerPos) < closestDistance)
                    {
                        closestPosition = i;
                        closestDistance = Vector3.Distance(legalLanes.transform.GetChild(i).position, playerPos);
                    }
                }
            }
            
            newLocation = new Vector3(legalLanes.transform.GetChild(closestPosition).position.x, spawnLocation.position.y, 0f);
        }else{
            newLocation = new Vector3(legalLanes.transform.GetChild(Mathf.FloorToInt(Random.Range(0f, legalLanes.transform.childCount))).position.x, spawnLocation.position.y, 0f);
        }
        GameObject obs = Instantiate(obstaclePrefab, newLocation, Quaternion.identity) as GameObject;
        obs.transform.SetParent(spawnLocation);
    }
}
