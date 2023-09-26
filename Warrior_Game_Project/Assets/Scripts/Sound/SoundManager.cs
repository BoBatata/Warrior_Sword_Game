using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    [SerializeField] private float volume = 100;

    private AudioSource audioSource;

    private void Awake()
    {
        #region Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
        }
        #endregion

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        audioSource.volume = volume / 2;
    }

}
