using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource[] destroyNoise;

    public void PlayRandomDestroyNoise()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                int clipToPlay = Random.Range(0, destroyNoise.Length);
                destroyNoise[clipToPlay].Play();
            }
        }
        else
        {
            int clipToPlay = Random.Range(0, destroyNoise.Length);
            destroyNoise[clipToPlay].Play();
        }
    }
}
