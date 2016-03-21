using UnityEngine;
using System.Collections;

public class PlayerCollisionScript : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D col)
    {
        Messenger.Invoke("GameOver");
        Destroy(transform.parent.gameObject);
    }
}
