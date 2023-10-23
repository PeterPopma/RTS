using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    bool moveScreenLeft;
    bool moveScreenRight;
    bool moveScreenUp;
    bool moveScreenDown;
    bool zoomIn;
    bool zoomOut;
    new Camera camera;
    private float timeMovingStarted;
    float cameraHeight = 100;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveSpeed = 24f * Mathf.Pow(3, (Time.time - timeMovingStarted));
        if (moveSpeed>200f)
        {
            moveSpeed = 200f;
        }
        if (moveScreenLeft)
        {
            camera.transform.position -= new Vector3(Time.deltaTime * moveSpeed, 0, 0);
        }
        if (moveScreenRight)
        {
            camera.transform.position += new Vector3(Time.deltaTime * moveSpeed, 0, 0);
        }
        if (moveScreenUp)
        {
            camera.transform.position += new Vector3(0, 0, Time.deltaTime * moveSpeed);
        }
        if (moveScreenDown)
        {
            camera.transform.position -= new Vector3(0, 0, Time.deltaTime * moveSpeed);
        }
        if (zoomIn)
        {
            if (cameraHeight > 70)
            {
                cameraHeight -= Time.deltaTime * 100f;
                camera.transform.position = new Vector3(camera.transform.position.x, cameraHeight, camera.transform.position.z);
            }
        }
        if (zoomOut)
        {
            if (cameraHeight < 300)
            {
                cameraHeight += Time.deltaTime * 100f;
                camera.transform.position = new Vector3(camera.transform.position.x, cameraHeight, camera.transform.position.z);
            }
        }
    }

    public void OnScreenLeft(InputValue value)
    {
        timeMovingStarted = Time.time;
        moveScreenLeft = value.isPressed;
    }

    public void OnScreenRight(InputValue value)
    {
        timeMovingStarted = Time.time;
        moveScreenRight = value.isPressed;
    }

    public void OnScreenUp(InputValue value)
    {
        timeMovingStarted = Time.time;
        moveScreenUp = value.isPressed;
    }

    public void OnScreenDown(InputValue value)
    {
        timeMovingStarted = Time.time;
        moveScreenDown = value.isPressed;
    }

    public void OnZoomIn(InputValue value)
    {
        if (value.isPressed)
        {
            zoomIn = true;
        }
        else
        {
            zoomIn = false;
        }
    }

    public void OnZoomOut(InputValue value)
    {
        if (value.isPressed)
        {
            zoomOut = true;
        }
        else
        {
            zoomOut = false;
        }
    }

}
