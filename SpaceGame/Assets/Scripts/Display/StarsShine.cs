using UnityEngine;
using System.Collections.Generic;
using System;

public class StarsShine : MonoBehaviour
{
    public float size;
    public float TShine;
    public float TBetweenShine;
    private float t0;
    private int n = 0;
    private float offset;
    private Vector3 scaleInit;
    private bool Ended = true;
    private System.Random rnd = new System.Random();
    public int nbStars = 11;
    private List<float> Offset = new List<float>();
    private List<Vector3> ScaleInit = new List<Vector3>();

    private void Shine(float t0, Transform transform, Vector3 scaleInit)
    {
        transform.localScale = scaleInit * (1 + size / 100 * Mathf.Sin(2 * Mathf.PI * ((Time.time - t0) / TShine)));
    }
    // Use this for initialization
    void Start()
    {
        for (int i=0; i < nbStars; i++)
        {
            offset = TBetweenShine * rnd.Next(0, 1000000) / 1000000;
            scaleInit = GetComponentsInChildren<Transform>()[i].localScale;
            Offset.Add(offset);
            ScaleInit.Add(scaleInit);
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i<nbStars; i++)
        {
            offset = Offset[i];
            scaleInit = ScaleInit[i];
            if ((Time.time - offset) > n * TBetweenShine)
            {
                if (Ended)
                {
                    t0 = Time.time;
                    Ended = false;
                }
                if ((Time.time - t0) < TShine && !Ended)
                {
                    Shine(t0, GetComponentsInChildren<Transform>()[i], scaleInit);
                }
                else
                {
                    if (!(Ended))
                    {
                        Ended = true;
                        n += 1;
                    }
                }
            }
        }
    }
}