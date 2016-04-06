using UnityEngine;
using System.Collections;

public class SectionSpawnScript : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "SpawnTrigger")
        {
            if (tag == "BackgroundTag")
            {
                col.transform.root.GetComponent<WorldManager>().SpawnBackground();
            }
            else col.transform.root.GetComponent<WorldManager>().SpawnSection();
        }
        if (col.gameObject.tag == "DeathTrigger")
        {
            col.transform.root.GetComponent<WorldManager>().RemoveSection(this.transform.parent.gameObject);
        }
    }
}
