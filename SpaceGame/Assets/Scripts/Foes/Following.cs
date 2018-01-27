using UnityEngine;
using System.Collections;
using System;

public class Following : MonoBehaviour {
    public float v;  // vitesse vaisseau = la notre dilatée par v

    private int index; // rang de la position
    private bool following;
    private GameObject[] Clouds;
    private float lambda;
    private Vector3 translation;
 
	void Start ()
    {
        following = false;
        Clouds = PlayerSmoke.GetClouds();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (following)
        {
            if (lambda > 1)
            {
                index = (index + 1)%(Clouds.Length);
                lambda = 0;
                GetComponent<Transform>().position = Clouds[index].GetComponent<Transform>().position;
                translation = Clouds[(index + 1) % Clouds.Length].GetComponent<Transform>().position - Clouds[index].GetComponent<Transform>().position;
            }
            float dLambda = v * Time.deltaTime / PlayerSmoke.Dt; // on multiplie par la vitesse !
            Orientation.Move(gameObject, translation * dLambda);
            lambda += dLambda;

            // index = sur quel nuage on est 
            // PlayerSmoke.m = sur quel nuage est le joueur sans modulo !
            if (index == PlayerSmoke.m % Clouds.Length) // il perd notre trace => va rendre compte
            {
                following = false;
                // que f ait-il quand il nous a perdu ?!
                Destroy(gameObject);
            }
        }

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!following && coll.gameObject.tag == "Cloud") { OnContact(coll.gameObject);}  // quand il tient une cible, il ne la lache pas
    }

    public void OnContact(GameObject cloud)
    {
        GetComponent<SnowRock>().enabled = false;
        following = true;
        GetComponent<Transform>().position = cloud.GetComponent<Transform>().position;
        index = Array.IndexOf(Clouds, cloud);
        lambda = 0;
        translation = Clouds[(index + 1) % Clouds.Length].GetComponent<Transform>().position - Clouds[index].GetComponent<Transform>().position;
    }

    public bool GetFollowing() { return following; }
}

