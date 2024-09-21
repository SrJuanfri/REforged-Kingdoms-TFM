using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro; // Aseg�rate de usar TextMeshPro

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
        // Diccionario de traducci�n ampliado
        {"Play", "Jugar"},
        {"Settings", "Configuraci�n"},
        {"Exit", "Salir"},
        {"Graphics", "Gr�ficos"},
        {"Audio", "Audio"},
        {"Resolution", "Resoluci�n"},
        {"Quality", "Calidad"},
        {"Low", "Bajo"},
        {"Medium", "Medio"},
        {"High", "Alto"},
        {"Very High", "Muy Alto"},
        {"Ultra", "Ultra"},
        {"Custom", "Personalizado"},
        {"Apply", "Aplicar"},
        {"Back", "Atr�s"},
        {"Main Menu", "Men� Principal"},
        {"Continue", "Continuar"},
        {"New Game", "Nuevo Juego"},
        {"Load Game", "Cargar Juego"},
        {"Save Game", "Guardar Juego"},
        {"Pause", "Pausa"},
        {"Resume", "Reanudar"},
        {"Quit", "Salir"},
        {"Options", "Opciones"},
        {"Help", "Ayuda"},
        {"Credits", "Cr�ditos"},
        {"Tutorial", "Tutorial"},
        {"Yes", "S�"},
        {"No", "No"},
        {"OK", "Aceptar"},
        {"Cancel", "Cancelar"},
        {"Confirm", "Confirmar"},
        {"Default", "Predeterminado"},
        {"Fog", "Niebla"},
        {"Bloom", "Brillo"},
        {"Shadows", "Sombras"},
        {"Antialiasing", "Antialiasing"},
        {"VSync", "Sincronizaci�n Vertical"},
        {"Textures", "Texturas"},
        {"Shadows Quality", "Calidad de Sombras"},
        {"Anti-Aliasing", "Anti-Aliasing"},
        {"Anisotropic Filtering", "Filtrado Anisotr�pico"},
        {"Shadow Distance", "Distancia de Sombra"},
        {"Ambient Occlusion", "Oclusi�n Ambiental"},
        {"Motion Blur", "Desenfoque de Movimiento"},
        {"Field of View", "Campo de Visi�n"},
        {"Render Distance", "Distancia de Renderizado"},
        {"Master Volume", "Volumen Maestro"},
        {"Music Volume", "Volumen de M�sica"},
        {"SFX Volume", "Volumen de Efectos"},
        {"Voice Volume", "Volumen de Voces"},
        {"Mute", "Silencio"},
        {"Unmute", "Quitar Silencio"},
        {"Audio Output", "Salida de Audio"},
        {"Stereo", "Est�reo"},
        {"Mono", "Mono"},
        {"Surround", "Sonido Envolvente"},
        {"Controls", "Controles"},
        {"Sensitivity", "Sensibilidad"},
        {"Invert Y-Axis", "Invertir Eje Y"},
        {"Invert X-Axis", "Invertir Eje X"},
        {"Mouse Sensitivity", "Sensibilidad del Rat�n"},
        {"Gamepad", "Gamepad"},
        {"Keyboard", "Teclado"},
        {"Mouse", "Rat�n"},
        {"Loading", "Cargando"},
        {"Saving", "Guardando"},
        {"Paused", "Pausado"},
        {"Game Over", "Fin del Juego"},
        {"Victory", "Victoria"},
        {"Defeat", "Derrota"},
        {"Warning", "Advertencia"},
        {"Error", "Error"},
        {"Success", "�xito"},
        {"Retry", "Reintentar"},
        {"Next", "Siguiente"},
        {"Previous", "Anterior"},
        {"Select", "Seleccionar"},
        {"Deselect", "Deseleccionar"},
        {"Confirm Exit", "Confirmar Salida"},
        {"Are you sure?", "�Est�s seguro?"},
        {"Yes, Exit", "S�, Salir"},
        {"No, Stay", "No, Quedarme"}
    };

    private void Awake()
    {
        // Buscar un AudioMixer existente si no est� asignado
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
                Debug.LogError("No se encontr� ning�n AudioMixer en el proyecto. Por favor, cree uno en el Editor de Unity y as�gnelo.");
            }
        }
    }

    private void Start()
    {
        // Configuraci�n de calidad
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Configuraci�n de resoluci�n
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Configuraci�n de volumen
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 0.75f);

        // Llamada a funciones para aplicar configuraciones actuales
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

        // Realizar traducci�n autom�tica de textos si est� activado
        if (autoTranslateTexts)
        {
            TranslateAllTextsInScene();
        }
    }

    // Funci�n para aplicar configuraci�n gr�fica
    public void ApplyGraphicsSettings()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        RenderSettings.fog = fogToggle.isOn;
        QualitySettings.shadows = shadowsToggle.isOn ? ShadowQuality.All : ShadowQuality.Disable;
        QualitySettings.antiAliasing = antialiasingToggle.isOn ? 4 : 0;
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
    }

    // Funci�n para cambiar la resoluci�n
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Funci�n para configurar el volumen maestro
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }

    // Funci�n para configurar el volumen de la m�sica
    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }

    // Funci�n para configurar el volumen de los efectos de sonido
    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
    }

    // Funci�n para configurar el volumen de las voces
    public void SetVoiceVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("VoiceVolume", volume);
        }
    }

    // Funci�n para cerrar el men� de configuraci�n
    public void CloseSettings()
    {
        //this.gameObject.SetActive(false);
    }

    // Funci�n para restablecer las configuraciones a los valores predeterminados
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

    // Funci�n para guardar las configuraciones actuales
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

        Debug.Log("Configuraciones guardadas.");
    }

    // Funci�n para traducir todos los textos en la escena
    private void TranslateAllTextsInScene()
    {
        // Encontrar todos los TextMeshProUGUI componentes en la escena
        TextMeshProUGUI[] tmpTexts = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI tmpText in tmpTexts)
        {
            TranslateText(tmpText);
        }
    }
   
    // Funci�n que traduce el texto si est� en ingl�s y existe en el diccionario
    private void TranslateText(TextMeshProUGUI tmpText)
    {
        if (translationDictionary.ContainsKey(tmpText.text))
        {
            tmpText.text = translationDictionary[tmpText.text];
        }
    }
}
