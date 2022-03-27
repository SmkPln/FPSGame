using UnityEngine;

public class ImpactScript : MonoBehaviour
{
    [SerializeField] private float timeToDestroy;
    [SerializeField] private AudioClip[] impactSounds;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        AudioClip clip = impactSounds[Random.Range(0, impactSounds.Length)];
        _audioSource.PlayOneShot(clip);

    }
}
