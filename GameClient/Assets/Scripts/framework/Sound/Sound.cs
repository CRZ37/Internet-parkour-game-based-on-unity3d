using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    AudioSource bg;
    AudioSource effect;
    public string ResourceDir = "";
    protected void Awake()
    {
        bg = gameObject.AddComponent<AudioSource>();
        bg.playOnAwake = false;
        bg.loop = true;
        bg.volume = 0.5f;
        //音效音量比bgm小
        effect = gameObject.AddComponent<AudioSource>();
        effect.volume = 0.4f;
    }
    /// <summary>
    /// 设置AudioSource的音量大小
    /// </summary>
    public void SetBGVolume(float value)
    {
        bg.volume = value;
    }
    public void SetEffectVolume(float value)
    {
        effect.volume = value;
    }
    /// <summary>
    /// 控制背景音乐的播放
    /// </summary>
    /// <param name="name">歌曲名</param>
    public void PlayBg(string name)
    {
        //获得正在播放的歌曲
        string oldName;
        if (bg.clip == null)
        {
            oldName = "";
        }
        else
        {
            oldName = bg.clip.name;
        }

        //如果需要播放的歌曲不是当前歌曲
        if (oldName != name)
        {
            //加载资源
            string path = ResourceDir + "/" + name;
            AudioClip clip = Resources.Load<AudioClip>(path);
            //播放
            if (clip != null)
            {
                bg.clip = clip;
                bg.Play();
            }
        }
        else
        {
            bg.Play();
        }
    }
    public void StopBG()
    {
        bg.Stop();
    }
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名</param>
    public void PlayEffect(string name){
        //加载资源
        string path = ResourceDir + "/" + name;
        AudioClip clip = Resources.Load<AudioClip>(path);
        //播放
        effect.PlayOneShot(clip);
    }
}
