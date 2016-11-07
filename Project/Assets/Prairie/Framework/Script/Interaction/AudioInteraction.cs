using UnityEngine;
using System.Collections;

public class AudioInteraction : Interaction
{
    public AudioClip audioClip;
    //we have a publi audio source so that the user can import their own
    public AudioSource audioSource;
    private bool playing; //toggles clip

    /// <summary>
    /// Plays an audio clip when the player interacts with an object
    /// </summary>
    protected override void PerformAction()
    {
        //A default audio source is created if the user does not import one
        if (audioSource == null)
        {
             audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = audioClip;
        
        //allows us to stop the clip while it is playing
        if (playing)
        {
            audioSource.Stop();
            playing = false;
            
        } else
        {
            audioSource.Play();
            playing = true;
        }
        
        
        
    }
}
