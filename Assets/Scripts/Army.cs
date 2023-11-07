using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Army : MonoBehaviour
{
    [SerializeField] int playerNumber;
    [SerializeField] float attackStrength;
    [SerializeField] float defenceStrength;
    [SerializeField] float range;
    [SerializeField] float speed;
    [SerializeField] private bool isArchers;
    Vector2 destination;
    List<Soldier> soldiers = new List<Soldier>();
    Soldier armyLeader;
    GameObject healthbar;
    int activeSoldiers = 10;
    bool isSelected;
    float health = 100;
    bool wantsToAttack = true;
    float timeLeftShowHealth;
    Vector3 attackDirection;

    public Vector2 Destination { get => destination; set => destination = value; }
    public Soldier ArmyLeader { get => armyLeader; set => armyLeader = value; }
    public int PlayerNumber { get => playerNumber; set => playerNumber = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }
    public List<Soldier> Soldiers { get => soldiers; set => soldiers = value; }
    public float AttackStrength { get => attackStrength; set => attackStrength = value; }
    public float DefenceStrength { get => defenceStrength; set => defenceStrength = value; }
    public float Range { get => range; set => range = value; }
    public float Speed { get => speed; set => speed = value; }
    public bool IsArchers { get => isArchers; set => isArchers = value; }
    public bool WantsToAttack { get => wantsToAttack; set => wantsToAttack = value; }
    public float TimeLeftShowHealth { get => timeLeftShowHealth; set => timeLeftShowHealth = value; }
    public int ActiveSoldiers { get => activeSoldiers; set => activeSoldiers = value; }
    public GameObject Healthbar { get => healthbar; set => healthbar = value; }
    public Vector3 AttackDirection { get => attackDirection; set => attackDirection = value; }

    private void Awake()
    {
    }

    public void ChangeHealth(float amount)
    {
        health += amount;
        while (activeSoldiers-1 > health/10)
        {
            activeSoldiers--;
            LetRandomSoldierDie();
        }
        timeLeftShowHealth = 2;
        healthbar.GetComponent<Slider>().value = health;
    }

    private void LetRandomSoldierDie()
    {
        foreach(Soldier soldier in soldiers)
        {
            if (soldier.IsAlive && (!soldier.IsLeader || activeSoldiers==0))
            {
                soldier.Die();
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        healthbar = Instantiate(Game.Instance.PfHealthbar);
        healthbar.transform.SetParent(Game.Instance.Canvas.transform, false);
        healthbar.name = "Health_" + gameObject.name;
        healthbar.SetActive(false);

        Game.Instance.Armies.Add(this);
        foreach (Transform child in transform)
        {
            Soldier soldier = child.GetComponent<Soldier>();
            if (soldier != null)
            {
                soldier.SetParent(this);
                soldiers.Add(soldier);
                if (soldier.IsLeader)
                {
                    armyLeader = soldier;
                }
            }
        }
    }

    void FixedUpdate()
    {
        DealDamageToUnits();
    }

    private void DealDamageToUnits()
    {
        if (wantsToAttack)
        {
            bool attackMode = false;
            Collider[] colliders = Physics.OverlapSphere(armyLeader.transform.position, range);
            //            DebugExtension.DebugWireSphere(armyLeader.transform.position, Color.white, range);
            Soldier soldierBeingAttacked = null;

            foreach (var collider in colliders)
            {
                soldierBeingAttacked = collider.GetComponent<Soldier>();
                if (soldierBeingAttacked != null)
                {
                    if (soldierBeingAttacked.Army != null && soldierBeingAttacked.Army.PlayerNumber != playerNumber)
                    {
                        attackMode = true;
                        break;
                    }
                }
            }
            UpdateAttackMode(attackMode);
            if (attackMode)
            {
                attackDirection = (soldierBeingAttacked.transform.position - armyLeader.transform.position).normalized;
                float damage = Time.deltaTime * attackStrength / soldierBeingAttacked.Army.defenceStrength;
                soldierBeingAttacked.Army.ChangeHealth(-damage);
            }
        }
    }

    public void UpdateAttackMode(bool attackMode)
    {
        foreach(Soldier soldier in soldiers)
        {
            if (soldier.IsAlive && attackMode!=soldier.IsAttacking)
            {
                soldier.ChangeAttackMode(attackMode);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected || timeLeftShowHealth>0)     // draw health bar
        {
            if (timeLeftShowHealth >0)
            {
                timeLeftShowHealth -= Time.deltaTime;
            }
            Vector3 screenPos = Camera.main.WorldToScreenPoint(armyLeader.transform.position + new Vector3(4, 0, -6));
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Game.Instance.Canvas.GetComponent<RectTransform>(), screenPos, null, out canvasPos);

            if (screenPos.z > 0)
            {
                healthbar.SetActive(true);
                healthbar.GetComponent<RectTransform>().anchoredPosition = canvasPos;
            }
            else
            {
                healthbar.SetActive(false);
            }
        }
        else
        {
            healthbar.SetActive(false);
        }
    }

    public void SetDestination(Vector2 destination)
    {
        this.destination = destination;

        foreach (Soldier soldier in soldiers) {
            soldier.SetDestination(destination);
        }
    }

    public void ChangeSelection(bool selected)
    {
        isSelected = selected;

        if (selected)
        {
            Game.Instance.SelectedArmies.Add(this);
        }
        else
        {
            Game.Instance.SelectedArmies.Remove(this);
        }

        foreach (Soldier soldier in soldiers)
        {
            soldier.ChangeSelection(selected);
        }
    }
}
