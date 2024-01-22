using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MineSweeperSound 
{
    AudioSource audioSource;

    const string directory = "Sounds/";

    public enum SoundType
    {
        Click, Fail, Win,Flag, DeFlag, Count
    }

    AudioClip[] SoundArray;
    public MineSweeperSound()
    {
        audioSource = Camera.main.GetComponent<AudioSource>();
        SoundArray = new AudioClip[(int)SoundType.Count];
        for (int i = 0; i < (int)SoundType.Count; i++)
        {
            SoundArray[i] = Resources.Load<AudioClip>(directory + ((SoundType)i).ToString());
            if (SoundArray[i] == null) Debug.Log("sound load fail!");
        }
    }

    public void Play(SoundType _sound)
    {
        if (SoundArray[(int)_sound] == null)
        {
            SoundArray[(int)_sound] = Resources.Load<AudioClip>(directory + _sound.ToString());
        }
        audioSource.PlayOneShot(SoundArray[(int)_sound]);
    }


}
