//used to play a sound at random times, is currently not working correctly
using UnityEngine;

public class RandomSound : MonoBehaviour
{
/*
    public AudioSource gameSound; //sound to play, is set up in the editor

    public float firstPlay; //how long until to play the sound, is set up in editor
    public float randomMin; //minimum time sound plays, is set up in the editor
    public float randomMax; //maximum time sound plays, is set up in the editor

    public bool sound3D = true; //if the sound is 3D or not, default is set to true in editor


    void Start()
    {
        Invoke("PlaySound", firstPlay); //get the sound ready
    }

    void Update()
    {
        //meant to be empty
    }

    void PlaySound()
    {
        //get the audio clip
        GameObject sound = new GameObject();
        AudioSource track = sound.AddComponent<AudioSource>();
        track.clip = gameSound.clip;
        
        //set up the 3D sound settings if it is 3D
        if (sound3D)
        {
            track.spatialBlend = 1;
            track.maxDistance = gameSound.maxDistance;
            track.volume = gameSound.volume;
            gameSound.transform.SetParent(transform);
            gameSound.transform.localPosition = Vector3.zero;
        }

        track.Play(); //play sound for the first time 

        //now play the sound at different times before destroying it
        Invoke("PlaySound", Random.Range(randomMin, randomMax));
        Destroy(sound, gameSound.clip.length);
    }
 */
}
