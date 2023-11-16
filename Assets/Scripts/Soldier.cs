using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    [SerializeField] GameObject arrowStartPosition;
    [SerializeField] private bool isLeader;
    Vector2 localPosition;
    Vector2 destination;
    Animator animator;
    private int animIDSpeed;
    private int playerOwner;
    private float moveSpeed = 12.0f;
    private CharacterController characterController;
    private Army army;
    private bool isAlive = true;
    private bool isAttacking;
    private float timeLeftDying;
    private float delaySound;
    private float attackDelay;
    private float timeLastShotArrow;
    private bool arrowShot;

    public Vector2 Destination { get => destination; set => destination = value; }
    public int PlayerOwner { get => playerOwner; set => playerOwner = value; }
    public Army Army { get => army; set => army = value; }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public bool IsLeader { get => isLeader; set => isLeader = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public Vector2 LocalPosition { get => localPosition; set => localPosition = value; }

    void Awake()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animator = GetComponent<Animator>();
        localPosition = new Vector2(transform.localPosition.x, transform.localPosition.z);
        characterController = GetComponent<CharacterController>();
        delaySound = Random.Range(0, 2);
    }

    public void Die()
    {
        IsAlive = false;
        timeLeftDying = 3;
        int layer = Random.Range(5, 8);
        animator.SetLayerWeight(layer, 1);
        animator.Play("Die", layer, 0);
        Sound.Instance.PlayDieSound();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("SelectionMarker").gameObject.SetActive(false);
    }

    public void OnFootstep()
    {
    }

    public void SetParent(Army army)
    {
        this.army = army;
    }

    void Update()
    {
        if (Game.Instance.GameState != GameState_.Playing)
        {
            return;
        }
        if (timeLeftDying > 0)
        {
            timeLeftDying -= Time.deltaTime;
            if (timeLeftDying < 0)
            {
                animator.SetLayerWeight(5, 0);
                animator.SetLayerWeight(6, 0);
                animator.SetLayerWeight(7, 0);
                if (army.ActiveSoldiers == 0)
                {
                    Game.Instance.Armies.Remove(army);
                    Game.Instance.SelectedArmies.Remove(army);
                    Destroy(army.Healthbar); 
                    Destroy(army.gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        if (isAttacking)
        {
            delaySound -= Time.deltaTime;
            if (delaySound < 0)
            {
                delaySound = 2;
                if (!army.IsArchers)
                {
                    Sound.Instance.PlayAttackSound();
                }
            }
            if (attackDelay > 0)
            {
                attackDelay -= Time.deltaTime;
                if (attackDelay < 0)
                {
                    int layer;
                    if (!army.IsArchers)
                    {
                        layer = Random.Range(1, 4);
                        animator.SetLayerWeight(layer, 1);
                    }
                    else
                    {
                        layer = 4;
                        animator.SetLayerWeight(layer, 1);
                    }
                    animator.Play("Attack", layer, 0);
                    timeLastShotArrow = Time.time;
                }
            }
            transform.rotation = Quaternion.LookRotation(army.AttackDirection);

            if (army.IsArchers)
            {
                if (Time.time - timeLastShotArrow > 2.4 && !arrowShot)
                {
                    arrowShot = true;
                    GameObject newArrow = Instantiate(army.PfArrow, arrowStartPosition.transform.position, Quaternion.identity);
                    newArrow.transform.rotation = Quaternion.LookRotation(army.AttackDirection);
                    newArrow.transform.Rotate(-15,0,0);
                    newArrow.GetComponent<Arrow>().Distance = army.AttackDistance;
                }
                if (Time.time - timeLastShotArrow > 4.29)
                {
                    arrowShot = false;
                    timeLastShotArrow = Time.time;
                    animator.Play("Attack", 4, 0);
                }
            }
        }
        else
        {
            Move();
        }

        if (!characterController.isGrounded)
        {
            // make sure characters stay on the ground
            characterController.Move(new Vector3(0.0f, -4f * Time.deltaTime, 0.0f));
        }
    }

    public void ChangeAttackMode(bool AttackMode)
    {
        isAttacking = AttackMode;

        if (AttackMode)
        {
            attackDelay = Random.Range(0.1f, 1.5f);
        }
        else
        {
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 0);
            animator.SetLayerWeight(3, 0);
            animator.SetLayerWeight(4, 0);
        }
    }

    private void Move()
    {
        if (destination != Vector2.zero)
        {
            Vector2 distanceToDestination = destination - new Vector2(transform.position.x, transform.position.z);
            if (distanceToDestination.magnitude > 0.1f)
            {
                animator.SetFloat(animIDSpeed, 2);
                distanceToDestination.Normalize();
                Vector3 direction = new Vector3(distanceToDestination.x, 0, distanceToDestination.y);
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                characterController.Move(direction * Time.deltaTime * moveSpeed);
            }
            else
            {
                if (isLeader)
                {
                    // army at destination, so attack if encountering enemies
                    army.WantsToAttack = true;
                }
                animator.SetFloat(animIDSpeed, 0);
            }
        }
    }

    public void ChangeSelection(bool selected)
    {
        if (selected)
        {
            transform.Find("SelectionMarker").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("SelectionMarker").gameObject.SetActive(false);
        }
    }

    public void SetDestination(Vector2 destination)
    {
        this.destination = destination + localPosition;
    }
}
