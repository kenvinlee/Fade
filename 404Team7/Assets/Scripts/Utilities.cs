using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Purpose of this Class

Static Settings for global variables (names)
--------------------------------------------------------------------------------------------------------------------------------------
Most of the joystick settings /defintions are here.   
not sure where left joystick is read.  (same settings on windows and on mac)

for y button reading look at PlayerMovement.cs

mac joystick settings can be observed in unity by Edit --> project settings --> Input.  (inspector)

---------------------------------------------------------------------------------------------------------------------------------------
NOTES

two duplicate versions of this class:  one for windows, one for mac to address controller issues (different mappings for each system)
testing system type at runtime didn't seem to work, using conditional compilation.
if updating this file be sure to update both versions where settings are the same (mac and windows). 
joystick control is now working on the mac.

//http://wiki.unity3d.com/index.php?title=Xbox360Controller  (reference for xbox360 joystick settings in unity)

*/

//windows code here

#if UNITY_STANDALONE_WIN

public static class Utilities{
	//Value Settings/Preferences
	public static int MIN_LIGHT_RADIUS = 1;
	public static int MAX_LIGHT_RADIUS = 8;


	//Names
	public static string LIGHT_NAME = "LightOrb";
	public static string PLAYER_CAMERA_NAME = "PlayerCamera";
	public static string PLAYER_Holding = "PlayerHolding";
	public static string INTERACTABLE_LAYER_NAME = "Interactable";
	public static string DEFAULT_LAYER_NAME = "Default";
	public static string DANGEROUS_OBJECT_TAG = "Dangerous";
	public static string GUI_CAMERA_NAME = "GUI_Camera";

    //Inputs

    public static string JUMP_INPUT = "Jump";  //button 3 = 'Y' button.
	public static string JOYSTICK_RIGHT_HOR = "Rightstick_Horizontal";
	public static string JOYSTICK_RIGHT_VER = "Rightstick_Vertical";
	public static string EXPAND_LIGHT_JOYSTICK_AXIS = "Right_Trigger";  // right triggger    
	public static KeyCode INTERACT_JOYSTICK_BUTTON = KeyCode.Joystick1Button4; // left bumper
	public static KeyCode INTERACT_KEYBOARD_BUTTON = KeyCode.F;
	public static KeyCode EXPAND_LIGHT_JOYSTICK_BUTTON = KeyCode.Joystick1Button5;  // right bumper

    //Settings
    public static int MAX_PLAYER_HEALTH = 100;


	public static float getCurrentLightRadius(){
		Collider _light_collider = GameObject.Find (Utilities.LIGHT_NAME).GetComponent<Collider>();
		return _light_collider.bounds.size.x / 2;
	}

	public static bool isObjectInSight(GameObject obj){
		Collider _light_collider = GameObject.Find (Utilities.LIGHT_NAME).GetComponent<Collider>();
		float dist = Vector3.Distance (_light_collider.transform.position, obj.transform.position);
		float light_radius = _light_collider.bounds.size.x / 2;
		return dist < light_radius;
	}

	public static bool isObjectInScene(string name){
		return GameObject.Find (name) != null;
	}

	public static GameObject getGUICamera(){
		return GameObject.Find (GUI_CAMERA_NAME);
	}
}

#endif

//OSX code here

#if UNITY_STANDALONE_OSX

public static class Utilities
{
    //Value Settings/Preferences
    public static int MIN_LIGHT_RADIUS = 1;
    public static int MAX_LIGHT_RADIUS = 8;


    //Names
    public static string LIGHT_NAME = "LightOrb";
    public static string PLAYER_CAMERA_NAME = "PlayerCamera";
    public static string PLAYER_Holding = "PlayerHolding";
    public static string INTERACTABLE_LAYER_NAME = "Interactable";
    public static string DEFAULT_LAYER_NAME = "Default";
    public static string DANGEROUS_OBJECT_TAG = "Dangerous";
    public static string GUI_CAMERA_NAME = "GUI_Camera";

    //Inputs

    public static string JUMP_INPUT = "Jump_Mac";  //y button probably has different mapping than windows (button 19)

    // leftstick mapping is the same on windows as on the mac, not sure where definitions are.

    public static string EXPAND_LIGHT_JOYSTICK_AXIS = "Right_Trigger_Mac";  // right triggger 
    public static string JOYSTICK_RIGHT_VER = "Rightstick_Vertical_Mac";  // right xbox 360 controller has different mapping than on windows
	public static string JOYSTICK_RIGHT_HOR = "Rightstick_Horizontal_Mac";
	public static KeyCode INTERACT_JOYSTICK_BUTTON = KeyCode.Joystick1Button13; // left bumper
	public static KeyCode INTERACT_KEYBOARD_BUTTON = KeyCode.F;  // keyboard controll for light (same as right bumper)
	public static KeyCode EXPAND_LIGHT_JOYSTICK_BUTTON = KeyCode.Joystick1Button14;  // right bumper

    
    //Settings
    public static int MAX_PLAYER_HEALTH = 100;


    public static float getCurrentLightRadius()
    {
        Collider _light_collider = GameObject.Find(Utilities.LIGHT_NAME).GetComponent<Collider>();
        return _light_collider.bounds.size.x / 2;
    }

    public static bool isObjectInSight(GameObject obj)
    {
        Collider _light_collider = GameObject.Find(Utilities.LIGHT_NAME).GetComponent<Collider>();
        float dist = Vector3.Distance(_light_collider.transform.position, obj.transform.position);
        float light_radius = _light_collider.bounds.size.x / 2;
        return dist < light_radius;
    }

    public static bool isObjectInScene(string name)
    {
        return GameObject.Find(name) != null;
    }

    public static GameObject getGUICamera()
    {
        return GameObject.Find(GUI_CAMERA_NAME);
    }
}
#endif



