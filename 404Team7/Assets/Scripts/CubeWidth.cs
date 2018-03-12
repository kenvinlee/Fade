using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeWidth : MonoBehaviour
{

    /*************************************************************************************************
    The purpose of this script is to change the size of a cube shaped platform only in the z direction.

        When the light is on, the cube's width raises from current width rightwards)
        When the light is off the cube's width lowers from current Width leftwards

        Leftwards and rightwards speeds of the platforms can also be adjusted to adjust difficulty in 
        alighning platforms for the player.

                
        known bugs:  if you are under a platform that comes down to the ground you get pushed (squeezed) 
        through the plane.  platforms have to start from the Width of the plane  
        
    *************************************************************************************************/


    bool animated = false;



    float currentWidth = 1.0f;      // the current size of the cube (should match minWidth)
    public float minWidth = 1.0F;          // the smallest Width for the cube.  (set to its original y-Width)

    public float maxWidth = 50.0F;         // the maximum Width of the cube (y-direction)
    public float speed_left = 5.0f;      // the speed of the Width change upwards (larger the number, the faster it goes)
    public float speed_right = 5.0f;    // the speed of the Width change downwards 
                                        //public float damageSize = 1.0f;       // the Width when the cube is dangerous
                                        //public bool isDangerous = false;      // default is that the cube is not a dangerous object.  


    private AudioSource[] soundEffects;
    private bool sndExists = false;
    private bool isPaused = false;

    //if we want to make the cube a dangerous object, the dangerous object script (DangerousObject.cs) must also be attached to the cube object.
    //**** the dangerous object idea doesn't work, as the dangerous object script disables collisions so the platforms become transparent.
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
            /*if (isDangerous && currentSize == damageSize)
            {
                DangerousObject otherScript = GetComponent<DangerousObject>();
                otherScript.isDangerous = true;

            }
            else
            {
                DangerousObject otherScript = GetComponent<DangerousObject>();
                otherScript.isDangerous = false;
            }
            */
            currentWidth = currentWidth + (Time.deltaTime * speed_left);
            if (currentWidth > maxWidth)
            {
                currentWidth = maxWidth;
            }
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, currentWidth);

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
            /*if (isDangerous && currentSize == damageSize)
            {
                DangerousObject otherScript = GetComponent<DangerousObject>();
                otherScript.isDangerous = true;

            }
            else
            {
                DangerousObject otherScript = GetComponent<DangerousObject>();
                otherScript.isDangerous = false;
            }
            */

            currentWidth = currentWidth - (Time.deltaTime * speed_right);
            if (currentWidth <= minWidth)
            {
                currentWidth = minWidth;
            }
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, currentWidth);

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
}
