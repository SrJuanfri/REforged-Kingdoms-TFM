using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro; // Asegúrate de usar TextMeshPro

public class SettingsManager : MonoBehaviour
{
    [Header("Graphics Settings")]
    public TMP_Dropdown qualityDropdown;
    public Toggle fogToggle;
    public Toggle bloomToggle;
    public Toggle shadowsToggle;
    public Toggle antialiasingToggle;
    public Toggle vsyncToggle;

    [Header("Resolution Settings")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider voiceVolumeSlider;

    [Header("Control Buttons")]
    public Button closeButton;
    public Button resetButton;
    public Button saveButton;

    [Header("Translation")]
    public bool autoTranslateTexts = true;

    private Dictionary<string, string> translationDictionary = new Dictionary<string, string>()
    {
        {"Play", "Jugar"}, {"Settings", "Configuración"}, {"Exit", "Salir"},
        {"Graphics", "Gráficos"}, {"Audio", "Audio"}, {"Resolution", "Resolución"},
        {"Quality", "Calidad"}, {"Low", "Bajo"}, {"Medium", "Medio"}, {"High", "Alto"},
        {"Master Volume", "Volumen Maestro"}, {"Music Volume", "Volumen de Música"},
        {"SFX Volume", "Volumen de Efectos"}, {"Voice Volume", "Volumen de Voces"}
        // ... Añade otras traducciones aquí
    };

    private void Awake()
    {
        // Buscar un AudioMixer existente si no está asignado
        if (audioMixer == null)
        {
            AudioMixer[] mixers = Resources.FindObjectsOfTypeAll<AudioMixer>();
            if (mixers.Length > 0)
            {
                audioMixer = mixers[0];
                DontDestroyOnLoad(audioMixer);
            }
            else
            {
                Debug.LogError("No se encontró ningún AudioMixer en el proyecto. Por favor, cree uno en el Editor de Unity y asígnelo.");
            }
        }
    }

    private void Start()
    {
        // Verificar y conectar sliders a los eventos correctos
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            Debug.Log("Music Volume Slider is assigned.");
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        else
        {
            Debug.LogError("Music Volume Slider is NOT assigned!");
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (voiceVolumeSlider != null)
        {
            voiceVolumeSlider.onValueChanged.AddListener(SetVoiceVolume);
        }

        // Configuración de calidad
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Configuración de resolución
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Configuración de volumen
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 0.75f);

        // Aplicar configuraciones actuales
        ApplyGraphicsSettings();
        SetResolution(currentResolutionIndex);
        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);
        SetVoiceVolume(voiceVolumeSlider.value);

        // Asignar funcionalidad a los botones
        closeButton.onClick.AddListener(CloseSettings);
        resetButton.onClick.AddListener(ResetSettings);
        saveButton.onClick.AddListener(SaveSettings);
    }

    // Función para aplicar configuración gráfica
    public void ApplyGraphicsSettings()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        RenderSettings.fog = fogToggle.isOn;
        QualitySettings.shadows = shadowsToggle.isOn ? ShadowQuality.All : ShadowQuality.Disable;
        QualitySettings.antiAliasing = antialiasingToggle.isOn ? 4 : 0;
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
    }

    // Función para cambiar la resolución
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Función para configurar el volumen maestro
    public void SetMasterVolume(float volume)
    {
        Debug.Log("Master Volume Slider value: " + volume);

        if (audioMixer != null)
        {
            if (volume <= 0.0001f)
            {
                audioMixer.SetFloat("MasterVolume", -80f);  // Silencio total
                Debug.Log("Master volume set to -80dB (muted).");
            }
            else
            {
                float dbValue = Mathf.Log10(volume) * 20;
                audioMixer.SetFloat("MasterVolume", dbValue);
                Debug.Log("Master volume set to: " + dbValue + " dB.");
            }
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
        else
        {
            Debug.LogError("AudioMixer is null!");
        }
    }

    // Función para configurar el volumen de la música
    public void SetMusicVolume(float volume)
    {
        Debug.Log("Music Volume Slider value: " + volume);

        if (audioMixer != null)
        {
            if (volume <= 0.0001f)
            {
                audioMixer.SetFloat("MusicVolume", -80f);  // Silencio total
                Debug.Log("Music volume set to -80dB (muted).");
            }
            else
            {
                float dbValue = Mathf.Log10(volume) * 20;
                audioMixer.SetFloat("MusicVolume", dbValue);
                Debug.Log("Music volume set to: " + dbValue + " dB.");
            }
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
        else
        {
            Debug.LogError("AudioMixer is null!");
        }
    }

    // Función para configurar el volumen de los efectos de sonido
    public void SetSFXVolume(float volume)
    {
        Debug.Log("SFX Volume Slider value: " + volume);

        if (audioMixer != null)
        {
            if (volume <= 0.0001f)
            {
                audioMixer.SetFloat("SFXVolume", -80f);  // Silencio total
                Debug.Log("SFX volume set to -80dB (muted).");
            }
            else
            {
                float dbValue = Mathf.Log10(volume) * 20;
                audioMixer.SetFloat("SFXVolume", dbValue);
                Debug.Log("SFX volume set to: " + dbValue + " dB.");
            }
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
        else
        {
            Debug.LogError("AudioMixer is null!");
        }
    }

    // Función para configurar el volumen de las voces
    public void SetVoiceVolume(float volume)
    {
        Debug.Log("Voice Volume Slider value: " + volume);

        if (audioMixer != null)
        {
            if (volume <= 0.0001f)
            {
                audioMixer.SetFloat("VoiceVolume", -80f);  // Silencio total
                Debug.Log("Voice volume set to -80dB (muted).");
            }
            else
            {
                float dbValue = Mathf.Log10(volume) * 20;
                audioMixer.SetFloat("VoiceVolume", dbValue);
                Debug.Log("Voice volume set to: " + dbValue + " dB.");
            }
            PlayerPrefs.SetFloat("VoiceVolume", volume);
        }
        else
        {
            Debug.LogError("AudioMixer is null!");
        }
    }

    // Función para cerrar el menú de configuración
    public void CloseSettings()
    {
        Debug.Log("Settings menu closed.");
        // Puedes agregar lógica para cerrar el menú aquí
    }

    // Función para restablecer las configuraciones a los valores predeterminados
    public void ResetSettings()
    {
        qualityDropdown.value = QualitySettings.names.Length - 1;
        fogToggle.isOn = true;
        bloomToggle.isOn = true;
        shadowsToggle.isOn = true;
        antialiasingToggle.isOn = true;
        vsyncToggle.isOn = true;
        resolutionDropdown.value = resolutions.Length - 1;

        masterVolumeSlider.value = 0.75f;
        musicVolumeSlider.value = 0.75f;
        sfxVolumeSlider.value = 0.75f;
        voiceVolumeSlider.value = 0.75f;

        ApplyGraphicsSettings();
        SetResolution(resolutionDropdown.value);
        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);
        SetVoiceVolume(voiceVolumeSlider.value);
    }

    // Función para guardar las configuraciones actuales
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualityLevel", qualityDropdown.value);
        PlayerPrefs.SetInt("FogEnabled", fogToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("BloomEnabled", bloomToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("ShadowsEnabled", shadowsToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("AntialiasingEnabled", antialiasingToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("VSyncEnabled", vsyncToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.Save();

        Debug.Log("Settings saved.");
    }

    // Función para traducir todos los textos en la escena
    private void TranslateAllTextsInScene()
    {
        // Encontrar todos los TextMeshProUGUI componentes en la escena
        TextMeshProUGUI[] tmpTexts = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI tmpText in tmpTexts)
        {
            TranslateText(tmpText);
        }
    }

    // Función que traduce el texto si está en inglés y existe en el diccionario
    private void TranslateText(TextMeshProUGUI tmpText)
    {
        if (translationDictionary.ContainsKey(tmpText.text))
        {
            tmpText.text = translationDictionary[tmpText.text];
        }
    }
}
