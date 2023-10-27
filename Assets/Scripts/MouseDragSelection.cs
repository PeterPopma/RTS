using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSelect : MonoBehaviour
{
    Texture2D selectTexture;

    Vector3 boxBegin;
    Vector3 boxEnd;

    bool drawing;
    [SerializeField] GameObject marker;
    GameObject marker1, marker2, marker3, marker4;

    private void Start()
    {
        selectTexture = new Texture2D(1, 1);
        selectTexture.SetPixel(0, 0, Color.white);
        selectTexture.Apply();
        marker1 = Instantiate(marker);
        marker2 = Instantiate(marker);
        marker3 = Instantiate(marker);
        marker4 = Instantiate(marker);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            boxBegin = Input.mousePosition;
            drawing = true;
        }

        if (drawing)
        {
            boxEnd = Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            drawing = false;
            UpdateSelection();
        }
    }

    void OnGUI()
    {
        if (drawing)
        {
            Rect area = new Rect(boxBegin.x, Screen.height - boxBegin.y, boxEnd.x - boxBegin.x, boxBegin.y - boxEnd.y);

            Rect lineArea = area;
            lineArea.height = 1; // Top line
            GUI.DrawTexture(lineArea, selectTexture);
            lineArea.y = area.yMax - 1; // Bottom
            GUI.DrawTexture(lineArea, selectTexture);
            lineArea = area;
            lineArea.width = 1; // Left
            GUI.DrawTexture(lineArea, selectTexture);
            lineArea.x = area.xMax - 1; // Right
            GUI.DrawTexture(lineArea, selectTexture);
        }
    }

    private void UpdateSelection()
    {
        Camera camera = GetComponent<Camera>();
        Vector3 beginPosition = camera.ViewportToWorldPoint(new Vector3(boxBegin.x / Screen.width, boxBegin.y / Screen.height, Game.Instance.CameraHeight));
        Vector3 endPosition = camera.ViewportToWorldPoint(new Vector3(boxEnd.x / Screen.width, boxEnd.y / Screen.height, Game.Instance.CameraHeight));
        marker1.transform.position = beginPosition;
        marker2.transform.position = endPosition;
        float left = Mathf.Min(beginPosition.x, endPosition.x);
        float right = Mathf.Max(beginPosition.x, endPosition.x);
        float top = Mathf.Min(beginPosition.z, endPosition.z);
        float bottom = Mathf.Max(beginPosition.z, endPosition.z);
        marker1.transform.position = new Vector3(left, 25, top);
        marker2.transform.position = new Vector3(left, 25, bottom);
        marker3.transform.position = new Vector3(right, 25, top);
        marker4.transform.position = new Vector3(right, 25, bottom);
        foreach (GameObject currentArmy in Game.Instance.Armies)
        {
            if (currentArmy.transform.position.x > left &&
                currentArmy.transform.position.x < right &&
                currentArmy.transform.position.z > top &&
                currentArmy.transform.position.z < bottom)
            {
                currentArmy.GetComponent<Army>().ChangeSelection(true);
            }
            else
            {
                currentArmy.GetComponent<Army>().ChangeSelection(false);
            }
        }
    }

}
