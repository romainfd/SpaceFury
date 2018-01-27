using UnityEngine;
using System.Collections;

public class SnowRock : MonoBehaviour {
    public float T;
    private Vector3 direction;
    private float normeDirection, distanceToRock;
    private float lambda;
    public int sens = 1;
    public GameObject UpLimit, DownLimit;

    float distance(Vector3 PosObj, Vector3 centre)
    {
        return Mathf.Sqrt(Mathf.Pow(PosObj.x - centre.x, 2) + Mathf.Pow(PosObj.y - centre.y, 2));
    }

    void Start()
    { // on def le segment sur lequel on va bouger
        direction = UpLimit.GetComponent<Transform>().position - DownLimit.GetComponent<Transform>().position;
        distanceToRock = distance(GetComponent<Transform>().position, DownLimit.GetComponent<Transform>().position);
        normeDirection = distance(direction, new Vector3(0, 0, 0));
        lambda = distanceToRock / normeDirection;
    }

    public void Refresh()
    {
        Start();
    }

    void FixedUpdate()
    {
        if (UpLimit != null && !Editor.editing) { Destroy(UpLimit); Destroy(DownLimit); }
        if (lambda < 0)
            sens = 1;
        else if (lambda > 1)
            sens = -1;
        lambda += sens * Time.deltaTime/T;
        Orientation.Move(gameObject, sens * direction * Time.deltaTime/T);
    }
}
