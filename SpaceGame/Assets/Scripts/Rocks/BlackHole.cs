using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {
    public float m;
    private Vector3 OM3;
    private Vector2 OM;
    private float norme;
    GameObject player;
    private Vector2 formerForce; 

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        OM3 = player.GetComponent<Transform>().position - gameObject.GetComponent<Transform>().position;
        OM = new Vector2(OM3.x, OM3.y);
        norme = Mathf.Sqrt(Mathf.Pow(OM.x, 2) + Mathf.Pow(OM.y, 2));
        formerForce = -player.GetComponent<Rigidbody2D>().mass * m * OM / Mathf.Pow(norme, 3);
    }
    void FixedUpdate ()
    {
        // pas faire bouger avant le début
        if (StartZoom.GetBegin())
        {
            player.GetComponent<Rigidbody2D>().AddForce(-formerForce);  // on enlève la force d'avant pour l'actualiser :
            OM3 = player.GetComponent<Transform>().position - gameObject.GetComponent<Transform>().position;
            OM = new Vector2(OM3.x, OM3.y);
            norme = Mathf.Sqrt(Mathf.Pow(OM.x, 2) + Mathf.Pow(OM.y, 2));
            formerForce = -player.GetComponent<Rigidbody2D>().mass * m * OM / Mathf.Pow(norme, 3);
            player.GetComponent<Rigidbody2D>().AddForce(formerForce);
        }
    }
}
