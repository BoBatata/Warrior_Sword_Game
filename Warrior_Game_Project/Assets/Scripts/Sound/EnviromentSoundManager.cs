using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] enviromentSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlaySoundClip(enviromentSound[0]);
    }

    public void PlaySoundClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

}
