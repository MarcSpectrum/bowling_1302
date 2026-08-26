using UnityEngine;

public static class BowlingSettings
{
    const string P="Bowling1302.";
    public static void Apply(float master, float music, float sfx, bool fullscreen, int quality)
    {
        PlayerPrefs.SetFloat(P+"Master",master); PlayerPrefs.SetFloat(P+"Music",music); PlayerPrefs.SetFloat(P+"SFX",sfx);
        PlayerPrefs.SetInt(P+"Fullscreen",fullscreen?1:0); PlayerPrefs.SetInt(P+"Quality",quality); PlayerPrefs.Save();
        AudioListener.volume=master; Screen.fullScreen=fullscreen; QualitySettings.SetQualityLevel(Mathf.Clamp(quality,0,QualitySettings.names.Length-1),true);
    }
    public static void LoadAndApply() => Apply(PlayerPrefs.GetFloat(P+"Master",.8f),PlayerPrefs.GetFloat(P+"Music",.65f),PlayerPrefs.GetFloat(P+"SFX",.85f),PlayerPrefs.GetInt(P+"Fullscreen",1)==1,PlayerPrefs.GetInt(P+"Quality",QualitySettings.GetQualityLevel()));
}
