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
    [Tooltip("Minimum random light intensity")]
    [SerializeField] private float minIntensity = 0f;
    [Tooltip("Maximum random light intensity")]
    [SerializeField] private float maxIntensity = 66f;
    [Tooltip("# light dims per second")]
    [SerializeField] private float dimfrequency = 5f;
    [Tooltip("% of the time the light is dimming")]
    [SerializeField] public int dimPercentage = 100;

    private float dimTarget;
    private float timeLastDim;
    private DimPhase_ dimPhase = DimPhase_.NotDimming;
    private float dimStep;

    public float MaxIntensity { get => maxIntensity; set => maxIntensity = value; }

    void Start()
    {
        light = GetComponent<Light>();
        timeLastDim = Time.time;
    }

    void FixedUpdate()
    {
        if (maxIntensity<=0)
        {
            return;
        }
        if (dimPhase == DimPhase_.NotDimming && Time.time - timeLastDim > (1/dimfrequency))
        {
            timeLastDim = Time.time;
            dimTarget = Random.Range(minIntensity, maxIntensity);
            dimPhase = DimPhase_.Fainter;
            //  [dimfrequency] * [updates/sec] * (fainting and brightening) * [intensity range] * [100/dimPercentage]
            dimStep = dimfrequency * 0.02f * 2 * System.Math.Abs(light.intensity - dimTarget) * (100/(float)dimPercentage);
        }

        if (dimPhase == DimPhase_.Fainter)
        {
            light.intensity -= dimStep;
            if (light.intensity <= dimTarget)
            {
                light.intensity = dimTarget;
                dimPhase = DimPhase_.Brighter;
            }
        }

        if (dimPhase == DimPhase_.Brighter)
        {
            light.intensity += dimStep;
            if (light.intensity >= maxIntensity)
            {
                light.intensity = maxIntensity;
                dimPhase = DimPhase_.NotDimming;
            }
        }

    }

}
