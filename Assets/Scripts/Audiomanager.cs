using UnityEngine;

public class audiomanager : MonoBehaviour
{
    [Header("----------Audiosource---------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("----------SFXsource---------")]
    public AudioClip Background;
    public AudioClip Jump;
    public AudioClip swing;
    public AudioClip death;
    public AudioClip hitted;
    public AudioClip enemyhitted;
    public AudioClip springbounce;


    private void Start()
    {
        musicSource.clip = Background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

}