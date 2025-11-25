using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using File = UnityEngine.Windows.File;

public class MyTest : MonoBehaviour
{
    public AudioSource audiosource;
    void Start()
    {
        StartCoroutine(LoadAudio("发如雪-周杰伦.mp3", (clip) =>
        {
            audiosource.clip = clip;
            audiosource.Play();
            
        }));
    }


    public IEnumerator LoadAudio(string fileName, UnityAction<AudioClip> callback)
    {
        string path = Path.Combine(Application.persistentDataPath, "Audio", fileName);

        if (!File.Exists(path))
        {
            Debug.Log("文件不存在");
            yield break;
        }

        string s = Path.Combine("file://" + path);

        using (UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(s, AudioType.UNKNOWN))
        {
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("获取音频失败");
                yield break;
            }

            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(unityWebRequest);
            callback(audioClip);
        }
    }
}