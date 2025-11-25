using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject(typeof(T).Name);
                instance = o.AddComponent<T>();
            }

            return instance;
        }
    }
}