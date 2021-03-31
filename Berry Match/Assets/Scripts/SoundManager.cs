using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource[] destroyNoise;

    public void PlayRandomDestroyNoise()
    {
        int clipToPlay = Random.Range(0, destroyNoise.Length);
        destroyNoise[clipToPlay].Play();

    }
}
