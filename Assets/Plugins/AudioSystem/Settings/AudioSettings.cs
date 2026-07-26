using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace AudioSystem
{
    public class AudioSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider uiSlider;
    
        /*
     *          Exemplo de como aproveitar os diferentes grupos de audios
     *
     *          As traducoes de Decibeis para Float e vice-versa sao para
     *          fazer os sliders terem uma progressao linear de volume
     */
    
        private void Start()
        {
            if (masterSlider != null && audioMixer.GetFloat("MasterVolume", out float masterVolume))
                masterSlider.value = ConversionUtils.DBToFloat(masterVolume);

            if (musicSlider != null && audioMixer.GetFloat("MusicVolume", out float musicVolume))
                musicSlider.value = ConversionUtils.DBToFloat(musicVolume);

            if (sfxSlider != null && audioMixer.GetFloat("SFXVolume", out float sfxVolume))
                sfxSlider.value = ConversionUtils.DBToFloat(sfxVolume);

            if (uiSlider != null && audioMixer.GetFloat("UIVolume", out float uiVolume))
                uiSlider.value = ConversionUtils.DBToFloat(uiVolume);
        }
        
        public void OnMasterChange(float volume) => audioMixer.SetFloat("MasterVolume", ConversionUtils.FloatToDB(volume));
        
        public void OnMusicChange(float volume) => audioMixer.SetFloat("MusicVolume", ConversionUtils.FloatToDB(volume));

        public void OnSFXChange(float volume) => audioMixer.SetFloat("SFXVolume", ConversionUtils.FloatToDB(volume));

        public void OnUIChange(float volume) => audioMixer.SetFloat("UIVolume", ConversionUtils.FloatToDB(volume));
    }
}