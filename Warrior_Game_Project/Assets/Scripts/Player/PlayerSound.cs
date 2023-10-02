using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] playerSoundClips;

    public static PlayerSound instance;

    private void PlayWalkSound()
    {
        SoundManager.instance.PlayerSoundClip(playerSoundClips[0]);
    }
    public void PlayJumpSound()
    {
        SoundManager.instance.PlayerSoundClip(playerSoundClips[1]);
    }

    public void PlayAttackSound()
    {
        SoundManager.instance.PlayerSoundClip(playerSoundClips[2]);
    }

}
