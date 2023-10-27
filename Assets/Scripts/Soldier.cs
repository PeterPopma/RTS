using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    Vector3 destination;
    Animator animator;
    private int animIDSpeed;
    private int playerOwner;
    private bool selected;

    public Vector3 Destination { get => destination; set => destination = value; }
    public int PlayerOwner { get => playerOwner; set => playerOwner = value; }

    void Awake()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("SelectionMarker").gameObject.SetActive(false);
    }

    public void OnFootstep()
    {
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
//            transform.position += direction * Time.deltaTime * 12f;
        }
        else
        {
            animator.SetFloat(animIDSpeed, 0);
        }
    }

    public void ChangeSelection(bool selected)
    {
        this.selected = selected;
        if (selected)
        {
            transform.Find("SelectionMarker").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("SelectionMarker").gameObject.SetActive(false);
        }
    }
}
