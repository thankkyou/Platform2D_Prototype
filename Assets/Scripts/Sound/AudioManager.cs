using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioSource")]
    public AudioSource musicAudioSource;
    public  AudioSource sfxAudioSource;

    [Header("AudioClip")]
    public AudioClip background;
    public AudioClip checkpoint;
    public AudioClip playerDash;
    public AudioClip playerJump;
    public AudioClip playerHeal;
    public AudioClip playerShoot;
    public AudioClip playerDeath;
    public AudioClip enemyHit;

    
    [Header("Player Attack SFX")]
    public AudioClip playerAttack1;
    public AudioClip playerAttack2;
    public AudioClip playerAttack3;

    
    void Start()
    {
        musicAudioSource.clip = background;
        musicAudioSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip);
    }

    public void PlayAttackSFX(int comboStep)
    {
        AudioClip clip = comboStep switch
        {
            1 => playerAttack1,
            2 => playerAttack2,
            3 => playerAttack3,
            _ => playerAttack1
        };

        float pitch = comboStep switch
        {
            1 => 1.0f,
            2 => 1.2f,
            3 => 1.5f,
            _ => 1.0f
        };

        sfxAudioSource.pitch = pitch;
        sfxAudioSource.PlayOneShot(clip);
        sfxAudioSource.pitch = 1f; 
    }
}
