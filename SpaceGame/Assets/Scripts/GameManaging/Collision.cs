using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Collision : MonoBehaviour {
    private string colliderTag;

    void OnCollisionEnter2D(Collision2D coll)
    {
        colliderTag = coll.gameObject.tag;
        if (colliderTag == "Path")
            GameManager.SetDead(true);
        else if (colliderTag == "Base")
        {
            GameManager.SetJustWon(true);
            Destroy(coll.gameObject.GetComponent<CircleCollider2D>());
        }
        else if (colliderTag == "SnowRock")
            GameManager.SetDead(true);
        else if (colliderTag == "MagmaRock")
            GameManager.SetDead(true);
        else if (colliderTag == "ExplosionRock")
            GameManager.SetDead(true);
        else if (colliderTag == "BlackHole")
            GameManager.SetDead(true);
        else if (colliderTag == "Portal")
            coll.gameObject.GetComponent<TeleportationRock>().Teleport(gameObject);
        else if (colliderTag == "Foe")
            GameManager.SetDead(true);
        else if (colliderTag == "FollowingFoe")
            GameManager.SetDead(true);
        else if (colliderTag == "Star")
        {
            GameManager.StarCatched();
            Destroy(coll.gameObject);
        }
    }
}
