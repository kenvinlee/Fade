using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AfterAnimation : MonoBehaviour {
    //public Animator animator;
    //public static int size = 1;
    public List<GameObject> objectlist = new List<GameObject>();
    public List<GameObject> changedobjectlist = new List<GameObject>();
    public Material newMaterial;
    private List<Animator> animatorlist = new List<Animator> ();
    private Animator _animatorComponent;
    private List<bool> boollist = new List<bool>();

    // Use this for initialization
    AnimatorStateInfo state;
    void Start () {
        _animatorComponent = GetComponent<Animator>();
        //Debug.Log(objectlist.Count);
        //Debug.Log(animatorlist[0]);
        for (int i = 0; i < objectlist.Count; i++) {
            animatorlist.Add(objectlist[i].GetComponent<Animator>());
            boollist.Add(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        
        for (int i = 0; i < objectlist.Count; i++) {
            state = animatorlist[i].GetCurrentAnimatorStateInfo(0);
            if (state.fullPathHash == Animator.StringToHash("Base Layer.Done") && !boollist[i])
            {
                //Debug.Log("hello " + i);
                changedobjectlist[i].GetComponent<Renderer>().material = newMaterial;
                boollist[i] = true;
            }
        }
        if (!boollist.Contains(false)) {
            _animatorComponent.SetTrigger(Animator.StringToHash("Open"));
            //Debug.Log("got all of them");
        }
    }
}
