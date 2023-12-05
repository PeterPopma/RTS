using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UnitSelection : MonoBehaviour
{
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject[] PanelUnitInfo;
    [SerializeField] new Camera camera;
    [SerializeField] UILineRenderer lineRenderer;
    [SerializeField] GameObject imageMap;

    Texture2D selectTexture;
    Vector3 boxBegin;
    Vector3 boxEnd;
    float timeLeftArrow;
    Vector3 arrowPosition;
    Vector2[] screenOnMapCorners = new Vector2[5];
    bool drawing;
    const float MAP_OFFSET_X = 0;
    const float MAP_OFFSET_Y = 0;

    private void Start()
    {
        selectTexture = new Texture2D(1, 1);
        selectTexture.SetPixel(0, 0, Color.white);
        selectTexture.Apply();
        arrow.SetActive(false);
        UpdateUnitInfo();
    }
    private void UpdateScreenOnMapCorners()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Terrain");
        Ray ray = camera.ScreenPointToRay(new Vector3(0, 0, 0));
        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            screenOnMapCorners[0] = screenOnMapCorners[4] = WorldToMapCoordinates(new Vector2(hit.point.x, hit.point.z));
        }
        ray = camera.ScreenPointToRay(new Vector3(0, Screen.height, 0));
        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            screenOnMapCorners[1] = WorldToMapCoordinates(new Vector2(hit.point.x, hit.point.z));
        }
        ray = camera.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0));
        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            screenOnMapCorners[2] = WorldToMapCoordinates(new Vector2(hit.point.x, hit.point.z));
        }
        ray = camera.ScreenPointToRay(new Vector3(Screen.width, 0, 0));
        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            screenOnMapCorners[3] = WorldToMapCoordinates(new Vector2(hit.point.x, hit.point.z));
        }

    }

    // Translates the world position to a point on the map
    // World is x,z: 0,0 to 1500, 1500
    private Vector2 WorldToMapCoordinates(Vector2 worldCoordinates)
    {
        return new Vector2(MAP_OFFSET_X + imageMap.GetComponent<RectTransform>().rect.width * worldCoordinates.x / 1500.0f, MAP_OFFSET_Y + imageMap.GetComponent<RectTransform>().rect.height * worldCoordinates.y / 1500.0f);
    }

    // Translates a point on the map to the world position
    // World is x,z: 0,0 to 1500, 1500
    private Vector2 MapToWorldCoordinates(Vector2 screenCoordinates)
    {
        return new Vector2(screenCoordinates.x / imageMap.GetComponent<RectTransform>().rect.width * 1500.0f, 
                           screenCoordinates.y / imageMap.GetComponent<RectTransform>().rect.height * 1500.0f);
    }

    public void SetCameraPosition(Vector2 newPosition)
    {
        Camera.main.transform.position = new Vector3(newPosition.x, Camera.main.transform.position.y, newPosition.y);
    }

    public void OnMapClick()
    {
        Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        // map is at viewport position (0.82, 0.39) to (0.97, 0.06)
        float ratioX = (mousePosition.x - 0.82f) * 6.666f;
        float ratioY = (mousePosition.y - 0.06f) * 3.0303f;
        Vector2 mapPosition = new Vector2(ratioX * 1500, ratioY * 1500);
        SetCameraPosition(mapPosition);
    }

    private Vector3 ScreenToWorldCoordinates(Vector3 screenPosition)
    {
        Ray ray = camera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Terrain");
        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && HumanPlayer.Instance.SelectedArmies.Count>0)
        {
            timeLeftArrow = 1;
            arrowPosition = ScreenToWorldCoordinates(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            arrow.transform.position = arrowPosition;
            arrow.SetActive(true);

            HumanPlayer.Instance.SetArmyDestination(new Vector2(arrowPosition.x, arrowPosition.z));
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
            boxBegin =  Input.mousePosition;
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


        UpdateScreenOnMapCorners();
        lineRenderer.Points = null;
        lineRenderer.Points = screenOnMapCorners;
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
        Vector3 position = ScreenToWorldCoordinates(boxBegin);
        Collider[] colliders = Physics.OverlapSphere(position, 30);
        Army unitSelected = null;
        foreach (var collider in colliders)
        {
            Soldier soldier = collider.GetComponent<Soldier>();
            if (soldier != null && soldier.Army.PlayerNumber == 0)
            {
                unitSelected = soldier.Army;
                unitSelected.ChangeSelection(true);
                break;
            }
        }
        if (unitSelected==null)
        {
            HumanPlayer.Instance.RemoveSelection();
        }
        UpdateUnitInfo();
    }

    private void SetUnitInfo(int currentArmy, GameObject panelUnitInfo)
    {
        switch (HumanPlayer.Instance.SelectedArmies[currentArmy].ArmyType)
        {
            case ArmyType_.Swordsman:
                panelUnitInfo.transform.Find("ImageUnitType").GetComponent<Image>().sprite = Resources.Load<Sprite>("swordsman");
                break;
            case ArmyType_.Pikeman:
                panelUnitInfo.transform.Find("ImageUnitType").GetComponent<Image>().sprite = Resources.Load<Sprite>("pikeman");
                break;
            case ArmyType_.Archer:
                panelUnitInfo.transform.Find("ImageUnitType").GetComponent<Image>().sprite = Resources.Load<Sprite>("archer");
                break;
        }
        panelUnitInfo.transform.Find("TextAttack").GetComponent<TextMeshProUGUI>().text = HumanPlayer.Instance.SelectedArmies[currentArmy].AttackStrength.ToString();
        panelUnitInfo.transform.Find("TextDefence").GetComponent<TextMeshProUGUI>().text = HumanPlayer.Instance.SelectedArmies[currentArmy].DefenceStrength.ToString();
        panelUnitInfo.transform.Find("TextSpeed").GetComponent<TextMeshProUGUI>().text = HumanPlayer.Instance.SelectedArmies[currentArmy].Speed.ToString();
        panelUnitInfo.transform.Find("TextRange").GetComponent<TextMeshProUGUI>().text = HumanPlayer.Instance.SelectedArmies[currentArmy].Range.ToString();
        panelUnitInfo.transform.Find("TextType").GetComponent<TextMeshProUGUI>().text = HumanPlayer.Instance.SelectedArmies[currentArmy].ArmyType.ToString();
        if (HumanPlayer.Instance.SelectedArmies[currentArmy].IsArchers)
        {
            panelUnitInfo.transform.Find("LabelRange").gameObject.SetActive(true);
            panelUnitInfo.transform.Find("TextRange").gameObject.SetActive(true);
        }
        else
        {
            panelUnitInfo.transform.Find("LabelRange").gameObject.SetActive(false);
            panelUnitInfo.transform.Find("TextRange").gameObject.SetActive(false);
        }
        panelUnitInfo.SetActive(true);
    }

    private void UpdateUnitInfo()
    {
        int currentArmy = 0;

        foreach (var panelUnitInfo in PanelUnitInfo)
        {
            if (currentArmy < HumanPlayer.Instance.SelectedArmies.Count)
            {
                SetUnitInfo(currentArmy, panelUnitInfo);
            }
            else
            {
                panelUnitInfo.SetActive(false);
            }
            currentArmy++;
        }
    }

    private void MultiSelectUnits()
    {
        Vector3 beginPosition = ScreenToWorldCoordinates(boxBegin);
        Vector3 endPosition = ScreenToWorldCoordinates(boxEnd);
        float left = Mathf.Min(beginPosition.x, endPosition.x);
        float right = Mathf.Max(beginPosition.x, endPosition.x);
        float top = Mathf.Min(beginPosition.z, endPosition.z);
        float bottom = Mathf.Max(beginPosition.z, endPosition.z);
        foreach (Army currentArmy in HumanPlayer.Instance.Player.Armies)
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
        UpdateUnitInfo();
    }

}
