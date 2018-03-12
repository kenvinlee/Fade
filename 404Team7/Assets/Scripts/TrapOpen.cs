using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapOpen : MonoBehaviour
{

    /*************************************************************************************************
    This script moves the trapdoor object left and right if the light is shining/not shining on it

        the idea is to put the trapdoor object over a hole in a plane or floor that falls through to 
        the floor below

        The simplest solution was to just move the platform.

        if we turn off the renderer the object is still there 
        if we turn off collisions there is no way to turn them on again, as we use a collider 
            detect interactions with the light sphere
        if we disable the objects, then the collisions stop to re-enable them.
    *************************************************************************************************/


    bool animated = false;

    public float movementDistance = 3;

    public char moveDirection = 'x';


    public bool moved_platform_flag = false;  // move platform left to right when animated and right to left when not animated (or vice versa.)

    private AudioSource[] soundEffects;
    private bool sndExists = false;
    private bool isPaused = false;


    // used to make trapdoors that move when the light is on versus slide when light is off.  Either case put trap door over the hole to cover.


    void Start()
    {
        Vector3 originalPosition = transform.position;
    
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

            if (!moved_platform_flag)
            {
                Vector3 V3 = transform.position;

                if (moveDirection == 'x')
                    V3.x += movementDistance;
                if (moveDirection == 'y')
                    V3.y += movementDistance;
                if (moveDirection == 'z')
                    V3.z += movementDistance;
                //else {
                //    Debug.Log("error should specify x, y or z direction");
                //}
                transform.position = V3;
                moved_platform_flag = true;
            }

            /*if (rotate_trap)
            {
                transform.Rotate(90, 0, 0);
            }*/

        }
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
        else  // (!animated)
        {

            if (moved_platform_flag)
            {
                Vector3 V3 = transform.position;
                if (moveDirection == 'x')
                    V3.x -= movementDistance;
                if (moveDirection == 'y')
                    V3.y -= movementDistance;
                if (moveDirection == 'z')
                    V3.z -= movementDistance;
                //else {
                //    Debug.Log("error should specify x, y or z direction");
                //}
                transform.position = V3;
                moved_platform_flag = false;
            }
            /*if (rotate_trap)
            {
                transform.Rotate(-90, 0, 0);
            }*/

            if (sndExists && soundEffects != null && soundEffects.Length != 0)
            {
                if (soundEffects[0].isPlaying)
                {
                    soundEffects[0].Pause();
                    isPaused = true;

                }
            }
        }
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

    private class function
    {
    }
}

