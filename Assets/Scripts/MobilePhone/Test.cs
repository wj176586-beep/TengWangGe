using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update

    public Joystick joystick;
    private int i = 10;
    private int a => 10;

    public int c { private set; get; }

    void Start()
    {
        Debug.Log(Screen.width);
        Debug.Log(Screen.height);
        // Input.GetTouch(0).phase

        // Input.GetTouch(0).fingerId
        float pressure = Input.GetTouch(0).pressure;
        float radius = Input.GetTouch(0).radius;
        float radiusVariance = Input.GetTouch(0).radiusVariance;
        //
        // if (Input.GetTouch())
        // {
        //     
        // }
    }

    // Update is called once per frame
    void Update()
    {
    }
}