using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************************************************
This program simply plays the background music if it isn't playing

    In our program this script is attached to The SoundManager Empty Object

    which has an attached audio component

    play on awake and loop checkboxes should be checked.

    reference
    https://thenappingkat.azurewebsites.net/unity-gaming-music-and-sound-part-2/

 ***********************************************************************************/
public class SoundManager : MonoBehaviour
{

    public AudioSource musicSource;
    public static SoundManager instanceSM = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (!(GetComponent<AudioSource>().isPlaying))
        {
            GetComponent<AudioSource>().Play();
        }
        else
        {
            //Debug.log("Something is wrong with Music.");	
        }
    }
}

