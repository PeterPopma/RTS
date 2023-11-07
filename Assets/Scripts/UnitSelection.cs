using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    Texture2D selectTexture;

    Vector3 boxBegin;
    Vector3 boxEnd;
    float timeLeftArrow;
    Vector3 arrowPosition;
    [SerializeField] GameObject arrow;
    new Camera camera;

    bool drawing;

    private void Start()
    {
        selectTexture = new Texture2D(1, 1);
        selectTexture.SetPixel(0, 0, Color.white);
        selectTexture.Apply();
        arrow.SetActive(false); 
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Game.Instance.SelectedArmies.Count>0)
        {
            timeLeftArrow = 1;
            //            arrowPosition = camera.ViewportToWorldPoint(new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, Game.Instance.CameraHeight));
            //            arrowPosition = new Vector3(arrowPosition.x, Terrain.activeTerrain.SampleHeight(arrowPosition) + 10, arrowPosition.z);

            Ray ray = camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Terrain"); 
            if (Physics.Raycast(ray, out hit, 1000, mask))
            {
                arrowPosition = hit.point;
                arrow.transform.position = arrowPosition;
                arrow.SetActive(true);
                Game.Instance.SetArmyDestination(new Vector2(arrowPosition.x, arrowPosition.z));
            }
        }

        if (timeLeftArrow>0)
        {
            timeLeftArrow -= Time.deltaTime;
            // move arrow up and down
            arrow.transform.position = new Vector3(arrowPosition.x, arrowPosition.y + 10 + (float)(timeLeftArrow%0.5 * 8f), arrowPosition.z);
            if (timeLeftArrow < 0)
            {
                arrow.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            boxBegin = Input.mousePosition;
            drawing = true;
        }

        if (drawing)
        {
            boxEnd = Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            drawing = false;

            if (boxBegin.Equals(boxEnd))
            {
                SingleSelectUnit();
            }
            else
            {
                MultiSelectUnits();
            }
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

    private void SingleSelectUnit()
    {
        Vector3 position = camera.ViewportToWorldPoint(new Vector3(boxBegin.x / Screen.width, boxBegin.y / Screen.height, Game.Instance.CameraHeight));
        Collider[] colliders = Physics.OverlapSphere(position, 30);
        bool unitSelected = false;
        foreach (var collider in colliders)
        {
            Soldier soldier = collider.GetComponent<Soldier>();
            if (soldier != null && soldier.Army.PlayerNumber == 0)
            {
                unitSelected = true;
                soldier.Army.ChangeSelection(true);
                break;
            }
        }
        if (!unitSelected)
        {
            Game.Instance.RemoveSelection();
        }
    }

    private void MultiSelectUnits()
    {
        Vector3 beginPosition = camera.ViewportToWorldPoint(new Vector3(boxBegin.x / Screen.width, boxBegin.y / Screen.height, Game.Instance.CameraHeight));
        Vector3 endPosition = camera.ViewportToWorldPoint(new Vector3(boxEnd.x / Screen.width, boxEnd.y / Screen.height, Game.Instance.CameraHeight));
        float left = Mathf.Min(beginPosition.x, endPosition.x);
        float right = Mathf.Max(beginPosition.x, endPosition.x);
        float top = Mathf.Min(beginPosition.z, endPosition.z);
        float bottom = Mathf.Max(beginPosition.z, endPosition.z);
        foreach (Army currentArmy in Game.Instance.Armies)
        {
            if (currentArmy.PlayerNumber==0 &&
                currentArmy.ArmyLeader.transform.position.x > left &&
                currentArmy.ArmyLeader.transform.transform.position.x < right &&
                currentArmy.ArmyLeader.transform.transform.position.z > top &&
                currentArmy.ArmyLeader.transform.transform.position.z < bottom)
            {
                currentArmy.ChangeSelection(true);
            }
            else
            {
                currentArmy.ChangeSelection(false);
            }
        }
    }

}
