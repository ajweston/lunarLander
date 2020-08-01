using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System;

public class Lander : MonoBehaviour
{

    public GameObject landerObject;
    public GameObject EngineParticle;

    public Transform camera;
    public float cameraRadius;
    public float phi, theta;

    private ParticleSystem ps;

    float fuel;

    private void setVector(Vector3 vec, float x, float y, float z)
    {
        vec.x = x;
        vec.y = y;
        vec.z = z;
    }

    private void modVector(Vector3 vec, float x, float y, float z)
    {
        vec.x += x;
        vec.y += y;
        vec.z += z;
    }

    private Vector3 acceleration;
    private Vector3 velocity;

    bool throttleUp;
    bool throttleDown;
    float throttle;

    float throttleMin = 0.0f;
    float throttleMax = 100.0f;

    bool cameraUp;
    bool cameraDown;
    bool cameraLeft;
    bool cameraRight;

    void processInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            throttleDown = true;
        }else if (Input.GetKeyDown(KeyCode.Space))
        {
            throttleUp = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            throttleDown = false;
        }else if (Input.GetKeyUp(KeyCode.Space))
        {
            throttleUp = false;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cameraUp = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cameraDown = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cameraLeft = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cameraRight = true;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            cameraUp = false;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            cameraDown = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            cameraLeft = false;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            cameraRight = false;
        }

    }

    void updateCamera()
    {
        if (cameraUp) theta -= 0.01f;
        else if (cameraDown) theta += 0.01f;
        if (cameraLeft) phi += 0.01f;
        else if (cameraRight) phi -= 0.01f;
    }



    // Start is called before the first frame update
    void Start()
    {
        acceleration.x = 0;
        acceleration.y = -1.6f;
        acceleration.z = 0;

        throttleUp = false;
        throttleDown = false;
        throttle = 0;

        fuel = 1000f;

        ps = EngineParticle.GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        processInput();
        updateCamera();

        Transform landerTransform = landerObject.GetComponent<Transform>();

        Vector3 camPos;
        camPos.x = (float)(cameraRadius * Math.Sin(theta)) + landerObject.transform.position.x;
        camPos.y = (float)(cameraRadius * Math.Cos(theta)) + landerObject.transform.position.y;
        camPos.z = (float)landerObject.transform.position.z ;

        
        velocity.y += (float)-Math.Pow(acceleration.y,2)*Time.deltaTime;

        camera.position = camPos;
        camera.LookAt(landerTransform);
    }

    void DrawQuad(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }


    void OnGUI()
    {
        
        if (throttleUp)
        {
            throttle += 0.25f;
            if (throttle > throttleMax) throttle = throttleMax;
        }else if (throttleDown)
        {
            throttle -= 0.25f;
            if (throttle < throttleMin) throttle = throttleMin;
        }
        var main = ps.main;
        main.simulationSpeed = throttle+1;
        if (throttle == 0 || fuel <= 0) main.loop = false;
        else
        {
            if (fuel >= 0)
            {
                main.loop = true;
                ps.Play();
            }
        }


        fuel -= throttle*0.001f;
        if (fuel <= 0) fuel = 0;

        Rigidbody rb = landerObject.GetComponent<Rigidbody>();
        if (fuel > 0)
        {
            rb.AddForce(0, throttle / 20, 0);
        }
        else
        {

        }

        string label = string.Format("Horizontal Velocity: {0}", rb.velocity.x);
        GUI.Label(new Rect(20, 40, 200, 20), label);
        label = string.Format("Vertical Velocity: {0}", rb.velocity.y);
        GUI.Label(new Rect(20, 60, 200, 20), label);

        label = string.Format("Throttle: {0}", throttle);
        GUI.Label(new Rect(20, 80, 200, 20), label);

        label = string.Format("Altitude: {0}", landerObject.transform.position.y);
        GUI.Label(new Rect(20, 100, 200, 20), label);

        label = string.Format("Fuel: {0}", fuel);
        GUI.Label(new Rect(20, 120, 200, 20), label);

        DrawQuad(new Rect(50, 200, 50, 200), new Color(1.0f, 0.0f, 0.0f));
        if (throttle != 0)
        DrawQuad(new Rect(50, 200+(200-throttle*2), 50, (throttle*2)), new Color(0.0f, 1.0f, 0.0f));

        DrawQuad(new Rect(150, 200, 50, 200), new Color(1.0f, 0.0f, 0.0f));
        if(fuel > 0)
        DrawQuad(new Rect(150, 200 + (200 - fuel / 5), 50, (fuel / 5)), new Color(0.0f, 1.0f, 0.0f));
    }

}
