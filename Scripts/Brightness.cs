using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;


// This script is used to adjust the brightness of the game. This can be done through the brightness slider in the options menu.
public class Brightness : MonoBehaviour
{
    // This is the slider that is used to adjust the brightness.
    public Slider brightnessSlider;

    // This is the post processing profile that is used to adjust the brightness.
    public PostProcessProfile brightness;

    // This is the post processing layer that is used to adjust the brightness.
    public PostProcessLayer layer;

    AutoExposure exposure;
    // Start is called before the first frame update
    void Start()
    {
        // This is used to get the post processing profile.
        brightness.TryGetSettings(out exposure);
    }


    // This is used to adjust the brightness based on the value
    public void AdjustBrightness(float value)
    {
        if (value != 0)
        {
            exposure.keyValue.value = value;
        }
        else 
        {
            exposure.keyValue.value = .05f;
        }
    }
}
