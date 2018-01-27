using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Harpon : MonoBehaviour {
    public GameObject harponEdge;
    public GameObject lifeBar;
    public float hpMax;

    public GameObject harponer;
    public bool rearming;

    private float hp;
    private Vector3 initScale;

    void Start ()
    {
        lifeBar.SetActive(false);
        rearming = false;
        initScale = lifeBar.GetComponent<Transform>().localScale;
        hp = hpMax;
	}
	
	void Update ()
    {
	    if (Input.GetMouseButtonUp(0) && PadMove.GetCaught())
        {
            hp -= 1;
            lifeBar.GetComponent<Transform>().localScale = initScale - new Vector3(0, initScale.y *(1-hp/hpMax), 0);
        }
        if (hp <= 0 && !rearming)
        {
            Rearm();
        }
	}

    void Rearm()
    {
        harponer.GetComponent<Transform>().rotation = Quaternion.identity;
        lifeBar.GetComponent<Transform>().localScale = initScale;
        harponer.GetComponent<Harponer>().CreateHarpon();
        PadMove.SetCaught(false);    // dans tous les cas, on s'en fout
        rearming = true;
        hp = hpMax;
        harponer.GetComponent<Harponer>().t0 = Time.time;
        gameObject.SetActive(false);
    }

    public void RotateHarpon(Vector3 direction)
    {
        harponEdge.GetComponent<Transform>().Rotate(new Vector3(0, 0, Vector3.Angle(new Vector3(0, 1, 0), direction) * ((direction.x < 0) ? 1f : -1f)));
    }

    void OnTriggerEnter2D(Collider2D coll)  // le harpon touche qqchose
    {
        if ((new List<string> { "Player", "Foe", "FollowingFoe" }).Contains(coll.gameObject.tag) && !harponer.GetComponent<Harponer>().caught)
        {
            if (!(coll.gameObject.tag == "FollowingFoe" && coll.isTrigger)) // ce n'est pas le trigger de l'asymptote 
            {
                lifeBar.SetActive(true);
                harponer.GetComponent<Harponer>().Harponed(coll.gameObject);
            }
        }
        string colliderTag = coll.gameObject.tag;
        if (colliderTag == "Path")
            Rearm();
        else if (colliderTag == "SnowRock")
            Rearm();
        else if (colliderTag == "MagmaRock")
            Rearm();
        else if (colliderTag == "ExplosionRock")
            Rearm();
        else if (colliderTag == "BlackHole")
            Rearm();
    }
}

