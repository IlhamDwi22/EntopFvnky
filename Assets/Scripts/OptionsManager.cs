using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    [Header("UI Audio Sliders")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("UI Screen Toggle")]
    public Toggle fullscreenToggle;

    [Header("UI Keybind Texts")]
    public TextMeshProUGUI textLeft;
    public TextMeshProUGUI textDown;
    public TextMeshProUGUI textUp;
    public TextMeshProUGUI textRight;

    [Header("Audio Mixer Setup")]
    [Tooltip("Tarik asset AudioMixer Anda ke sini")]
    public AudioMixer audioMixer;
    public string paramMaster = "VolumeMaster";
    public string paramBgm = "VolumeBGM";
    public string paramSfx = "VolumeSFX";

    private string rebindingAction = "";
    private TextMeshProUGUI rebindingText = null;

    private void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        // 1. Load Audio Volumes
        float masterVol = PlayerPrefs.GetFloat("VolumeMaster", 1f);
        float bgmVol = PlayerPrefs.GetFloat("VolumeBGM", 0.8f);
        float sfxVol = PlayerPrefs.GetFloat("VolumeSFX", 0.8f);

        if (masterSlider != null) masterSlider.value = masterVol;
        if (bgmSlider != null) bgmSlider.value = bgmVol;
        if (sfxSlider != null) sfxSlider.value = sfxVol;

        ApplyAudioVolumes(masterVol, bgmVol, sfxVol);

        // 2. Load Screen Mode
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFullscreen;
        
        if (isFullscreen)
        {
            Resolution currentRes = Screen.currentResolution;
            Screen.SetResolution(currentRes.width, currentRes.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        }

        // 3. Load Keybinds
        UpdateKeybindTexts();
    }

    public void OnMasterVolumeChanged(float val)
    {
        Debug.Log($"[OptionsManager] OnMasterVolumeChanged called! value={val}, db={ConvertToDecibel(val)}");
        PlayerPrefs.SetFloat("VolumeMaster", val);
        if (audioMixer != null)
        {
            bool success = audioMixer.SetFloat(paramMaster, ConvertToDecibel(val));
            Debug.Log($"[OptionsManager] Setting mixer parameter '{paramMaster}' success status: {success}");
        }
        else
        {
            AudioListener.volume = val; // Fallback jika mixer kosong
        }
    }

    public void OnBgmVolumeChanged(float val)
    {
        Debug.Log($"[OptionsManager] OnBgmVolumeChanged called! value={val}, db={ConvertToDecibel(val)}");
        PlayerPrefs.SetFloat("VolumeBGM", val);
        if (audioMixer != null)
        {
            audioMixer.SetFloat(paramBgm, ConvertToDecibel(val));
        }
    }

    public void OnSfxVolumeChanged(float val)
    {
        Debug.Log($"[OptionsManager] OnSfxVolumeChanged called! value={val}, db={ConvertToDecibel(val)}");
        PlayerPrefs.SetFloat("VolumeSFX", val);
        if (audioMixer != null)
        {
            audioMixer.SetFloat(paramSfx, ConvertToDecibel(val));
        }
    }

    public void OnFullscreenToggled(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        if (isFullscreen)
        {
            // Fullscreen mengikuti resolusi monitor saat ini
            Resolution currentRes = Screen.currentResolution;
            Screen.SetResolution(currentRes.width, currentRes.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            // Windowed disetel ke 1920x1080
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        }
    }

    // --- REBIND KEY MAPPING ---
    public void StartRebindKey(string actionName)
    {
        if (!string.IsNullOrEmpty(rebindingAction)) return;

        rebindingAction = actionName;
        
        if (actionName == "KeyLeft") rebindingText = textLeft;
        else if (actionName == "KeyDown") rebindingText = textDown;
        else if (actionName == "KeyUp") rebindingText = textUp;
        else if (actionName == "KeyRight") rebindingText = textRight;

        if (rebindingText != null)
        {
            rebindingText.text = "...";
        }
    }

    private void OnGUI()
    {
        if (!string.IsNullOrEmpty(rebindingAction) && Event.current.isKey)
        {
            KeyCode newKey = Event.current.keyCode;

            if (newKey != KeyCode.None && newKey != KeyCode.Escape)
            {
                if (KeyMappingManager.Instance != null)
                {
                    KeyMappingManager.Instance.SaveKey(rebindingAction, newKey);
                }
                
                UpdateKeybindTexts();

                rebindingAction = "";
                rebindingText = null;
            }
        }
    }

    private void UpdateKeybindTexts()
    {
        if (KeyMappingManager.Instance == null) return;

        if (textLeft != null) textLeft.text = KeyMappingManager.Instance.keyLeft.ToString();
        if (textDown != null) textDown.text = KeyMappingManager.Instance.keyDown.ToString();
        if (textUp != null) textUp.text = KeyMappingManager.Instance.keyUp.ToString();
        if (textRight != null) textRight.text = KeyMappingManager.Instance.keyRight.ToString();
    }

    private void ApplyAudioVolumes(float master, float bgm, float sfx)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(paramMaster, ConvertToDecibel(master));
            audioMixer.SetFloat(paramBgm, ConvertToDecibel(bgm));
            audioMixer.SetFloat(paramSfx, ConvertToDecibel(sfx));
        }
        else
        {
            AudioListener.volume = master;
        }
    }

    private float ConvertToDecibel(float value)
    {
        // Konversi linear 0..1 ke decibel logaritma -80dB..0dB
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
    }
}
