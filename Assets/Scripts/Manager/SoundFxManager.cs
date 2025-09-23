using UnityEngine;

public class SoundFxManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource soundFxObject;
    [SerializeField]
    private AudioClip goalScoredClip;
    [SerializeField]
    private AudioClip ballKickedClip;
    [SerializeField]
    private AudioClip goalClip;

    public void PlaySoundFxClip(AudioClip audioClip, Vector3 position)
    {
        // spawn in gameObject
        AudioSource audioSource = Instantiate(soundFxObject, position, Quaternion.identity);

        //assign the audio clip 
        audioSource.clip = audioClip;

        //assign the volume
        audioSource.volume = 1.0f;

        // play sound
        audioSource.Play();

        // get length of the clip
        float clipLength = audioSource.clip.length;

        // destroy the gameObject after the clip has finished playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayGoalScored()
    {
        PlaySoundFxClip(goalClip, Vector3.zero);
        PlaySoundFxClip(goalScoredClip, Vector3.zero);
    }

    public void PlayBallKicked(Vector3 position) {
        PlaySoundFxClip(ballKickedClip, position);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
