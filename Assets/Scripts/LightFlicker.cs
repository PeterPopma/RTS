using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Component which will flicker a linked light while active by changing its
/// intensity between the min and max values given. The flickering can be
/// sharp or smoothed depending on the value of the smoothing parameter.
///
/// Just activate / deactivate this component as usual to pause / resume flicker
/// </summary>


public class LightFlicker : MonoBehaviour
{
    private enum DimPhase_ { NotDimming, Fainter, Brighter }; 
    
    private new Light light;
    [Tooltip("Default light intensity")]
    [SerializeField] private float defaultIntensity = 100f;
    [Tooltip("Maximum light intensity (can also be a minimum)")]
    [SerializeField] private float maxIntensity = 0f;
    [Tooltip("# light dims per second")]
    [SerializeField] private float dimfrequency = 2f;
    [Tooltip("How long it takes to dim the light (s)")]
    [SerializeField] public float dimDuration = 0.5f;
    [Tooltip("Randomness %: 0=constant,100=random")]
    [SerializeField] public int dimRandomness = 50;

    private float dimTargetValue;
    private float timeLastDim;

    public float MaxIntensity { get => maxIntensity; set => maxIntensity = value; }
    public float DefaultIntensity { get => defaultIntensity; set => defaultIntensity = value; }

    void Start()
    {
        light = GetComponent<Light>();
        timeLastDim = Time.time;
    }

    void FixedUpdate()
    {
        // start a new dim
        if (Time.time - timeLastDim > (1/dimfrequency))
        {
            timeLastDim = Time.time;
            dimTargetValue = ((100-dimRandomness) * maxIntensity + dimRandomness * Random.Range(defaultIntensity, maxIntensity)) / 100.0f;
        }

        float percentageComplete = (Time.time - timeLastDim) / dimDuration;

        // first half of dimming
        if (percentageComplete < .5)
        {
            light.intensity = (defaultIntensity * (.5f - percentageComplete) + dimTargetValue * percentageComplete) / .5f;
        }
        else if (percentageComplete >= .5 && percentageComplete < 1)
        {
            light.intensity = (defaultIntensity * (percentageComplete - .5f) + dimTargetValue * (1f - percentageComplete)) / .5f;
        }
        else
        {
            light.intensity = defaultIntensity;
        }
    }

}
