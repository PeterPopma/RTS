using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Swordsman : MonoBehaviour
{
    Vector3 destination;
    Animator animator;
    private int animIDSpeed;

    public Vector3 Destination { get => destination; set => destination = value; }

    void Awake()
    {
        animIDSpeed = Animator.StringToHash("Speed");
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        destination = new Vector3(52, 0, 939);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToDestination = destination - transform.position;
        if (distanceToDestination.magnitude > 0.1f)
        {
            animator.SetFloat(animIDSpeed, 2);
            Vector3 direction = Vector3.Normalize(distanceToDestination);
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.position += direction * Time.deltaTime * 12f;
        }
        else
        {
            animator.SetFloat(animIDSpeed, 0);
        }
    }
}
