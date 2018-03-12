using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class environmentSoundPlayer : MonoBehaviour
{

    /*************************************************************************************************
    This script plays the sound stored stored in the audiosource when the light orb is animating and
    interacting with that object.  The object would have to have some sort of collider, for this to
    work correctly.  When animating the sound plays.  when not animating the sound pauses.

        This could be modified to play more than one sound effect at random
        if more audiosources were available to the object, than only the one
        or to play sounds in a certian order etc.

        be sure to uncheck "play on awake" or the sounds will play continously, automatically. (unity default)
    ------------------------------------------------------------------------------------------------------

        requires AudioSource, Collider.  
        for correct operation uncheck play on awake in AudioSource

        Attach audioSource and this script to object, and the sound will play
        when the orb interaction occurs, pause when it stops, and restart when
        the interaction continues.

        The script doesn't complain when the audiosource isn't yet attached to the object.
    
    April 2/2017 
    SS
    *************************************************************************************************/


    private AudioSource[] soundEffects;
    private bool sndExists = false;
    private bool isPaused = false;

    bool animated = false;

    void Start()
    {
        if (GetComponents<AudioSource>() != null)
        {
            soundEffects = GetComponents<AudioSource>();
            sndExists = true;
        }
    }

    void Update()
    {

        if (animated)
        {

            if (soundEffects != null && soundEffects.Length != 0)
            {

                if (sndExists && !isPaused && !(soundEffects[0].isPlaying))
                {
                    soundEffects[0].Play();
                    isPaused = false;
                }
                else if (sndExists && isPaused && !(soundEffects[0].isPlaying))
                {
                    soundEffects[0].UnPause();
                    isPaused = false;
                }
            }
        }
        else  // (!animated)
        {

            if (sndExists && soundEffects != null && soundEffects.Length != 0)
            {
                if (soundEffects[0].isPlaying)
                {
                    soundEffects[0].Pause();
                    isPaused = true;

                }
            }
        }//else not animated
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Orb")
        {
            animated = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Orb")
        {
            animated = false;
        }
    }

}

