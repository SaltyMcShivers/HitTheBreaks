using UnityEngine;
using System.Collections;

public class SectionScript : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "SpawnTrigger")
        {
            col.transform.root.GetComponent<WorldManager>().SpawnSection();
        }
        if (col.gameObject.tag == "DeathTrigger")
        {
            col.transform.root.GetComponent<WorldManager>().RemoveSection(this.transform.parent.gameObject);
        }
    }
}
