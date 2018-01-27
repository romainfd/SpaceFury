using UnityEngine;
using System.Collections;

public class ExplosionRock : MonoBehaviour
{
    private bool triggered = false;
    public float size;
    public float TShine;
    public float TLife;
    private Vector3 scaleInit;
    private float t0;
    public float TExplosion;

    void Start()
    {
        scaleInit = GetComponent<Transform>().localScale;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {// eu 1er déclenchement ET si joueur OU enemy
        if (!triggered && coll.gameObject.tag == "Player") // au 1er délenchement
        {
            triggered = true;
            t0 = Time.time;
        }
    }

    void OnCollisionEnter2D()
    {
        Destroy(gameObject);
    }

    void Shine(float size, float t0, float TShine, Vector3 scaleInit, Transform transform)
    {
        transform.localScale = scaleInit *  (1 + size / 100 * Mathf.Sin(2 * Mathf.PI * ((Time.time - t0 - TShine/8) / TShine)))/ (1 + size / 100 * Mathf.Sin(- Mathf.PI / 4)); // Thsine/8 déphase pour grandir plus que rétrécit
    }

    void Update()
    {
        if (triggered)
        {
            Shine(size, t0, TShine, scaleInit, GetComponent<Transform>());
            if (Time.time - t0 > TLife)  // explosion finale
            {
                transform.localScale = scaleInit * (1 + size / 100)/(1+size/100 * Mathf.Sin(-Mathf.PI /4));
                if (Time.time - t0 - TLife > TExplosion)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
