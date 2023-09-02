using System;
using UnityEngine;


public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }
    
    public Transform pipeHead;
    public Transform pipeBody;
    public SoundAudioClip[] playList;

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }   

    private void Awake()
    {
        Instance = this;
    }
}