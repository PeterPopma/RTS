using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : MonoBehaviour
{
    [SerializeField] int playerNumber;

    public int PlayerNumber { get => playerNumber; set => playerNumber = value; }

    // Start is called before the first frame update
    void Start()
    {
        
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
                armyLeaderBeingHealed.Army.Health <100)
            {
                armyLeaderBeingHealed.Army.ChangeHealth(0.02f);
            }
        }
    }
}
