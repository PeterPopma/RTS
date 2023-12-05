using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyAI : MonoBehaviour
{
    Army army;
    Player player;
    MovementType_ movementType;
    List<Vector3> patrolDestinations = new List<Vector3>();
    int currentPatrolDestination;

    public enum MovementType_
    {
        Explore,
        AttackEnemyHQ,
        DefendOwnHQ
    }

    void Awake()
    {
        army = GetComponent<Army>();
        player = Game.Instance.Players[1];
        ChooseRandomMovementMode();
    }

    public void ChooseRandomMovementMode()
    {
        Array values = Enum.GetValues(typeof(MovementType_));
        movementType = (MovementType_)values.GetValue(new System.Random().Next(values.Length));
        SetMovementMode(movementType);
    }

    public void SetMovementMode(MovementType_ movementType)
    {
        this.movementType = movementType;
        switch (movementType)
        {
            case MovementType_.AttackEnemyHQ:
                army.Destination = new Vector2(Game.Instance.Players[0].HeadQuarters.transform.position.x, Game.Instance.Players[0].HeadQuarters.transform.position.z);
                player.SetArmyDestination(army, player.WantsToAttackEnemy(army.Destination), army.Destination);
                break;
            case MovementType_.Explore:
                SetRandomDestination();
                break;
            case MovementType_.DefendOwnHQ:
                patrolDestinations.Add(GetRandomDestinationNearOwnHQ());
                patrolDestinations.Add(GetRandomDestinationNearOwnHQ());
                SetNextPatrolDestination();
                break;
        }
    }

    private Vector3 GetRandomDestinationNearOwnHQ()
    {
        float xAdjust = 10 + UnityEngine.Random.value * 100;
        if (UnityEngine.Random.value < 0.5)
        {
            xAdjust = -xAdjust;
        }
        float yAdjust = 10 + UnityEngine.Random.value * 100;
        if (UnityEngine.Random.value < 0.5)
        {
            yAdjust = -yAdjust;
        }
        return new Vector2(player.HeadQuarters.transform.position.x + xAdjust,
                                       player.HeadQuarters.transform.position.z + yAdjust);
    }

    private void SetRandomDestination()
    {
        army.Destination = new Vector2(100 + UnityEngine.Random.value * 1300, 80 + UnityEngine.Random.value * 1270);
        player.SetArmyDestination(army, player.WantsToAttackEnemy(army.Destination), army.Destination);
    }

    private void SetNextPatrolDestination()
    {
        currentPatrolDestination++;
        if (currentPatrolDestination >= patrolDestinations.Count)
        {
            currentPatrolDestination = 0;
        }
        army.Destination = patrolDestinations[currentPatrolDestination];
        player.SetArmyDestination(army, player.WantsToAttackEnemy(army.Destination), army.Destination);
    }

    public void OnArmyArrivedAtDestination()
    {
        if (movementType == MovementType_.Explore)
        {
            SetRandomDestination();
        }
        if (movementType == MovementType_.DefendOwnHQ)
        {
            SetNextPatrolDestination();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
