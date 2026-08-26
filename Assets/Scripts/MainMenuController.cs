using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public sealed class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject home, settings, credits;
    void Start(){BowlingSettings.LoadAndApply();ShowImmediate(home);}
    public void Play()=>SceneManager.LoadScene("BowlingGame");
    public void Quit()=>Application.Quit();
    public void ShowHome()=>Show(home);
    public void ShowSettings()=>Show(settings);
    public void ShowCredits()=>Show(credits);
    void Show(GameObject target){StopAllCoroutines();StartCoroutine(FadeTo(target));}
    void ShowImmediate(GameObject target){foreach(var panel in new[]{home,settings,credits}){panel.SetActive(panel==target);var group=panel.GetComponent<CanvasGroup>();if(group)group.alpha=panel==target?1:0;}}
    IEnumerator FadeTo(GameObject target)
    {
        foreach(var panel in new[]{home,settings,credits})if(panel!=target)panel.SetActive(false);
        target.SetActive(true);var group=target.GetComponent<CanvasGroup>();if(!group)yield break;
        group.alpha=0;group.interactable=false;group.blocksRaycasts=false;
        for(float t=0;t<1;t+=Time.unscaledDeltaTime*5f){group.alpha=Mathf.SmoothStep(0,1,t);yield return null;}
        group.alpha=1;group.interactable=true;group.blocksRaycasts=true;
    }
    public void SetMaster(float v)=>BowlingSettings.Apply(v,PlayerPrefs.GetFloat("Bowling1302.Music",.65f),PlayerPrefs.GetFloat("Bowling1302.SFX",.85f),Screen.fullScreen,QualitySettings.GetQualityLevel());
    public void SetMusic(float v)=>BowlingSettings.Apply(AudioListener.volume,v,PlayerPrefs.GetFloat("Bowling1302.SFX",.85f),Screen.fullScreen,QualitySettings.GetQualityLevel());
    public void SetSfx(float v)=>BowlingSettings.Apply(AudioListener.volume,PlayerPrefs.GetFloat("Bowling1302.Music",.65f),v,Screen.fullScreen,QualitySettings.GetQualityLevel());
    public void SetFullscreen(bool v)=>BowlingSettings.Apply(AudioListener.volume,PlayerPrefs.GetFloat("Bowling1302.Music",.65f),PlayerPrefs.GetFloat("Bowling1302.SFX",.85f),v,QualitySettings.GetQualityLevel());
    public void SetQuality(int v)=>BowlingSettings.Apply(AudioListener.volume,PlayerPrefs.GetFloat("Bowling1302.Music",.65f),PlayerPrefs.GetFloat("Bowling1302.SFX",.85f),Screen.fullScreen,v);
    public void SetResolution(int v){var r=Screen.resolutions;if(r.Length==0)return;var x=r[Mathf.Clamp(v,0,r.Length-1)];Screen.SetResolution(x.width,x.height,Screen.fullScreen);PlayerPrefs.SetInt("Bowling1302.Resolution",v);}
}
