using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightTransition : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxLight = 3f;
    public float fadeInTime = 3f;
    public float fadeOutTime = 3f;
    public bool fullBright = false;
    Light2D globalLight;
    public static LightTransition current;
    public bool fadingOut = false;
    public bool fadedOut = false;
    void Start()
    {
        current = this;
        globalLight = GetComponent<Light2D>();
        globalLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (fullBright == false)
        {
            globalLight.intensity += Time.deltaTime*maxLight/fadeInTime;
            if (globalLight.intensity >= maxLight)
            {
                globalLight.intensity = maxLight;
                fullBright = true;
            }
        }
        if (fadingOut)
        {
            globalLight.intensity -= Time.deltaTime * maxLight / fadeOutTime;
            if (globalLight.intensity <= 0)
            {
                globalLight.intensity = 0;
                fadingOut = false;
                fadedOut = true;
            }
        }
    }

    public void StartFadeOut(float setFadeOutTime)
    {
        fullBright = true;
        globalLight.intensity = maxLight;

        fadingOut = true;
        fadeOutTime = setFadeOutTime;
       
    }
}
