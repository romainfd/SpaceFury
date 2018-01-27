using UnityEngine;
using System.Collections;

public class StartZoom : MonoBehaviour {
    private Camera cam;
    private static bool zoomed = false;
    private static float t0;
    private static bool begin = false;
    public float cameraSize;
    public GameObject pad;
    private Vector3 playerPos, padTranslation;
    private float screenRatio = 16/9;
    private static float TDarkness = -1;  // par défaut -1 ie pas de darkness mais si CreatedLevel awake setUpScene le màj
    private static float size;
    private GameObject darkness;
    private Editor editorScript;

 	void Start ()
    {
        cam = Camera.main;
        pad = GameObject.FindWithTag("Pad");
        pad.SetActive(false);
        begin = false;
        zoomed = false;
        t0 = 0;
        if (GameObject.FindWithTag("EditorSet") != null && TDarkness >= 0)   // si on est dans un createdLevel et que le dark va apparaitre à un moment (-1 : apparait jamais)
        {
            editorScript = GameObject.FindWithTag("EditorSet").GetComponent<Editor>();
            darkness = Instantiate(editorScript.darknessPrefab, GameObject.FindWithTag("Player").GetComponent<Transform>().position, Quaternion.identity) as GameObject;
            darkness.GetComponent<Transform>().localScale = size * editorScript.darknessPrefab.GetComponent<Transform>().localScale;
            darkness.SetActive(false);   // il réapparaitra quand time>tview
        }
    }

    public static void SetTDarkness(float T) { TDarkness = T; }
    public static void SetSize (float _size) { size = _size; }

    public static bool GetBegin()
    {
        return begin;
    }

    public static float Gett0()
    {
        return t0;
    }

    public void SetBegin(bool paramBegin)
    {
        begin = paramBegin;
    }

    private void PlacePad()
    {
        pad.SetActive(true);
        // la taille adaptée est 0.2 pour cam.size = 1
        pad.GetComponent<Transform>().localScale = new Vector3(0.2f * cam.orthographicSize* pad.GetComponent<Transform>().localScale.x, 0.2f * cam.orthographicSize* pad.GetComponent<Transform>().localScale.y, 1);
        // puis on le place dans le coin gauche sachant que le demi pad mesure 0.1*tailleCam
        // ATTENTION ERREUR DE MESURE !!!!!
        pad.GetComponent<Transform>().position = GameObject.FindWithTag("Player").GetComponent<Transform>().position;
        if (PlayerPrefs.GetString("PadPosition") == "Right") {
            padTranslation = new Vector3(-cam.orthographicSize * screenRatio, cam.orthographicSize * (1 - 0.6f), 1);
        } else { 
            padTranslation = new Vector3(cam.orthographicSize * screenRatio, cam.orthographicSize * (1 - 0.6f), 1);
        }
        pad.GetComponent<Transform>().Translate(-padTranslation);
        foreach (SpriteRenderer sprite in pad.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.color =  new Color(sprite.color.r, sprite.color.g, sprite.color.b, (255- PlayerPrefs.GetInt("PadTransparency")) /255f);
        }

        // On fait apparaitre les pouvoirs
        GameObject.FindWithTag("Powers").GetComponent<PowerManager>().Pause(false);
    }

    void Update ()
    {
        if (TDarkness >= 0 && !darkness.activeSelf && Time.time > TDarkness)  // si on doit mettre darkness && pas encore activé && il est temps
        {
            darkness.SetActive(true);
        }
        if ((!zoomed && Input.GetMouseButtonUp(0)) && !GameManager.GetPaused())
        {
            t0 = Time.time;
            cam.orthographicSize = cameraSize;
            playerPos = GameObject.FindWithTag("Player").GetComponent<Transform>().position;
            cam.GetComponent<RectTransform>().position = new Vector3(playerPos.x, playerPos.y, -10); // on centre sur le joueur mais on mt bien la caméra à z = -10
            PlacePad();
            SoundEffect.Instance.MakeStartingEngineSound();
            if (TDarkness >= 0) { darkness.SetActive(true); }  // si on doit avoir darkness, quand on commence => darkness
            zoomed = true;
        }
        if (zoomed && !begin && Time.time > t0 + 1 && !GameManager.GetPaused())
        {
                begin = true;  // begin que quand on peut bouger!
                t0 = Time.time; // on réinitialise le temps
        }        
	}
}
