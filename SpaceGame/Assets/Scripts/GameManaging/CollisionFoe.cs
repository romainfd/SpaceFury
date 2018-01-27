using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CollisionFoe : MonoBehaviour {
    private string colliderTag = null;
    public int starsNb = 0;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (!Editor.editing) { colliderTag = coll.gameObject.tag; }
        if (colliderTag == "Portal") { coll.gameObject.GetComponent<TeleportationRock>().Teleport(gameObject); }
        else if ((new List<string> { "Path", "SnowRock", "MagmaRock", "ExplosionRock", "BlackHole", "Foe", "FollowingFoe" }).Contains(colliderTag))
        {
            if (starsNb > 0) { GameManager.StarsPoping(starsNb, gameObject.GetComponent<Transform>().position); }
            Destroy(gameObject);
        }
    }
}


