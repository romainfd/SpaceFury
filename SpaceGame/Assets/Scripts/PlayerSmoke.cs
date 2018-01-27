using UnityEngine;
using System.Collections;

public class PlayerSmoke : MonoBehaviour {
    public int n; // nb de nuages
    public float DeltaT; // tps entre nuages
    public GameObject cloudModel;

    public static float Dt; // delta t entre 2 nuages
    public static int m;

    private float initialAlpha = 1;
    private Vector3 initScale;
    private static GameObject[] Clouds;

    public static GameObject[] GetClouds() { return Clouds; }

	void Awake ()
    {
        Dt = DeltaT;
        m = 0;
        initScale = cloudModel.GetComponent<Transform>().localScale;
        Clouds = new GameObject[n];
        Vector3 playerPos = GameObject.FindWithTag("Player").GetComponent<Transform>().position;
        for (int i = 0; i < Clouds.Length; i++)
        {
            Clouds[i] = CreateCloud(playerPos, initialAlpha, 1);
            Clouds[i].GetComponent<Transform>().SetParent(GameObject.FindWithTag("LevelObjects").GetComponent<Transform>());
        }
    } 
	
	void Update ()
    {
	    if (Time.time - StartZoom.Gett0() > m * Dt && !GameManager.GetPaused() && StartZoom.GetBegin())
        {
            Destroy(Clouds[m % n]);
            Clouds[m%n] = CreateCloud(GameObject.FindWithTag("Player").GetComponent<Transform>().position, 1, 1);
            Clouds[m%n].GetComponent<Transform>().SetParent(GameObject.FindWithTag("LevelObjects").GetComponent<Transform>());
            for (int i = 1; i < n; i++) { ChangeCloud(ref Clouds[(m + i) % n]); }
            m += 1;
        }
	}

    private GameObject CreateCloud(Vector3 pos, float alpha, float partOfInitSize)
    {
        GameObject cloud = Instantiate(cloudModel, pos, Quaternion.identity) as GameObject;
        cloud.GetComponent<Transform>().localScale = partOfInitSize * initScale;
        // gérer l'alpha cloud.GetComponent<Renderer>().alpha
        return cloud;
    }

    private void ChangeCloud(ref GameObject cloud)
    {
        cloud.GetComponent<Transform>().localScale +=  - initScale/n;
        // gérer l'alpha
    }
}