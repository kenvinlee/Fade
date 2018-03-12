using System;
using UnityEngine;

public class LightOrbController : MonoBehaviour
{

    public float minRadius = 1f;
    public float maxRadius = 6f;
    public float expandTime = 1.5f;
    public float contractTime = 1f;
    public AnimationCurve expandCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
    public AnimationCurve contractCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
    public KeyCode expandLightKey = KeyCode.Mouse0;
    public float radiusSmoothing = 0.1f;

    float o_orbRadius = 0f;
    bool o_orbExpanding = false;
    bool o_orbContracting = false;
    float radiusDiff = 0f;
    float targetRadius = 1f;
    float keyDownTimmer = 0f;
    bool scriptLoaded = false;

    Light lightOrb;
    //	SphereCollider lightCol;
    AnimationCurve o_ExpandCurve;
    AnimationCurve o_ContractCurve;

    private AudioSource[] lightOnAudio;

    public float orbRadius
    {
        get { return o_orbRadius; }
    }

    public bool orbExpanding
    {
        get { return o_orbExpanding; }
    }

    public bool orbExpanded
    {
        get { return o_orbRadius > minRadius; }
    }

    public bool orbFullyExpanded
    {
        get { return o_orbRadius == maxRadius; }
    }

    public bool orbContracting
    {
        get { return o_orbContracting; }
    }

    public void SetOrbRadius(float r)
    {
        targetRadius = r;
    }


    // Wrap the class inputs into curves
    void ScaleCurves()
    {
        o_ExpandCurve = new AnimationCurve();
        o_ContractCurve = new AnimationCurve();
        float offset = minRadius;

        foreach (Keyframe k in expandCurve.keys)
        {
            Keyframe k2 = k;
            k2.time *= expandTime;
            k2.value = k.value * radiusDiff + offset;
            k2.inTangent *= radiusDiff;
            k2.outTangent *= radiusDiff;
            o_ExpandCurve.AddKey(k2);
        }
        foreach (Keyframe k in contractCurve.keys)
        {
            Keyframe k2 = k;
            k2.time *= contractTime;
            k2.value = k.value * radiusDiff + offset;
            k2.inTangent *= radiusDiff;
            k2.outTangent *= radiusDiff;
            o_ContractCurve.AddKey(k2);
        }
    }

    void ExpandOrb()
    {
        if (!o_orbExpanding)
        {
            o_orbContracting = false;
            o_orbExpanding = true;
            keyDownTimmer = 0.4f; // A very rough transition
        }
        if (keyDownTimmer < expandTime && keyDownTimmer >= 0f)
        {
            keyDownTimmer += Time.deltaTime;
            SetOrbRadius(o_ExpandCurve.Evaluate(keyDownTimmer));
        }
    }

	void ExpandOrb(float scale)
	{
		if (!o_orbExpanding)
		{
			o_orbContracting = false;
			o_orbExpanding = true;
			keyDownTimmer = 0.4f; // A very rough transition
		}
		if (keyDownTimmer < expandTime && keyDownTimmer >= 0f)
		{
			keyDownTimmer += Time.deltaTime*scale;
			SetOrbRadius(o_ExpandCurve.Evaluate(keyDownTimmer));
		}
	}

    void ContractOrb()
    {
        if (!o_orbContracting)
        {
            o_orbExpanding = false;
            o_orbContracting = true;
            keyDownTimmer = 0.4f;
        }
        if (keyDownTimmer < contractTime && keyDownTimmer >= 0f)
        {
            keyDownTimmer += Time.deltaTime;
            SetOrbRadius(o_ContractCurve.Evaluate(keyDownTimmer));
        }
    }


    void Start()
    {
        lightOrb = GetComponent<Light>();
        lightOrb.range = minRadius;
        //		lightCol = GetComponent<SphereCollider> ();
        radiusDiff = maxRadius - minRadius;
        ScaleCurves();
        scriptLoaded = true;

        if (GetComponents<AudioSource>() != null)
            lightOnAudio = GetComponents<AudioSource>();
    }

    void FixedUpdate()
    {
        if (scriptLoaded)
        {
			if (Input.GetKey(expandLightKey) || Input.GetAxis(Utilities.EXPAND_LIGHT_JOYSTICK_AXIS) > 0.1f)
            {
				if (Input.GetAxis (Utilities.EXPAND_LIGHT_JOYSTICK_AXIS) > 0) {
					ExpandOrb (Input.GetAxis (Utilities.EXPAND_LIGHT_JOYSTICK_AXIS));
				} else {
					ExpandOrb ();
				}
            }
            else
            {
                ContractOrb();
            }
        }
    }

    void Update()
    {
        float newR = Mathf.Lerp(lightOrb.range, targetRadius, radiusSmoothing);
        lightOrb.range = newR;
        transform.localScale = new Vector3(newR, newR, newR) * 2;
        o_orbRadius = newR;

        //float joy = 0.0f;
        //joy = Input.GetAxis(Utilities.EXPAND_LIGHT_JOYSTICK_AXIS);
        if (lightOnAudio.Length >= 2)
        {
            //Debug.Log("Right Trigger " + joy);
			if (Input.GetKeyDown(expandLightKey) || Input.GetAxis(Utilities.EXPAND_LIGHT_JOYSTICK_AXIS) >= 0.1f)
            {
                if (!(lightOnAudio[0].isPlaying || lightOnAudio[1].isPlaying))
                {
                    lightOnAudio[0].PlayOneShot(lightOnAudio[0].clip);
                    lightOnAudio[1].PlayDelayed(0.05f);
                }
            }
			if (Input.GetKeyUp(expandLightKey) || Input.GetAxis(Utilities.EXPAND_LIGHT_JOYSTICK_AXIS) == 0.0f)
            {
                if (lightOnAudio[0].isPlaying)
                {
                    lightOnAudio[0].Stop();
                    

                }
                else if (lightOnAudio[1].isPlaying)
                {
                    lightOnAudio[1].Stop();
                    lightOnAudio[0].PlayDelayed(0.05f);
                    lightOnAudio[0].Stop();
                }
            }
        }
        
    }
}