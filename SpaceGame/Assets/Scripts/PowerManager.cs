using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PowerManager : MonoBehaviour {
    public static bool[] powersTaken;
    public GameObject[] slotZones;
    public Sprite[] powersImage; // la liste des images à afficher pour chaque pouvoir

    public static GameObject player;

    private Slot[] slots;
    private int nbPowers;

    // les différentes actions dans la vie d'un pouvoir
    private Action[] startingPower;
    private Action[] actingPower;
    private Action[] stoppingPower;

    private Action[] startingPowerSlot;
    private Action[] actingPowerSlot;
    private Action[] stoppingPowerSlot;

    void Awake()
    {
        startingPower = new Action[] { PowerEmpty, Boost.StartingBoost, Ghost.StartingGhost, SlowMotion.StartingSlowMotion };
        actingPower = new Action[] { PowerEmpty, Boost.Boosting, Ghost.Ghosting, SlowMotion.SlowMotioning };
        stoppingPower = new Action[] { PowerEmpty, Boost.StoppingBoost, Ghost.StoppingGhost, SlowMotion.StoppingSlowMotion };
        player = GameObject.FindWithTag("Player");
        slots = new Slot[slotZones.Length];
        int i = 0;
        foreach (GameObject slotZone in slotZones) { slots[i++] = new Slot(slotZone, powersImage[0]); } // on initialise les zones avec l'image neutre
        if (PlayerPrefs.GetString("PadPosition")=="Right")
        {
            foreach (GameObject slot in slotZones) {
                Vector2 tempMax = new Vector2(1 - slot.GetComponent<RectTransform>().anchorMin.x, slot.GetComponent<RectTransform>().anchorMin.y);
                slot.GetComponent<RectTransform>().anchorMin = new Vector2(1 - slot.GetComponent<RectTransform>().anchorMax.x, slot.GetComponent<RectTransform>().anchorMax.y);
                slot.GetComponent<RectTransform>().anchorMax = tempMax;
            }
        }
    }

    void Start() {
        SetUp(powersTaken);
        Pause(true); // on cache les pouvoirs
    }

    public void SetUp(bool[] powers) // powers est une liste avec true si on active le i_ème pouvoir
    {
        int n = 0;
        foreach (bool power in powers) { if (power) { n += 1; } } // le nombre de pouvoirs pris
        if (n > slots.Length) { n = slots.Length; }  // qu'on limite au nombre de slots dispos
        // on a donc n pouvoirs => on initialise les listes qui contiendront les actions des n pouvoirs
        startingPowerSlot = new Action[n];
        actingPowerSlot = new Action[n];
        stoppingPowerSlot = new Action[n];
        int i = 0;
        nbPowers = 0;
        while (nbPowers < n) // i parcourt tous les pouvoirs et nbPowers parcourent les slots ie les pouvoirs qu'on prend
        {
            if (powers[i]) // si on prend le i_ème pouvoir
            {
                slots[nbPowers].powerNb = i;    // le power i est activé => on le met au 1er slot libre puis on passe au slot suivant pour après
                slots[nbPowers].UpdateZone(powersImage[i]); // on met la bonne image
                slots[nbPowers].slotZone.SetActive(true); // et on l'active
                startingPowerSlot[nbPowers] = startingPower[i];
                actingPowerSlot[nbPowers] = actingPower[i];
                stoppingPowerSlot[nbPowers] = stoppingPower[i];
                nbPowers += 1;
            }
            i += 1;
        }
    }

    public void PowerEmpty()
    {
        // faire vibrer l'écran , ... pour signaler que le pouvoir est vide ou un petit son
    }

    void Update()
    {  // les PowerSlot permettent de placer les pouvoirs activés de côté :)
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    if (Input.GetButtonDown("ButtonFirstPower")) { startingPowerSlot[0](); }
        //    if (Input.GetButton("ButtonFirstPower")) { actingPowerSlot[0](); }
        //    if (Input.GetButtonUp("ButtonFirstPower")) { stoppingPowerSlot[0](); }
        //    if (Input.GetButtonDown("ButtonSecondPower")) { startingPowerSlot[1](); }
        //    if (Input.GetButton("ButtonSecondPower")) { actingPowerSlot[1](); }
        //    if (Input.GetButtonUp("ButtonSecondPower")) { stoppingPowerSlot[1](); }
        //    if (Input.GetButtonDown("ButtonThirdPower")) { startingPowerSlot[2](); }
        //    if (Input.GetButton("ButtonThirdPower")) { actingPowerSlot[2](); }
        //    if (Input.GetButtonUp("ButtonThirdPower")) { stoppingPowerSlot[2](); }
        //}
        //else
        {
            if (Input.GetKeyDown(KeyCode.Keypad1)) { startingPowerSlot[0](); }
            if (Input.GetKey(KeyCode.Keypad1)) { actingPowerSlot[0](); }
            if (Input.GetKeyUp(KeyCode.Keypad1)) { stoppingPowerSlot[0](); }
            if (Input.GetKeyDown(KeyCode.Keypad2)) { startingPowerSlot[1](); }
            if (Input.GetKey(KeyCode.Keypad2)) { actingPowerSlot[1](); }
            if (Input.GetKeyUp(KeyCode.Keypad2)) { stoppingPowerSlot[1](); }
            if (Input.GetKeyDown(KeyCode.Keypad3)) { startingPowerSlot[2](); }
            if (Input.GetKey(KeyCode.Keypad3)) { actingPowerSlot[2](); }
            if (Input.GetKeyUp(KeyCode.Keypad3)) { stoppingPowerSlot[2](); }
        }
    }

    public void Pause(bool pausing)
    {
        if (pausing)
        {
            for (int i = 0; i < nbPowers; i++)
            {
                slots[i].slotZone.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < nbPowers; i++)
            {
                slots[i].slotZone.SetActive(true); // et on l'active
            }
        }
    }
}

public struct Slot
{
    public GameObject slotZone;
    public int powerNb;

    public Slot(GameObject obj)
    {
        slotZone = obj;
        powerNb = 0;  //emptyPower
    }

    public Slot(GameObject obj, Sprite image) // ATTENTION : désactive le slot à la fin !!
    {
        slotZone = obj;
        slotZone.GetComponent<Button>().image.sprite = image;
        powerNb = 0;
        obj.SetActive(false);
    }

    public void UpdateZone(Sprite image) { slotZone.GetComponent<Button>().image.overrideSprite = image; }

    // 1er pouvoir/neutre pour désactiver un pouvoir, une fois qu'il est vidé
    public void NeutralZone() { slotZone.GetComponent<Button>().image.overrideSprite = null; }
}

public class Boost
{
    public static float duration = 1;  // durée totale du boost
    private static float v0 = PowerManager.player.GetComponent<PadMove>().speed;
    private static float currentDuration = duration; // la durée qu'il reste

    public static void StartingBoost()
    {
        if (currentDuration > 0) { PowerManager.player.GetComponent<PadMove>().speed = 2 * v0; }
    }

    public static void Boosting()
    {
        currentDuration -= Time.deltaTime;
        if (currentDuration < 0) { StoppingBoost(); }
    }

    public static void StoppingBoost() { PowerManager.player.GetComponent<PadMove>().speed = v0; }
}

public class Ghost // juste enlever son image et son collider !!
{
    public static float duration = 2;  // durée totale du ghost
    private static float currentDuration = duration; // la durée qu'il reste


    public static void StartingGhost()
    {
        if (currentDuration > 0)
        {
            PowerManager.player.GetComponent<SpriteRenderer>().enabled = false;
            foreach (BoxCollider2D collider in PowerManager.player.GetComponents<BoxCollider2D>()) { collider.enabled = false; }
        } // on désactive son sprite => tjs nuages !
    }

    public static void Ghosting()
    {
        currentDuration -= Time.deltaTime;
        if (currentDuration < 0) { StoppingGhost(); }
    }

    public static void StoppingGhost()
    {
        PowerManager.player.GetComponent<SpriteRenderer>().enabled = true;
        PowerManager.player.GetComponent<BoxCollider2D>().enabled = true;
    }
}

public class Jump // juste passer à l'arrière plan !!
{
    public static float duration = 3;  // durée totale du jump
    private static float currentDuration = duration; // la durée qu'il reste


    public static void StartingJump()
    {
    }

    public static void Jumping()
    {
        currentDuration -= Time.deltaTime;
    }

    public static void StoppingJump()
    {
    }
}

public class SlowMotion
{
    public static float duration = 1;  // durée totale du boost
    private static float timeScale0 = Time.timeScale;
    private static float currentDuration = duration; // la durée qu'il reste

    public static void StartingSlowMotion()
    {
        if (currentDuration > 0) { Time.timeScale = 0.5f * timeScale0 ; }
    }

    public static void SlowMotioning()
    {
        currentDuration -= Time.deltaTime;
        if (currentDuration < 0) { StoppingSlowMotion(); }
    }

    public static void StoppingSlowMotion() { Time.timeScale = timeScale0; }
}
