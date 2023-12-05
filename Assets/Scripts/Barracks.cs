using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barracks : MonoBehaviour
{
    public GameObject pfSwordsman;
    public GameObject pfPikeman;
    public GameObject pfArcher;
    [SerializeField] int playerNumber;
    [SerializeField] GameObject spawnPosition;
    [SerializeField] Button buttonSpawnArcher;
    [SerializeField] Button buttonSpawnPikeman;
    [SerializeField] Button buttonSpawnSwordsman;
    [SerializeField] bool autoSpawnAI;
    GameObject newArmy;
    float timeLeftSpawning;
    GameObject progressBar;
    GameObject progressBarArmy;

    public int PlayerNumber { get => playerNumber; set => playerNumber = value; }

    // Start is called before the first frame update
    void Start()
    {
        progressBar = Instantiate(Game.Instance.PfProgressBar);
        progressBar.transform.SetParent(Game.Instance.Canvas.transform, false);
        progressBar.name = "Progress_" + gameObject.name;
        progressBar.transform.localScale = new Vector3(4, 2, 4); 
        progressBar.SetActive(false);
        if (playerNumber == 0)
        {
            buttonSpawnArcher.transform.Find("Progress").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 36);
            buttonSpawnSwordsman.transform.Find("Progress").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 36);
            buttonSpawnPikeman.transform.Find("Progress").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 36);
        }
        else
        {
            SpawnArmyAI();
        }
        // spawn one unit at startup
        SpawnNewArmy(ArmyType_.Swordsman);
        timeLeftSpawning = 0.1f;        
    }

    public void ChangeSpawnButtonsState(bool enabled)
    {
        if (playerNumber != 0)
        {
            // only display buttons for human player
            return;
        }
        string extra_text = "";
        if (!enabled)
        {
            extra_text = "_disabled";
        }
        buttonSpawnArcher.GetComponent<Image>().sprite = Resources.Load<Sprite>("button_archer" + extra_text);
        buttonSpawnPikeman.GetComponent<Image>().sprite = Resources.Load<Sprite>("button_pikeman" + extra_text);
        buttonSpawnSwordsman.GetComponent<Image>().sprite = Resources.Load<Sprite>("button_swordsman" + extra_text);
    }

    public void SpawnNewArmy(ArmyType_ armyType)
    {
        if (timeLeftSpawning > 0)
        {
            // already spawning
            return;
        }
        switch (armyType)
        {
            case ArmyType_.Archer:
                newArmy = pfArcher;
                if (playerNumber == 0)
                {
                    progressBarArmy = buttonSpawnArcher.transform.Find("Progress").gameObject;
                }
                break;
            case ArmyType_.Swordsman:
                newArmy = pfSwordsman;
                if (playerNumber == 0)
                {
                    progressBarArmy = buttonSpawnSwordsman.transform.Find("Progress").gameObject;
                }
                break;
            case ArmyType_.Pikeman:
                newArmy = pfPikeman;
                if (playerNumber == 0)
                {
                    progressBarArmy = buttonSpawnPikeman.transform.Find("Progress").gameObject;
                }
                break;
        }
        timeLeftSpawning = 20;
        ChangeSpawnButtonsState(false);        
    }

    private void Update()
    {
        if (timeLeftSpawning > 0)
        {
            timeLeftSpawning -= Time.deltaTime;
            progressBar.GetComponent<Slider>().value = timeLeftSpawning*5;
            DrawProgressBar();
            if (playerNumber == 0)
            {
                progressBarArmy.GetComponent<RectTransform>().sizeDelta = new Vector2((100 - (timeLeftSpawning * 5f))*.46f, 36);
            }

            if (timeLeftSpawning < 0)
            {
                if (playerNumber == 0)
                {
                    progressBarArmy.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 36);
                }
                progressBar.SetActive(false);
                Instantiate(newArmy, spawnPosition.transform.position, Quaternion.identity);
                ChangeSpawnButtonsState(true);
                if (playerNumber != 0 && autoSpawnAI)
                {
                    SpawnArmyAI();
                }
            }
        }
    }

    private void SpawnArmyAI()
    {
        Array values = Enum.GetValues(typeof(ArmyType_));
        SpawnNewArmy((ArmyType_)values.GetValue(new System.Random().Next(values.Length)));
    }

    private void DrawProgressBar()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0, 0));
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Game.Instance.Canvas.GetComponent<RectTransform>(), screenPosition, null, out canvasPosition);

        if (screenPosition.z > 0)
        {
            progressBar.SetActive(true);
            progressBar.GetComponent<RectTransform>().anchoredPosition = canvasPosition;
        }
        else
        {
            progressBar.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (Game.Instance.GameState != GameState_.Playing)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, 40);

        foreach (var collider in colliders)
        {
            Soldier armyLeaderBeingHealed = collider.GetComponent<Soldier>();
            if (armyLeaderBeingHealed != null && 
                armyLeaderBeingHealed.IsLeader && 
                armyLeaderBeingHealed.Army.PlayerNumber == playerNumber &&
                armyLeaderBeingHealed.Army.Health < 100)
            {
                armyLeaderBeingHealed.Army.ChangeHealth(0.02f);
            }
        }
    }
}
