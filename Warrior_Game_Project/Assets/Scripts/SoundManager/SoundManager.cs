using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    [SerializeField] private AudioClip[] enviromentSound;
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

    private void Start()
    {
        PlaySoundClip(enviromentSound[0]);
    }

    private void Update()
    {
        
    }

    public void PlaySoundClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

}
