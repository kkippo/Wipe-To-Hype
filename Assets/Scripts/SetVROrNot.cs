using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetVRorNot : MonoBehaviour
{
    public GameObject XRRig;
    public GameObject WebGLRig;
    private bool isOnXRDevice = false;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            isOnXRDevice = true;
        }
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            isOnXRDevice = false;
        }
        XRRig.SetActive(isOnXRDevice);
        WebGLRig.SetActive(!isOnXRDevice);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
