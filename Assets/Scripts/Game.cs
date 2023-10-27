using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;
    private List<GameObject> armies = new List<GameObject>();
    float cameraHeight = 100;

    public List<GameObject> Armies { get => armies; set => armies = value; }
    public float CameraHeight { get => cameraHeight; set => cameraHeight = value; }

    public void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        
    }

    void Update()
    {
    }
}
