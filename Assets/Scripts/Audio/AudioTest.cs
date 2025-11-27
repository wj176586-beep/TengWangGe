using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        // Debug.Log(Application.persistentDataPath);  C:/Users/wj176/AppData/LocalLow/DefaultCompany/TengWangGeTest
        //
        // AudioManager.Instance.LoadAudio("发如雪-周111杰伦.mp3", (obj) =>
        // {
        //     audioSource.clip = obj;
        //     audioSource.Play();
        // });

        // C:\Users\MSI\AppData\LocalLow\DefaultCompany\TengWangGeTest\Audio\发如雪-周杰伦.mp3
        AudioManager.LoadAudio(this, Path.Combine(Application.persistentDataPath, "Audio", "发如雪-周杰伦.mp3"), (obj) =>
        {
            audioSource.clip = obj;
            audioSource.Play();
        });
    }
}