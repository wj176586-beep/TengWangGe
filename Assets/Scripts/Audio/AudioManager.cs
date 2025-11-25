using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : SingletonAutoMono<AudioManager>
{
    
    public void LoadAudio(string fileName, Action<AudioClip> callback)
    {
        StartCoroutine(LoadAudioReally(fileName, callback));
    }

    // 异步加载本地音频文件并返回 AudioClip
    private IEnumerator LoadAudioReally(string fileName, Action<AudioClip> callback)
    {
        string folder = Path.Combine(Application.persistentDataPath, "Audio");

        string filePath = Path.Combine(folder, fileName); // 例如传入 "bgm.mp3"

        if (!File.Exists(filePath))
        {
            Debug.LogError("音频文件不存在: " + filePath);
            callback?.Invoke(null);
            yield break;
        }

        // 注意本地文件需要加前缀 file://
        string url = "file://" + filePath;

        // 根据后缀确定音频类型
        AudioType type = GetAudioType(filePath);

        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, type);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("加载音频失败: " + request.error);
            callback?.Invoke(null);
            yield break;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
        callback?.Invoke(clip);
    }

    // 根据文件后缀返回音频类型
    private AudioType GetAudioType(string path)
    {
        string ext = Path.GetExtension(path).ToLower();
        switch (ext)
        {
            case ".wav": return AudioType.WAV;
            case ".mp3": return AudioType.MPEG;
            case ".ogg": return AudioType.OGGVORBIS;
            default: return AudioType.UNKNOWN;
        }
    }
}