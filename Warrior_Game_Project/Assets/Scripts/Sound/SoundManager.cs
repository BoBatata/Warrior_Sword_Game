using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    [SerializeField] private float volume = 100;

    public static SoundManager instance;

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

        DontDestroyOnLoad(this.gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        audioSource.volume = volume / 100;
    }

    public void PlayerSoundClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
