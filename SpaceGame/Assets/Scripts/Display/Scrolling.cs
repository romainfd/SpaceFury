using UnityEngine;
using System.Collections;

public class Scrolling : MonoBehaviour {
    private Vector3 initialPos, newPos, translation;
    private GameObject player, pad;
    private GameObject[] backgroundStars;

	void Start ()
    {
        player = GameObject.FindWithTag("Player");
        pad = gameObject.GetComponent<StartZoom>().pad;
        initialPos = player.GetComponent<Transform>().position;
        newPos = initialPos;
        backgroundStars = GameObject.FindGameObjectsWithTag("BackgroundStar");
	}

    void LateUpdate()
    {
        // si on a commencé à bouger, on fait le scrolling
        if (StartZoom.GetBegin())
        {
            initialPos = newPos;
            newPos = player.GetComponent<Transform>().position;
            translation = newPos - initialPos;
            Camera.main.GetComponent<RectTransform>().Translate(translation);
            pad.GetComponent<Transform>().Translate(translation);
            foreach (GameObject star in backgroundStars)
            {
                star.GetComponent<Transform>().Translate(-star.GetComponent<Transform>().position.z * translation / 150);
            }
        }
    }
}
