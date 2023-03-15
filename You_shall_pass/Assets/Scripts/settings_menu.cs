using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class settings_menu : MonoBehaviour
{
    public Slider slider;
    public TMPro.TMP_Dropdown dropDown;
    public AudioMixer mixer;
    public bool inGame;
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality Level"));
        mixer.SetFloat("volume", PlayerPrefs.GetFloat("Volume"));
    }

    private void Awake()
    {
        slider.value = PlayerPrefs.GetFloat("Volume");
        dropDown.value = 2 - PlayerPrefs.GetInt("Quality Level");
        Debug.Log(PlayerPrefs.GetInt("Quality Level"));
    }

    private void Update()
    {
        if (inGame)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(2-qualityIndex);
        PlayerPrefs.SetInt("Quality Level", 2 - qualityIndex);
        
    }
    
    public void SetVolume(float volume)
    {
        mixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("Volume", volume);
    }
}
