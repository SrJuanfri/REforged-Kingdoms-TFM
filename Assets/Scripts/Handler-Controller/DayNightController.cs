﻿using UnityEngine;
using System.Collections;

public enum DayStages
{
    Dawn,
    Day,
    Dusk,
    Night
}

public class DayNightController : MonoBehaviour
{
    [Header("Current World Time")]
    public WorldTime LocalTime;

    /// <summary>
    /// The number of seconds in one full game day.
    /// </summary>
    public int SecondsInFullDay = 120;

    [Header("Time Of Day")]
    public WorldTime StartTime;
    public WorldTime DawnTime;
    public WorldTime DayTime;
    public WorldTime DuskTime;
    public WorldTime NightTime;

    [Header("Ambient Colours")]
    public Color DuskAmbience;
    public Color DayAmbience;
    public Color DawnAmbience;
    public Color NightAmbience;

    [Header("Fog Colours")]
    public Color DawnFog;
    public Color DayFog;
    public Color DuskFog;
    public Color NightFog;

    [Header("Current Day Stage")]
    public DayStages _currentStage;

    [Header("Sun/Moon")]
    public Transform Player;
    public float SunMoonRotationRadius = 20;

    [Header("Sun")]
    public Flare SunFlare;
    public float SunFlareBrightness = 0.5f;
    public float SunFlareFadeSpeed = 10f;
    public float MaxSunlightIntensity = 2.4f;
    public Color SunLightColour = new Color(1, 1, 0.5f);
    public Vector3 SunScale;
    public Material SunMaterial;

    [Header("Moon")]
    public Vector3 MoonScale;
    public Material MoonMaterial;

    private float _timeMultiplier = 1f;
    public float TimeMultiplier { get { return _timeMultiplier; } set { _timeMultiplier = value; } }

    private float _dawnTime = 0.20f;
    private float _dayTime = 0.25f;
    private float _duskTime = 0.70f;
    private float _nightTimePre = 0.23f;
    private float _nightTimePost = 0.75f;

    private float _initialSunIntensity;
    private GameObject _sun, _moon;
    private float _currentTimeOfDay = 0;
    private float _skyBoxBlend = 0;

    public float CurrentTimeOfDay { get { return _currentTimeOfDay; } set { _currentTimeOfDay = value; } }

    [HideInInspector]
    public bool dayStarted = false;

    [HideInInspector]
    public bool stop = false;

    private void Awake()
    {
        _initialSunIntensity = MaxSunlightIntensity;
        LocalTime = new WorldTime();

        _dawnTime = WorldTimeToInt(DawnTime);
        _dayTime = WorldTimeToInt(DayTime);
        _duskTime = WorldTimeToInt(DuskTime);
        _nightTimePre = _dawnTime - 0.03f;
        _nightTimePost = WorldTimeToInt(NightTime);

        _currentTimeOfDay = WorldTimeToInt(LocalTime);

        if ((_nightTimePost < _duskTime) || (_duskTime < _dayTime) || (_dayTime < _dawnTime))
        {
            Debug.LogError("Day stages have not been correctly setup, disabling script.");
            enabled = false;
        }

        CreateSun();
        CreateMoon();
        UpdateWorldTime();

        SetDayNightType();
        UpdateLightIntensity();
        UpdateFog();
        UpdateAmbientLightColour();
        UpdateSkyBox();
        UpdateSunPosition();
    }

    public void StartDayCycle()
    {
        dayStarted = true;
    }

    void Update()
    {
        if (dayStarted && !stop)
        {
            UpdateWorldTime();
            SetDayNightType();
            UpdateLightIntensity();
            UpdateFog();
            UpdateAmbientLightColour();
            UpdateSkyBox();

            _currentTimeOfDay += (Time.deltaTime / SecondsInFullDay) * _timeMultiplier;

            if (_currentTimeOfDay >= 1)
            {
                _currentTimeOfDay = 1;
                stop = true;  // Detener el ciclo al finalizar el día
            }
        }
    }

    private float WorldTimeToInt(WorldTime time)
    {
        return (time.Hours + (time.Minutes / 60f)) / 24f;
    }

    private void CreateSun()
    {
        _sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sun.name = "sun";
        _sun.GetComponent<SphereCollider>().enabled = false;
        _sun.transform.localScale = SunScale;

        var renderer = _sun.GetComponent<Renderer>();
        renderer.material.color = Color.yellow;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        var light = _sun.AddComponent<Light>();
        light.enabled = false;
        light.type = LightType.Directional;
        light.shadows = LightShadows.Soft;
        light.color = SunLightColour;
        light.intensity = 2f;

        if (SunFlare)
        {
            var sunFlare = _sun.AddComponent<LensFlare>();
            sunFlare.flare = SunFlare;
            sunFlare.brightness = SunFlareBrightness;
        }

        light.enabled = true;
    }

    private void CreateMoon()
    {
        _moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _moon.name = "moon";
        _moon.GetComponent<SphereCollider>().enabled = false;
        _moon.transform.localScale = MoonScale;

        var renderer = _moon.GetComponent<Renderer>();
        renderer.material.color = new Color(0.75f, 0.75f, 0.75f);
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        var light = _moon.AddComponent<Light>();
        light.type = LightType.Directional;
        light.shadows = LightShadows.Soft;
        light.color = new Color(0.5f, 0.5f, 0.5f);
        light.intensity = 0.1f;
    }

    private void SetDayNightType()
    {
        if (_currentTimeOfDay <= _nightTimePre || _currentTimeOfDay >= _nightTimePost)
        {
            _currentStage = DayStages.Night;
        }
        else if (_currentTimeOfDay >= _duskTime)
        {
            _currentStage = DayStages.Dusk;
        }
        else if (_currentTimeOfDay >= _dayTime)
        {
            _currentStage = DayStages.Day;
        }
        else if (_currentTimeOfDay >= _dawnTime)
        {
            _currentStage = DayStages.Dawn;
        }
    }

    private void UpdateLightIntensity()
    {
        _sun.transform.localRotation = Quaternion.Euler((_currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;

        switch (_currentStage)
        {
            case DayStages.Dawn:
                intensityMultiplier = Mathf.Clamp01((_currentTimeOfDay - _dawnTime) * (1 / 0.02f));
                break;
            case DayStages.Day:
                intensityMultiplier = Mathf.Clamp01(1 - (CurrentTimeOfDay - 0.73f) * (1 / 0.02f));
                break;
            case DayStages.Dusk:
                intensityMultiplier = Mathf.Clamp01(1 - ((_currentTimeOfDay - _duskTime) * (1 / 0.02f)));
                break;
            case DayStages.Night:
                intensityMultiplier = 0;
                break;
        }

        _sun.GetComponent<Light>().intensity = _initialSunIntensity * intensityMultiplier;
    }

    private void UpdateFog()
    {
        float relativeTime;
        switch (_currentStage)
        {
            case DayStages.Dawn:
                RenderSettings.fogColor = Color.Lerp(NightFog, DawnFog, Mathf.Clamp01((_currentTimeOfDay - _dawnTime) * (1 / 0.02f)));
                break;
            case DayStages.Day:
                relativeTime = _currentTimeOfDay - _dayTime;
                RenderSettings.fogColor = Color.Lerp(DawnFog, DayFog, relativeTime / (0.25f + 0.125f)); // Remplazando con valores directos
                break;
            case DayStages.Dusk:
                RenderSettings.fogColor = Color.Lerp(DayFog, DuskFog, Mathf.Clamp01(((_currentTimeOfDay - _duskTime) * (1 / 0.02f))));
                break;
            case DayStages.Night:
                relativeTime = (_currentTimeOfDay >= _nightTimePost) ? (_currentTimeOfDay - _nightTimePost) / 0.125f : 1f; // Remplazando con valores directos
                RenderSettings.fogColor = Color.Lerp(DuskFog, NightFog, relativeTime);
                break;
        }
    }

    private void UpdateAmbientLightColour()
    {
        float relativeTime;
        switch (_currentStage)
        {
            case DayStages.Dawn:
                RenderSettings.ambientLight = Color.Lerp(NightAmbience, DawnAmbience, Mathf.Clamp01((_currentTimeOfDay - _dawnTime) * (1 / 0.02f)));
                break;
            case DayStages.Day:
                relativeTime = _currentTimeOfDay - _dayTime;
                RenderSettings.ambientLight = Color.Lerp(DawnAmbience, DayAmbience, relativeTime / (0.25f + 0.125f)); // Remplazando con valores directos
                break;
            case DayStages.Dusk:
                RenderSettings.ambientLight = Color.Lerp(DayAmbience, DuskAmbience, Mathf.Clamp01(((_currentTimeOfDay - _duskTime) * (1 / 0.02f))));
                break;
            case DayStages.Night:
                relativeTime = (_currentTimeOfDay >= _nightTimePost) ? (_currentTimeOfDay - _nightTimePost) / 0.125f : 1f; // Remplazando con valores directos
                RenderSettings.ambientLight = Color.Lerp(DuskAmbience, NightAmbience, relativeTime);
                break;
        }
    }

    private void UpdateSkyBox()
    {
        float totalDayDuration = 1f - _dawnTime; // Duración total del día desde el amanecer hasta el final del día
        _skyBoxBlend = Mathf.Clamp01((_currentTimeOfDay - _dawnTime) / totalDayDuration);
        RenderSettings.skybox.SetFloat("_Blend", _skyBoxBlend);
    }

    private void UpdateSunPosition()
    {
        Vector3 midpoint = Player.position;
        midpoint.y -= 0.5f;

        float sunAngle = (_currentTimeOfDay - _dawnTime) * 360;
        _sun.transform.position = midpoint + Quaternion.Euler(0, 0, sunAngle) * (SunMoonRotationRadius * Vector3.right);
        _sun.transform.LookAt(midpoint);

        float moonAngle = ((_currentTimeOfDay - _duskTime) * 360);
        _moon.transform.position = midpoint + Quaternion.Euler(0, 0, moonAngle) * (SunMoonRotationRadius * Vector3.right);
        _moon.transform.LookAt(midpoint);
    }

    public void ResetDayCycle()
    {
        _currentTimeOfDay = 0;
        stop = false;
        dayStarted = false;
        UpdateWorldTime();
        UpdateSkyBox();
        UpdateSunPosition();
        UpdateFog();
        UpdateAmbientLightColour();
        UpdateLightIntensity();
    }

    /// Updates the World-time hour based on the current time of day.  
    private void UpdateWorldTime()
    {
        var fHours = 24 * _currentTimeOfDay;
        var hours = (int)fHours;
        var minutes = (int)(60 * (fHours - Mathf.Floor(fHours)));

        LocalTime.SetTime(hours, minutes);
    }
}
