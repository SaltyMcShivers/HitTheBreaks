using UnityEngine;
using System.Collections;

public class ObstacleScript : MonoBehaviour {
    protected bool obstructPlayer { get; set; }

	public void BlockPlayer()
    {
        obstructPlayer = true;
    }
}
