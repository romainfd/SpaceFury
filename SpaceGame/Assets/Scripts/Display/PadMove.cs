using UnityEngine;
using System.Collections;

public class PadMove : MonoBehaviour {
    public GameObject PadZone;
    public GameObject Pad;
    private Vector2 center;
    public GameObject PadBorder;
    public GameObject PadCenter;
    private Vector2 PadPosition;
    private Camera cam;
    private Vector3 direction;
    public float speed;
    private static bool caught;  // s'il a été pris par un harpon
    public static void SetCaught(bool _caught) { caught = _caught; }
    public static bool GetCaught() { return caught; }
    public static GameObject player;

    void Start ()
    {
        cam = Camera.main;
        caught = false;
        player = GameObject.FindWithTag("Player");
	}
	
    // Vérifier que le pad ne sort pas de la zone => quelle distance
    float distance(Vector3 PosObj, Vector3 centre)
    {
        return Mathf.Sqrt(Mathf.Pow(PosObj.x - centre.x, 2) + Mathf.Pow(PosObj.y - centre.y, 2));
    }


	// Update is called once per frame
	void Update () {
//        if (Input.touchCount > 0) pour quand Touch
        {
            if (StartZoom.GetBegin() && !caught && !GameManager.GetPaused()) // tout en position réelle (pas écran) et que x et y
            {
                center = new Vector2(PadCenter.transform.position.x, PadCenter.transform.position.y);
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (Input.GetTouch(0).position == null) { PadPosition = PadCenter.GetComponent<Transform>().position; }
                    PadPosition = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
                }
                else { PadPosition = cam.ScreenToWorldPoint(Input.mousePosition); }
                direction = PadPosition - center;
                if (distance(PadPosition, center) > 0.85f*distance(PadBorder.transform.position, center))
                {
                    direction /= distance(direction, new Vector3(0, 0, 0)); // divise par la norme
                    direction *= distance(PadBorder.transform.position, center);  // on donne la direction max
                    PadPosition = center + 0.85f*(new Vector2(direction.x, direction.y)); // on place le pad à 0.85 du centre
                }
                Vector3 directionNormee = direction / distance(PadBorder.transform.position, center); // norme entre 0 et  1
                Orientation.Move(this.gameObject, directionNormee * speed * Time.deltaTime);
                if (direction != new Vector3(0, 0, 0))
                {
                    float angle = Vector3.Angle(new Vector3(0, 1, 0), direction) * ((direction.x < 0) ? 1f : -1f) - GetComponent<Transform>().eulerAngles.z;   // l'angle manquant (la ref est Oy)
                    gameObject.GetComponent<Transform>().Rotate(new Vector3(0, 0, angle));
                }
                Pad.transform.position = PadPosition;
            }
        }
	}
}
