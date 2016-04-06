using UnityEngine;
using System.Collections.Generic;

public class PlayerCollisionScript : MonoBehaviour {

    public List<GameObject> itemsToDeactivate;
    
    void OnCollisionEnter2D(Collision2D col)
    {
        Messenger.Invoke("GameOver");
        foreach (GameObject go in itemsToDeactivate)
        {
            go.SetActive(false);
        }
    }
}
