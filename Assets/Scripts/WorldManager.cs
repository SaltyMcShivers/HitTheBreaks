using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public struct DifficultyElement
{
    public float revampDifficulty;
    public float minSpawnDistance;
    public float maxSpawnDistance;
    public float blockIncrement;
    public float blockReset;
}

public class WorldManager : MonoBehaviour {
    public GameObject playerPrefab;
    public Vector3 playerSpawnLocation;

    public GameObject sectionPrefab;

    public List<GameObject> sectionPrefabs;

    public GameObject entryObject;
    public GameObject exitObject;

    public Transform travelItem;

    public Text distanceDisplay;
    public GameObject replayButton;

    public GameObject backgroundPrefab;

    public PlayerBreaksControl brakes;

    public float minSpawnDistance;
    public float maxSpawnDistance;

    public List<DifficultyElement> challengeTeirs;

    public float defaultBlockChance;

    public Renderer roadRenderer;

    public float spawnInterval;
    public float spawnOffset;

    float roadOffsetFactor;

    bool gameOver = true;
    float blockChance;
    Material roadMaterial;

    // Use this for initialization
    void Start () {
        roadMaterial = roadRenderer.material;
        roadOffsetFactor = roadMaterial.GetTextureScale("_MainTex").y * 0.1f;
        //PrePopulateBackground();
        Messenger.AddListener("GameOver", StopMovement);
        //ResetGame();
	}

    void Update()
    {
        if (gameOver) return;
        float currentSpeed = brakes.GetCurrentSpeed();
        travelItem.position -= new Vector3(0, currentSpeed * Time.deltaTime, 0);

        roadMaterial.SetTextureOffset("_MainTex", roadMaterial.GetTextureOffset("_MainTex") + Vector2.down * currentSpeed * roadOffsetFactor * Time.deltaTime);
        DisplayDistance();
    }

    void StopMovement()
    {
        gameOver = true;
        brakes.StopMovement();
        DisplayDistance();
        replayButton.SetActive(true);
    }

    public void PrePopulateBackground()
    {
        Vector3 spawnPos = entryObject.transform.position;
        while (spawnPos.y > exitObject.transform.position.y)
        {
            GameObject go = Instantiate(backgroundPrefab, spawnPos, Quaternion.identity) as GameObject;
            go.transform.SetParent(travelItem);
            spawnPos += Vector3.down * (go.GetComponentInChildren<BoxCollider2D>().transform.localPosition.y - 0.5f * (go.GetComponentInChildren<BoxCollider2D>().transform.localScale.y + entryObject.transform.localScale.y));
        }
    }

    public void SpawnBackground()
    {
        GameObject newSection = Instantiate(backgroundPrefab, entryObject.transform.position, Quaternion.identity) as GameObject;
        newSection.transform.SetParent(travelItem);
    }

    public void SpawnSection()
    {
        GameObject chosenPrefab = sectionPrefabs[Mathf.FloorToInt(Random.Range(0, sectionPrefabs.Count))];
        GameObject newSection = Instantiate(chosenPrefab, entryObject.transform.position, Quaternion.identity) as GameObject;
        newSection.transform.SetParent(travelItem);
        float roundedPosition = (newSection.transform.localPosition.y - spawnOffset) % spawnInterval;
        newSection.transform.localPosition += Vector3.down * roundedPosition;
        int currentChallenge = 0;
        while(currentChallenge < challengeTeirs.Count && challengeTeirs[currentChallenge].revampDifficulty < Mathf.Abs(travelItem.transform.localPosition.y)){
            currentChallenge++;
        }
        currentChallenge = Mathf.Min(currentChallenge, challengeTeirs.Count - 1);
        newSection.GetComponent<SectionScript>().ChangeSpawnDistance(Random.Range(challengeTeirs[currentChallenge].minSpawnDistance, challengeTeirs[currentChallenge].maxSpawnDistance));
        if(Random.Range(0f, 1f) < blockChance)
        {
            blockChance *= challengeTeirs[currentChallenge].blockIncrement;
        }
        else
        {
            newSection.GetComponentInChildren<ObstacleScript>().BlockPlayer();
            blockChance *= challengeTeirs[currentChallenge].blockReset;
        }
    }

    public void RemoveSection(GameObject go)
    {
        Destroy(go);
    }

    void DisplayDistance()
    {
        distanceDisplay.text = (-1 * travelItem.transform.localPosition.y).ToString("F1") + "m";

    }

    public void ResetGame()
    {
        while(travelItem.childCount > 0)
        {
            DestroyImmediate(travelItem.GetChild(0).gameObject);
        }
        replayButton.SetActive(false);
        travelItem.transform.localPosition = Vector3.zero;
        gameOver = false;
        brakes.RestartPlayer();
        DisplayDistance();
        SpawnSection();
        //PrePopulateBackground();
        blockChance = defaultBlockChance;
        roadMaterial.SetTextureOffset("_MainTex", Vector2.zero);
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}
