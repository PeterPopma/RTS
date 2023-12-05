using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScene : MonoBehaviour
{
    public static DemoScene Instance;
    bool sceneCreated;

    void Awake()
    {
        Instance = this;
    }

    public void Create()
    {
        Barracks barracksBlue = Game.Instance.Players[0].Barracks;
        Barracks barracksRed = Game.Instance.Players[1].Barracks;
        Vector2 headQuartersBluePosition = new Vector2(Game.Instance.Players[0].HeadQuarters.transform.position.x, Game.Instance.Players[0].HeadQuarters.transform.position.z);
        Vector2 headQuartersRedPosition = new Vector2(Game.Instance.Players[1].HeadQuarters.transform.position.x, Game.Instance.Players[1].HeadQuarters.transform.position.z);

        if (!sceneCreated)
        {
            sceneCreated = true;
            SpawnUnitsInCircleAroundPoint(headQuartersBluePosition, 10, 140, barracksRed.pfArcher, barracksRed);
            SpawnUnitsInCircleAroundPoint(headQuartersBluePosition, 9, 140, barracksRed.pfSwordsman, barracksRed);
            SpawnUnitsInCircleAroundPoint(headQuartersBluePosition, 5, 140, barracksRed.pfPikeman, barracksRed);
            SpawnUnitsInCircleAroundPoint(headQuartersBluePosition, 16, 90, null, barracksBlue);
        }
    }

    private void SpawnUnitsInCircleAroundPoint(Vector2 centerPosition, int numUnits, float spawn_radius, GameObject pfArmy, Barracks barracks)
    {
        if (pfArmy == null)
        {
            pfArmy = SelectRandomUnit(barracks);
        }
        float angle = 0;
        while (angle < 360)
        {
            angle += 360 / numUnits;
            Vector3 position = new Vector3(centerPosition.x + Mathf.Sin(Mathf.PI * angle / 180) * spawn_radius,
                400,
                centerPosition.y + Mathf.Cos(Mathf.PI * angle / 180) * spawn_radius);
            float y = Terrain.activeTerrain.SampleHeight(position);

            GameObject newArmy = Instantiate(pfArmy, new Vector3(position.x, y + 2f, position.z), Quaternion.identity);
            if (newArmy.GetComponent<ArmyAI>() != null)
            {
                newArmy.SetActive(true);
                newArmy.GetComponent<ArmyAI>().SetMovementMode(ArmyAI.MovementType_.AttackEnemyHQ);
            }
        }
    }

    private GameObject SelectRandomUnit(Barracks barracks)
    {
        Array values = Enum.GetValues(typeof(ArmyType_));
        ArmyType_ armyType = (ArmyType_)values.GetValue(new System.Random().Next(values.Length));
        if (armyType == ArmyType_.Swordsman)
        {
            return barracks.pfSwordsman;
        }
        else if (armyType == ArmyType_.Pikeman)
        {
            return barracks.pfPikeman;
        }
        else
        {
            return barracks.pfArcher;
        }
    }
}
