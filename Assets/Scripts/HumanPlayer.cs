using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanPlayer : MonoBehaviour
{
    public static HumanPlayer Instance;
    [SerializeField] TextMeshProUGUI textWood;
    [SerializeField] TextMeshProUGUI textStone;
    [SerializeField] Button buttonWallBuild;
    private List<Army> selectedArmies = new List<Army>();
    private bool isBuildingWall;
    private Player player;

    public List<Army> SelectedArmies { get => selectedArmies; set => selectedArmies = value; }
    public bool IsBuildingWall { get => isBuildingWall; set => isBuildingWall = value; }
    public Player Player { get => player; set => player = value; }

    public void Awake()
    {
        Instance = this;
    }

    public void OnButtonSpawnArcherClick()
    {
        player.SpawnArmy(ArmyType_.Archer);
    }

    public void OnButtonSpawnPikemanClick()
    {
        player.SpawnArmy(ArmyType_.Pikeman);
    }

    public void OnButtonSpawnSwordsmanClick()
    {
        player.SpawnArmy(ArmyType_.Swordsman);
    }

    public void OnButtonWallClick()
    {
        SetBuildWall(!isBuildingWall);
    }

    public void ChangeStoneAmount(int amount)
    {
        player.Stone += amount;
        textStone.text = ": " + player.Stone;
    }

    public void ChangeWoodAmount(int value)
    {
        player.Wood += value;
        textWood.text = ": " + player.Wood;
    }

    public void SetBuildWall(bool isBuilding)
    {
        if (player.Stone == 0)
        {
            IsBuildingWall = false;
            GameObject.Find("Scripts/WallBuild").GetComponent<WallBuild>().Reset();
            buttonWallBuild.GetComponent<Image>().sprite = Resources.Load<Sprite>("button_wall_disabled");
            return;
        }
        isBuildingWall = isBuilding;
        if (isBuilding)
        {
            buttonWallBuild.GetComponent<Image>().sprite = Resources.Load<Sprite>("button_wall_disabled");
        }
        else
        {
            buttonWallBuild.GetComponent<Image>().sprite = Resources.Load<Sprite>("button_wall");
            GameObject.Find("Scripts/WallBuild").GetComponent<WallBuild>().Reset();
        }
    }

    public void RemoveSelection()
    {
        foreach (var army in SelectedArmies)
        {
            army.IsSelected = false;
            foreach (Soldier soldier in army.Soldiers)
            {
                soldier.ChangeSelection(false);
            }
        }
        SelectedArmies.Clear();
    }

    public void SetArmyDestination(Vector2 destination)
    {
        bool attackingEnemy = player.WantsToAttackEnemy(destination);
        foreach (var army in SelectedArmies)
        {
            player.SetArmyDestination(army, attackingEnemy, destination);
        }
    }
}
