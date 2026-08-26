using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class ArcadeAudio : MonoBehaviour
{
    AudioSource source; AudioClip launch, pins, cheer;
    void Awake(){source=GetComponent<AudioSource>();source.playOnAwake=false;launch=Tone("Launch",160,260,.18f);pins=Tone("Pins",700,260,.22f);cheer=Tone("Cheer",440,880,.4f);}
    public void PlayLaunch(){source.volume=PlayerPrefs.GetFloat("Bowling1302.SFX",.85f);source.PlayOneShot(launch);}
    public void PlayPins(int count){source.volume=PlayerPrefs.GetFloat("Bowling1302.SFX",.85f);source.PlayOneShot(count==10?cheer:pins);}
    static AudioClip Tone(string name,float from,float to,float seconds){const int rate=22050;int count=Mathf.CeilToInt(rate*seconds);var data=new float[count];for(int i=0;i<count;i++){float t=i/(float)rate,f=Mathf.Lerp(from,to,i/(float)count),fade=1-i/(float)count;data[i]=Mathf.Sin(2*Mathf.PI*f*t)*fade*.22f;}var clip=AudioClip.Create(name,count,1,rate,false);clip.SetData(data,0);return clip;}
}
