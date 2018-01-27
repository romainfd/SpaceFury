using UnityEngine;
using System.Collections;

public class AsymptoteFix : MonoBehaviour {
    public float v;   // coeff de vPlayer
    public float s;  // sensibility

    private Vector3 direction;
    private bool tracking;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        tracking = false;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!Editor.editing)
        {
            if (coll.gameObject.tag == "Player")
            {
                tracking = true;
                direction = Direction().normalized;
            }
        }
    }
	
    Vector3 Direction() { return player.GetComponent<Transform>().position - GetComponent<Transform>().position; }

    void Update ()
    {
	    if (tracking && !GameManager.GetPaused() && StartZoom.GetBegin())
        {
            Vector3 currentDirection = Direction().normalized;
            Orientation.Move(gameObject, direction * GameObject.FindWithTag("Player").GetComponent<PadMove>().speed *v * Time.deltaTime); 
            direction = (1 - s) * direction + s * currentDirection;
        }
	}
}
