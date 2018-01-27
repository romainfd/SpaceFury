using UnityEngine;
using System.Collections;

public class Harponer : MonoBehaviour
{
    public GameObject harponModel;
    public float vGoRelative;
    public float vBack;
    public float TRearmement;

    public float t0;

    private GameObject harpon;
    private Vector3 direction;
    private bool going, returning;
    public bool caught;
    private GameObject victim;
    private float vGo;

    void Start()
    {
        harpon = Instantiate(harponModel, GetComponent<Transform>().position, Quaternion.identity) as GameObject;
        harpon.GetComponent<Harpon>().harponer = gameObject;
        harpon.GetComponent<Transform>().SetParent(gameObject.GetComponent<Transform>());
        CreateHarpon();
        vGo = vGoRelative * GameObject.FindWithTag("Player").GetComponent<PadMove>().speed;  // c'est vPlayer * le coeff
        vBack = vBack * GameObject.FindWithTag("Player").GetComponent<PadMove>().speed;  // c'est vPlayer * le coeff
    }

    public void CreateHarpon()
    {
        harpon.GetComponent<Transform>().position = GetComponent<Transform>().position;
        harpon.GetComponent<Harpon>().lifeBar.SetActive(false);
        going = false;
        returning = false;
        caught = false;
    }

    void Update()
    {
        if (!harpon.GetComponent<Harpon>().rearming)    // si on recharge => pas se déplacer
        {
            if (going) { Orientation.Move(harpon, direction * vGo * Time.deltaTime); }
            if (returning) { Orientation.Move(harpon,-direction * vBack / 4 * Time.deltaTime); }
            if (caught) { victim.GetComponent<Transform>().position = harpon.GetComponent<Transform>().position; }
        }
        else if (Time.time > t0 + TRearmement)  // on a fini de recharger => harpon apparait
        {
            harpon.SetActive(true);
            harpon.GetComponent<Harpon>().rearming = false;
        }
    }


    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && !harpon.GetComponent<Harpon>().rearming && !going && !returning && !Editor.editing && StartZoom.GetBegin()) // si on est à l'attente et que le joueur rentre
        {
            direction = (coll.gameObject.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized;
            going = true;
//            harpon.GetComponent<Harpon>().RotateHarpon(direction);      inutile quand on tourne le harponer qui contient le harpon en enfant
            gameObject.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, Vector3.Angle(direction, new Vector3(0, 1, 0)) * ((direction.x < 0) ? 1.0f : -1.0f));
        }
    }

    public void Harponed(GameObject _victim)
    {
        victim = _victim;
        if (victim.tag == "Player") { PadMove.SetCaught(true); }
        else { Destroy(victim.GetComponent<Following>()); }
        caught = true;
        going = false;
        returning = true;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject == harpon) { returning = false; }
        else { Destroy(coll.gameObject); }
    }
}
