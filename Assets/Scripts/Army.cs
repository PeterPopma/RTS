using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour
{
    Vector3 destination;
    List<Soldier> soldiers = new List<Soldier>();

    // Start is called before the first frame update
    void Start()
    {
        Game.Instance.Armies.Add(gameObject);
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Soldier>()!=null)
            {
                soldiers.Add(child.GetComponent<Soldier>());
            }
        }

        SetDestination(new Vector3(950, 0, 750));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToDestination = destination - transform.position;
        if (distanceToDestination.magnitude > 0.1f)
        {
            Vector3 direction = Vector3.Normalize(distanceToDestination);
            transform.position += direction * Time.deltaTime * 12f;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;

        foreach (Soldier soldier in soldiers) {
            soldier.Destination = destination;
        }
    }

    public void ChangeSelection(bool selected)
    {
        foreach (Soldier soldier in soldiers)
        {
            soldier.ChangeSelection(selected);
        }
    }
}
