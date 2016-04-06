using UnityEngine;
using System.Collections;

public class SectionScript : MonoBehaviour {
    public Transform spawner;

    public void ChangeSpawnDistance(float dist)
    {
        spawner.localPosition = Vector3.up * dist;
    }
}
