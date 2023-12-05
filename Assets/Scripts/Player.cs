using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject pfLumberjack;
    [SerializeField] GameObject headQuarters;
    [SerializeField] GameObject villagerSpawnPosition;
    [SerializeField] Barracks barracks;
    [SerializeField] int playerNumber;
    private List<Army> armies = new List<Army>();
    private int wood;
    private int stone;

    public List<Army> Armies { get => armies; set => armies = value; }

    public int Stone { get => stone; set => stone = value; }
    public int Wood { get => wood; set => wood = value; }
    public GameObject HeadQuarters { get => headQuarters; set => headQuarters = value; }
    public Barracks Barracks { get => barracks; set => barracks = value; }

    public void NewGame()
    {
        GameObject newLumberJack = Instantiate(pfLumberjack, villagerSpawnPosition.transform.position, Quaternion.identity);
        newLumberJack.GetComponent<Lumberjack>().SetVillagerSpawnPosition(new Vector2(villagerSpawnPosition.transform.position.x, villagerSpawnPosition.transform.position.z));
        Instantiate(pfLumberjack, villagerSpawnPosition.transform.position, Quaternion.identity);
        newLumberJack.GetComponent<Lumberjack>().SetVillagerSpawnPosition(new Vector2(villagerSpawnPosition.transform.position.x, villagerSpawnPosition.transform.position.z));
        headQuarters.SetActive(true);
        stone = wood = 0;
        HumanPlayer.Instance.ChangeStoneAmount(2500);
    }

    public void SpawnArmy(ArmyType_ armyType)
    {
        barracks.SpawnNewArmy(armyType);
    }

    public bool WantsToAttackEnemy(Vector2 destination)
    {
        float y = Terrain.activeTerrain.SampleHeight(new Vector3(destination.x, destination.y, 400));
        Collider[] colliders = Physics.OverlapSphere(new Vector3(destination.x, y, destination.y), 10);

        foreach (var collider in colliders)
        {
            Soldier soldier = collider.GetComponent<Soldier>();
            if (soldier != null)
            {
                if (soldier.Army.PlayerNumber != playerNumber)
                {
                    return true;
                }
            }
            HeadQuarters headQuarters = collider.GetComponent<HeadQuarters>();
            if (headQuarters != null)
            {
                if (headQuarters.PlayerNumber != playerNumber)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetArmyDestination(Army army, bool attackingEnemy, Vector2 destination)
    {
        army.WantsToAttack = attackingEnemy;
        army.SetDestination(destination);
        if (!attackingEnemy)
        {
            army.UpdateAttackMode(false);
        }
    }
}
