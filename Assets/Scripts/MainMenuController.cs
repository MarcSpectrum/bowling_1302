using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject home, settings, credits;
    void Start(){BowlingSettings.LoadAndApply();ShowHome();}
    public void Play()=>SceneManager.LoadScene("BowlingGame");
    public void Quit()=>Application.Quit();
    public void ShowHome(){home.SetActive(true);settings.SetActive(false);credits.SetActive(false);}
    public void ShowSettings(){home.SetActive(false);settings.SetActive(true);credits.SetActive(false);}
    public void ShowCredits(){home.SetActive(false);settings.SetActive(false);credits.SetActive(true);}
    public void SetMaster(float v)=>BowlingSettings.Apply(v,PlayerPrefs.GetFloat("Bowling1302.Music",.65f),PlayerPrefs.GetFloat("Bowling1302.SFX",.85f),Screen.fullScreen,QualitySettings.GetQualityLevel());
    public void SetMusic(float v)=>BowlingSettings.Apply(AudioListener.volume,v,PlayerPrefs.GetFloat("Bowling1302.SFX",.85f),Screen.fullScreen,QualitySettings.GetQualityLevel());
    public void SetSfx(float v)=>BowlingSettings.Apply(AudioListener.volume,PlayerPrefs.GetFloat("Bowling1302.Music",.65f),v,Screen.fullScreen,QualitySettings.GetQualityLevel());
    public void SetFullscreen(bool v)=>BowlingSettings.Apply(AudioListener.volume,PlayerPrefs.GetFloat("Bowling1302.Music",.65f),PlayerPrefs.GetFloat("Bowling1302.SFX",.85f),v,QualitySettings.GetQualityLevel());
    public void SetQuality(int v)=>BowlingSettings.Apply(AudioListener.volume,PlayerPrefs.GetFloat("Bowling1302.Music",.65f),PlayerPrefs.GetFloat("Bowling1302.SFX",.85f),Screen.fullScreen,v);
    public void SetResolution(int v){var r=Screen.resolutions;if(r.Length==0)return;var x=r[Mathf.Clamp(v,0,r.Length-1)];Screen.SetResolution(x.width,x.height,Screen.fullScreen);PlayerPrefs.SetInt("Bowling1302.Resolution",v);}
}
