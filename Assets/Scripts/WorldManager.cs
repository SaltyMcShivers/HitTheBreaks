using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {
    public float defaultMoveSpeed;
    public float slowDownRate;
    public float speedUpRate;

    public GameObject sectionPrefab;

    public GameObject entryObject;

    public Transform travelItem;

    public float currentSpeed;

    bool gameOver;

	// Use this for initialization
	void Start () {
        currentSpeed = 0;
        Messenger.AddListener("GameOver", StopMovement);
        SpawnSection();
	}

    void Update()
    {
        if (gameOver) return;
        FindSpeed();
        travelItem.position -= new Vector3(0, currentSpeed * Time.deltaTime, 0);
    }

    void StopMovement()
    {
        gameOver = true;
    }
	
	// Update is called once per frame
	void FindSpeed () {
        if (Input.GetKey(KeyCode.Space))
        {
            if (currentSpeed == 0f) return;
            currentSpeed = Mathf.Max(currentSpeed - slowDownRate * Time.deltaTime, 0);
        }
        else
        {
            if (currentSpeed == defaultMoveSpeed) return;
            currentSpeed = Mathf.Min(currentSpeed + speedUpRate * Time.deltaTime, defaultMoveSpeed);
        }
	}

    public void SpawnSection()
    {
        GameObject newSection = Instantiate(sectionPrefab, entryObject.transform.position, Quaternion.identity) as GameObject;
        newSection.transform.SetParent(travelItem);
    }

    public void RemoveSection(GameObject go)
    {
        Destroy(go);
    }
}
