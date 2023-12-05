using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LumberjackState_
{
    WalkingToTree,
    CuttingTree,
    WalkingToHQ
}

public class Lumberjack : MonoBehaviour
{
    [SerializeField] GameObject woodLog;
    Animator animator;
    Vector2 destination = Vector2.zero;
    LumberjackState_ lumberjackState;
    private int animIDSpeed;
    private CharacterController characterController;
    private GameObject currentTree;
    private float timeCuttingTree;
    private float timeLastChopSound;
    AudioSource soundTreeFall;
    private bool treeHasFallen;
    Vector2 villagerSpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SetLumberjackState(LumberjackState_.WalkingToTree);
        animIDSpeed = Animator.StringToHash("Speed");
        characterController = GetComponent<CharacterController>(); 
        soundTreeFall = GameObject.Find("/Sound/TreeFall").GetComponent<AudioSource>();
    }

    public void SetVillagerSpawnPosition(Vector2 villagerSpawnPosition)
    {
        this.villagerSpawnPosition = villagerSpawnPosition;
        transform.position = new Vector3(villagerSpawnPosition.x, 50, villagerSpawnPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        switch (lumberjackState)
        {
           case LumberjackState_.WalkingToTree:
           case LumberjackState_.WalkingToHQ:
                Move();
                break;
           case LumberjackState_.CuttingTree:
                CutTree();
                break;
        }
    }

    private void SetLumberjackState(LumberjackState_ lumberjackState)
    {
        this.lumberjackState = lumberjackState;
        switch (lumberjackState)
        {
            case LumberjackState_.WalkingToTree:
                woodLog.SetActive(false);
                treeHasFallen = false;
                currentTree = Game.Instance.SelectClosestTree(villagerSpawnPosition);
                destination = new Vector2(currentTree.transform.position.x + 2f, currentTree.transform.position.z - 7f);
                break;
            case LumberjackState_.WalkingToHQ:
                timeCuttingTree = 0;
                Destroy(currentTree);
                destination = villagerSpawnPosition;
                woodLog.SetActive(true);
                break;
            case LumberjackState_.CuttingTree:
                // rotate lumberjack towards tree
                Vector2 direction = new Vector2(currentTree.transform.position.x, currentTree.transform.position.z) - new Vector2(transform.position.x, transform.position.z);
                direction.Normalize();
                transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
                animator.Play("Chop", 1, 0);
                animator.SetLayerWeight(1, 1);
                timeLastChopSound = Time.time - 2.3f;
                break;
        }
    }

    private void CutTree()
    {
        timeCuttingTree += Time.deltaTime;
        if (Time.time - timeLastChopSound > 5.05f)
        {
            timeLastChopSound = Time.time;
            Sound.Instance.PlayChopSound();
        }
        if (timeCuttingTree > 23.5)
        {
            if (!treeHasFallen)
            {
                treeHasFallen = true;
                animator.SetLayerWeight(1, 0);
                soundTreeFall.Play();
            }
            currentTree.transform.Rotate(0, 0, Time.deltaTime*60);
        }
        if (timeCuttingTree > 25.5)
        {
            SetLumberjackState(LumberjackState_.WalkingToHQ);
        }
    }

    public void OnFootstep()
    {
        //Sound.Instance.PlayFootStepSound();
    }

    private void Move()
    {
        Vector2 distanceToDestination = destination - new Vector2(transform.position.x, transform.position.z);
        if (distanceToDestination.magnitude > 0.1f)
        {
            animator.SetFloat(animIDSpeed, 2);
            distanceToDestination.Normalize();
            Vector3 direction = new Vector3(distanceToDestination.x, 0, distanceToDestination.y);
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            characterController.Move(direction * Time.deltaTime * 12f);
        }
        else
        {
            animator.SetFloat(animIDSpeed, 0);
            
            if (lumberjackState != LumberjackState_.WalkingToHQ)
            {
                SetLumberjackState(LumberjackState_.CuttingTree);
            }
            else
            {
                HumanPlayer.Instance.ChangeWoodAmount(100);
                SetLumberjackState(LumberjackState_.WalkingToTree);
            }
        }
    }
}
