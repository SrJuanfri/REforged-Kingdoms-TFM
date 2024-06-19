using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;

    [SerializeField] private Gradient gradientNightToSunrise;
    [SerializeField] private Gradient gradientSunriseToDay;
    [SerializeField] private Gradient gradientDayToSunset;
    [SerializeField] private Gradient gradientSunsetToNight;
    [SerializeField] private Light globalLight;


    [SerializeField] private float timeMultiplier;

    public int hours;
    public int Hours
    { get { return hours; } set { hours = value; OnHoursChange(value); } }

    public int minutes;
    public int Minutes
    { get { return minutes; } set { minutes = value; OnMinutesChange(value); } }

    public int days;
    public int Days 
    { get {return days; } set { days = value;} }

    public float tempSeconds;

    private void Awake()
    {
        RenderSettings.skybox.SetTexture("_Texture1", skyboxNight);
        RenderSettings.skybox.SetTexture("_Texture2", skyboxDay);
        RenderSettings.skybox.SetFloat("_Blend", 0f);
    }

    public void Update()
    {
        tempSeconds += Time.deltaTime * timeMultiplier;

        if(tempSeconds >= 1)
        {
            Minutes += 1;
            tempSeconds = 0;
        }
    }

    private void OnMinutesChange(int value)
    {
        globalLight.transform.Rotate(Vector3.up, (1f/1440f)*360f, Space.World);
        if (value >= 60)
        {
            Hours++;
            minutes = 0;
        }
        if (Hours >= 24)
        { 
            Hours = 0;
            Days++;
        }
    }
    private void OnHoursChange(int value)
    {
        if(value == 6)
        {
            StartCoroutine(LerpSkybox(skyboxNight,skyboxSunrise,10f));
            StartCoroutine(LerpLight(gradientNightToSunrise, 10f));
        }
        else if (value == 8)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 10f));
            StartCoroutine(LerpLight(gradientSunriseToDay, 10f));
        }
        else if (value == 18)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 10f));
            StartCoroutine(LerpLight(gradientDayToSunset, 10f));
        }
        else if (value == 22)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 10f));
            StartCoroutine(LerpLight(gradientSunsetToNight, 10f));
        }
        
    }

    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        RenderSettings.skybox.SetTexture("_Texture1", a); 
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0f);

        for (float i=0; i<time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("_Texture1", b);
    }

    private IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            globalLight.color = lightGradient.Evaluate(i / time);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
    }
    
    
    
//Cambio de código


    /*
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset lightingPreset;

    [SerializeField, Range(0, 12)] private float timeOfDay;


    private void Update()
    {
        if(lightingPreset == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime;
            timeOfDay %= 24;
            UpdateLighting(timeOfDay);
        }
        else
        {
            UpdateLighting(timeOfDay / 12f);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = lightingPreset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = lightingPreset.FogColor.Evaluate(timePercent);

        if(directionalLight != null)
        {
            directionalLight.color = lightingPreset.DirectionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 180f) - 90f, 170f, 0));
        }
    }
    
    private void OnValidate()
    {
        if (directionalLight != null)
            return;
        if(RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>(); 
            foreach(Light light in lights)
            {
                directionalLight = light;
            }
        }
    }
    */
}
