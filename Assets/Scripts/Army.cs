using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ArmyType_
{
    Archer,
    Swordsman,
    Pikeman
}

public class Army : MonoBehaviour
{
    [SerializeField] int playerNumber;
    [SerializeField] float attackStrength;
    [SerializeField] float defenceStrength;
    [SerializeField] float range;
    [SerializeField] float speed;
    [SerializeField] private bool isArchers;
    [SerializeField] GameObject pfArrow;
    [SerializeField] ArmyType_ armyType;
    Vector2 destination;
    List<Soldier> soldiers = new List<Soldier>();
    Soldier armyLeader;
    GameObject healthbar;
    int activeSoldiers = 10;
    bool isSelected;
    float health = 100;
    bool wantsToAttack = true;          // Indication whether the army wants to first move to the exact location, or directly attack
    float timeLeftShowHealth;
    Vector3 attackDirection;
    float attackDistance;

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
    public GameObject PfArrow { get => pfArrow; set => pfArrow = value; }
    public float AttackDistance { get => attackDistance; set => attackDistance = value; }
    public ArmyType_ ArmyType { get => armyType; set => armyType = value; }
    public float Health { get => health; set => health = value; }

    public void ChangeHealth(float amount)
    {
        health += amount;
        while (activeSoldiers-1 > health/10)
        {
            LetRandomSoldierDie();
        }
        while (activeSoldiers - 1 < (health-10) / 10)
        {
            BringSoldierToLife();
        }
        timeLeftShowHealth = 2;
        healthbar.GetComponent<Slider>().value = 100 - health;
    }

    private void LetRandomSoldierDie()
    {
        activeSoldiers--;
        foreach (Soldier soldier in soldiers)
        {
            if (soldier.IsAlive && (!soldier.IsLeader || activeSoldiers==0))
            {
                soldier.Die();
                break;
            }
        }
    }

    private void BringSoldierToLife()
    {
        activeSoldiers++;
        foreach (Soldier soldier in soldiers)
        {
            if (!soldier.IsAlive)
            {
                soldier.IsAlive = true;
                soldier.gameObject.SetActive(true);
                // the soldier is still at the location where he died, so must be moved back
                soldier.transform.position = armyLeader.transform.position - armyLeader.transform.localPosition + soldier.transform.localPosition;
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        healthbar = Instantiate(Game.Instance.PfHealthbar);
        healthbar.transform.SetParent(Game.Instance.Canvas.transform, false);
        healthbar.name = "Health_" + gameObject.name;
        healthbar.SetActive(false);

        Game.Instance.Players[playerNumber].Armies.Add(this);
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
        if (Game.Instance.GameState != GameState_.Playing)
        {
            return;
        }
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
            HeadQuarters headQuarterUnderAttack = null;

            foreach (var collider in colliders)
            {
                soldierBeingAttacked = collider.GetComponent<Soldier>();
                if (soldierBeingAttacked != null)
                {
                    if (soldierBeingAttacked.Army != null && soldierBeingAttacked.Army.PlayerNumber != playerNumber)
                    {
                        float damage = Time.deltaTime * attackStrength / soldierBeingAttacked.Army.defenceStrength;
                        attackDirection = soldierBeingAttacked.transform.position - armyLeader.transform.position;
                        attackDirection.y = 0;
                        attackDistance = attackDirection.magnitude;
                        attackDirection = attackDirection.normalized;
                        soldierBeingAttacked.Army.ChangeHealth(-damage);
                        attackMode = true;
                        break;
                    }
                }
                headQuarterUnderAttack = collider.GetComponent<HeadQuarters>();
                if (headQuarterUnderAttack != null)
                {
                    if (headQuarterUnderAttack.PlayerNumber != playerNumber)
                    {
                        headQuarterUnderAttack.Damage(Time.deltaTime * attackStrength * 0.1f);
                        attackDirection = headQuarterUnderAttack.transform.position - armyLeader.transform.position;
                        attackDirection.y = 0;
                        attackDistance = attackDirection.magnitude;
                        attackDirection = attackDirection.normalized;
                        attackMode = true;
                        break;
                    }
                }
            }
            UpdateAttackMode(attackMode);
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
        if (Game.Instance.GameState != GameState_.Playing)
        {
            return;
        }
        if (isSelected || timeLeftShowHealth>0)     // draw health bar
        {
            if (timeLeftShowHealth > 0)
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
        if (isSelected == selected)
        {
            // already (un)selected
            return;
        }

        isSelected = selected;

        if (selected)
        {
            HumanPlayer.Instance.SelectedArmies.Add(this);
        }
        else
        {
            HumanPlayer.Instance.SelectedArmies.Remove(this);
        }

        foreach (Soldier soldier in soldiers)
        {
            soldier.ChangeSelection(selected);
        }
    }
}
