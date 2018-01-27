using UnityEngine;
using System.Collections;

public class Orientation : MonoBehaviour {
    private Vector3 position, formerPosition, direction;

	void Start ()
    {
        position = gameObject.GetComponent<Transform>().localPosition;
        formerPosition = position;
	}
	
	void Update ()
    {
        formerPosition = position;
        position = gameObject.GetComponent<Transform>().localPosition;   // si les parents se déplacent !!
        direction = position - formerPosition;
        if (direction != new Vector3(0, 0, 0))
        {
            float angle = Vector3.Angle(new Vector3(0, 1, 0), direction)*((direction.x < 0) ? 1f : -1f) - GetComponent<Transform>().eulerAngles.z;   // l'angle manquant (la ref est Oy)
            gameObject.GetComponent<Transform>().Rotate(new Vector3(0, 0, angle));
        } 
	}

    public static void Move(GameObject obj, Vector3 translation) // remet droit avant de déplacer
    {
        Vector3 angle = obj.GetComponent<Transform>().eulerAngles;
        obj.GetComponent<Transform>().Rotate(-angle);
        obj.GetComponent<Transform>().Translate(translation);
        obj.GetComponent<Transform>().Rotate(angle);
    }
}
