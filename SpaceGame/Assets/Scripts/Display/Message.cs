using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    public GameObject messageZone;

    void Start()
    {
        messageZone = GameObject.FindWithTag("Message");
        messageZone.SetActive(false);
        // TO DISPLAY A MESSAGE AT THE BEGINNING :
        // StartCoroutine(DisplayMessage(PlayerPrefs.GetString("Ghost:test23"), 3));
    }

    public IEnumerator DisplayMessage(string message, float deltaTime)
    {
        messageZone.SetActive(true);
        messageZone.GetComponentInChildren<Text>().text = message;
        yield return new WaitForSeconds(deltaTime);
        messageZone.SetActive(false);
    }
}