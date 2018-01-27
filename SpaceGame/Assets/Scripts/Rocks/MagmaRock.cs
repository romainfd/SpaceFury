using UnityEngine;
using System.Collections.Generic;

public class MagmaRock : MonoBehaviour {
    public float T, TMove, TOut;
    public GameObject rock, flame, limit;

    private Vector3 scaleInit;
    private float t;
    private float t0;
    private int n;
    private Vector3 rockPos, limitPos;
    private Vector3 translation;
    private float lambda;
    private float tBegin; // => remet à 0 quand ça recommence (Time.time continue !!)

    float distance(float x, float y, float x0, float y0) { return Mathf.Sqrt((x - x0) * (x - x0) + (y - y0) * (y - y0)); }

    void Start()
    {
        // mettre flame entre le rocher et la limit
        limitPos = limit.GetComponent<Transform>().position;
        rockPos = rock.GetComponent<Transform>().position;
        translation = (limitPos - rockPos)/2;
        // on prend la longueur de la flamme :
        Vector3 angleFlame = flame.GetComponent<Transform>().eulerAngles;
        flame.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, 0);
        float length = 2 * flame.GetComponent<Renderer>().bounds.extents.x;
        flame.GetComponent<Transform>().eulerAngles = angleFlame; // on la remet bien après

        scaleInit = flame.GetComponent<Transform>().localScale * distance(2 * translation.x, 2 * translation.y, 0, 0) / length;
        flame.GetComponent<Transform>().rotation = Quaternion.identity;
        float angle = Vector3.Angle(new Vector3(1, 0, 0), translation) * ((translation.y > 0) ? 1.0f : -1.0f);  // -1 si dessous
        flame.GetComponent<Transform>().Rotate(new Vector3(0,0,angle));
        flame.GetComponent<Transform>().position = rockPos;
        n = 0;
        tBegin = Time.time;
        t0 = 0;
        if (!Editor.editing) { Destroy(limit); }
    }

    public void Refresh()
    {
        if (rock.GetComponent<Transform>().position != limit.GetComponent<Transform>().position) { Start(); }
    }

    float time() { return Time.time - tBegin; }

    void FlameThrower(float t0, float TMove, float TOut)
    {
        t = time() - t0;   // depuis cbn de temps ça a commencé
        if (t < TMove)
        {
            lambda = t / TMove;
            flame.GetComponent<Transform>().localScale = scaleInit * lambda;
            flame.GetComponent<Transform>().position = rock.GetComponent<Transform>().position + lambda*translation;
        }
        else if (t < TMove + TOut)  // on le sort complétement pour TOut
        {
            lambda = 1;
            flame.GetComponent<Transform>().localScale = scaleInit * lambda;
            flame.GetComponent<Transform>().position = rock.GetComponent<Transform>().position + lambda * translation;
        }
        else if (t < 2*TMove + TOut)
        {
            lambda = (t - (TMove + TOut)) / TMove;
            flame.GetComponent<Transform>().localScale = scaleInit * (1 - lambda);
            flame.GetComponent<Transform>().position = rock.GetComponent<Transform>().position + (1- lambda) * translation;
        }
        else // if (t>2*TMove + TOut)  // il est rentré jusqu'à la fin
        {
            lambda = 0.999f;
            flame.GetComponent<Transform>().localScale = scaleInit * (1 - lambda);
            flame.GetComponent<Transform>().position = rock.GetComponent<Transform>().position + (1 - lambda) * translation;
        }
    }
	
	void FixedUpdate ()
    {
//        if (n == 0 && limit != null) { Destroy(limit); }
        if (time() - t0 > T)
        {
            n += 1;
            t0 = time();
        }
        FlameThrower(t0, TMove, TOut);
	}
}
