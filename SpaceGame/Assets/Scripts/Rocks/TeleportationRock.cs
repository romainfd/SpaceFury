using UnityEngine;
using System.Collections.Generic;

public class TeleportationRock : MonoBehaviour {
    public List<GameObject> portals = new List<GameObject>();
    private int nbPortals;

    void Start ()
    {
        nbPortals = portals.Count;
	}

    public void Teleport(GameObject toTeleport)
    {
        int index = portals.IndexOf(this.gameObject);  // l'indice du portail où on rentre
        portals[(index + 1) % nbPortals].GetComponent<CircleCollider2D>().enabled = false;
        Vector3 translation = portals[(index + 1) % nbPortals].GetComponent<Transform>().position - portals[index].GetComponent<Transform>().position;
        toTeleport.GetComponent<Transform>().Translate(translation);
    }

    void OnTriggerExit2D()
    {
        GetComponent<CircleCollider2D>().enabled = true;
    }
}
