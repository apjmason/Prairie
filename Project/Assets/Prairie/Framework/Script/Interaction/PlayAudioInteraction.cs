using UnityEngine;
using System.Collections;

public class PlayAudioInteraction : Interaction
{
    public AudioClip audioClip;
    //we have a publi audio source so that the user can import their own
    public AudioSource audioSource;

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
        audioSource.Play();
        
        
    }
}
