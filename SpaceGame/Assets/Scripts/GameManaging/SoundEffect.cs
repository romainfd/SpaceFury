using UnityEngine;
using System.Collections;

public class SoundEffect : MonoBehaviour
{
    public static SoundEffect Instance;
    public AudioClip explosion;
    public AudioClip startingEngine;

    void Awake()
    {
        if (Instance != null)
        {
//            Debug.LogError("Multiple instances of SoundEffectsHelper!");
        }
        Instance = this;
    }

    private void MakeSound(AudioClip effet)
    {
        AudioSource.PlayClipAtPoint(effet, transform.position, PlayerPrefs.GetFloat("SoundVolume"));
    }

    public void MakeExplosionSound()
    {
        MakeSound(explosion);
    }

    public void MakeStartingEngineSound()
    {
        MakeSound(startingEngine);
    }
}