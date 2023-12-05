using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] List<AudioSource> attackSounds;
    [SerializeField] List<AudioSource> dieSounds;
    [SerializeField] List<AudioSource> arrowSounds;
    [SerializeField] List<AudioSource> chopSounds;
    [SerializeField] List<AudioSource> footStepSounds;

    public static Sound Instance;
    public void Awake()
    {
        Instance = this;
    }

    public void PlayAttackSound()
    {
        attackSounds[Random.Range(0, attackSounds.Count)].Play();
    }

    public void PlayDieSound()
    {
        dieSounds[Random.Range(0, dieSounds.Count)].Play();
    }

    public void PlayArrowSound()
    {
        arrowSounds[Random.Range(0, dieSounds.Count)].Play();
    }

    public void PlayChopSound()
    {
        chopSounds[Random.Range(0, chopSounds.Count)].Play();
    }

    public void PlayFootStepSound()
    {
        footStepSounds[Random.Range(0, footStepSounds.Count)].Play();
    }
}
