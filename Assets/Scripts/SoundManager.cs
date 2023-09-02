using System;
using UnityEngine;

public static class SoundManager
{
    public enum Sound
    {
        Jump,
        Score,
        Death,
        Highscore,
        ButtonClick
    }

    public static void PlaySound(Sound sound)
    {
        GameObject gameObject = new GameObject("Sound", typeof(AudioSource));
        AudioSource source = gameObject.GetComponent<AudioSource>();
        source.PlayOneShot(GetAudioClip(sound));
    }

    static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.playList)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }
}
