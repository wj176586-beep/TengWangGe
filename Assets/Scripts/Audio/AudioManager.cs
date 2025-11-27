using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class AudioManager
{
    /// <summary>
    /// 异步安全加载本地音频
    /// </summary>
    /// <param name="runner">用于启动协程的 MonoBehaviour</param>
    /// <param name="fileName">文件名，例如 "bgm.mp3"</param>
    /// <param name="callback">回调返回 AudioClip，失败或文件不存在返回 null</param>
    public static void LoadAudio(MonoBehaviour runner, string fileName, Action<AudioClip> callback)
    {
        runner.StartCoroutine(LoadAudioCoroutine(fileName, callback));
    }

    private static IEnumerator LoadAudioCoroutine(string path, Action<AudioClip> callback)
    {
        
        // 文件不存在，安全返回 null
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[AudioManager] 音频文件不存在: {path}");
            callback?.Invoke(null);
            yield break;
        }

        string url = "file://" + path;
        AudioType type = GetAudioType(path);

        if (type == AudioType.UNKNOWN)
        {
            Debug.LogWarning($"[AudioManager] 不支持的音频格式: {path}");
            callback?.Invoke(null);
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, type))
        {
            yield return request.SendWebRequest();

            // 请求失败，安全返回 null
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[AudioManager] 加载音频失败: {request.error}");
                callback?.Invoke(null);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

            // 再次安全检查
            if (clip == null)
            {
                Debug.LogWarning($"[AudioManager] 音频加载失败: {path}");
            }

            callback?.Invoke(clip);
        }
    }

    private static AudioType GetAudioType(string path)
    {
        string ext = Path.GetExtension(path).ToLower();
        return ext switch
        {
            ".wav" => AudioType.WAV,
            ".mp3" => AudioType.MPEG,
            ".ogg" => AudioType.OGGVORBIS,
            _ => AudioType.UNKNOWN
        };
    }
}
