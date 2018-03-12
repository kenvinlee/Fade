using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class CubeHeight : MonoBehaviour {

    /*************************************************************************************************
    The purpose of this script is to change the size of a cube shaped platform only in the y direction.

        When the light is on, the cube's height raises from current height upwards)
        When the light is off the cube's height lowers from current height downwards, drawing to the 
        ground level, allowing the player to align platforms by turning the light on and off.  

        Upwards and downwards speeds of the platforms can also be adjusted to adjust difficulty in 
        alighning platforms for the player.

        
        known bugs:  if you are under a platform that comes down to the ground you get pushed (squeezed) 
        through the plane.  
        
    *************************************************************************************************/

    
	

    bool animated = false;
    
    float currentHeight = 1.0f;      // the current size of the cube (should match minHeight)
    float old_height_value = 0.0f;

    public float minHeight = 1.0F;          // the smallest height for the cube.  (set to its original y-height)
    public float maxHeight = 50.0F;         // the maximum height of the cube (y-direction)
    public float speed_upwards = 5.0f;      // the speed of the height change upwards (larger the number, the faster it goes)
    public float speed_downwards = 5.0f;    // the speed of the height change downwards 
    //public float damageSize = 1.0f;       // the height when the cube is dangerous
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
        
    void Update () {
        if (animated) {

            currentHeight = currentHeight + (Time.deltaTime * speed_upwards);

            if (currentHeight > maxHeight)
            {
                currentHeight = maxHeight;
            }
            if (old_height_value != currentHeight)
            {
                transform.localScale = new Vector3(transform.localScale.x, currentHeight, transform.localScale.z);
                old_height_value = currentHeight;
            }
            //if (!(GetComponent<AudioSource>().isPlaying))
            //{
            //    GetComponent<AudioSource>().Play();
            //}

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
        if (!animated)
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

            currentHeight = currentHeight - (Time.deltaTime * speed_downwards);
            if (currentHeight < minHeight)
            {
                currentHeight = minHeight;
            }
            if (old_height_value != currentHeight)
            {
                transform.localScale = new Vector3(transform.localScale.x, currentHeight, transform.localScale.z);
                old_height_value = currentHeight;
            }
            //transform.localScale = new Vector3(transform.localScale.x, currentHeight, transform.localScale.z );

            //if (GetComponent<AudioSource>().isPlaying)
            //{
            //    GetComponent<AudioSource>().Stop();
            //}


                if (sndExists && soundEffects != null && soundEffects.Length != 0)
                {
                    soundEffects[0].Pause();
                    isPaused = true;

                }

        }
    }

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Orb") {
			animated = true;
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.tag == "Orb") {
			animated = false;
		}
	}
}
