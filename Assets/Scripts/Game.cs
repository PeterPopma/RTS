using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState_
{
    Playing,
    GameOver
}

public class Game : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject pfHealthbar;
    [SerializeField] GameObject panelGameOver;
    [SerializeField] GameObject baseBlue;
    [SerializeField] GameObject baseRed;
    [SerializeField] TextMeshProUGUI textPlayerWon;
    public static Game Instance;
    private List<Army> armies = new List<Army>();
    private List<Army> selectedArmies = new List<Army>();
    float cameraHeight = 100;
    GameState_ gameState;
    int playerWon;

    public List<Army> Armies { get => armies; set => armies = value; }
    public List<Army> SelectedArmies { get => selectedArmies; set => selectedArmies = value; }
    public float CameraHeight { get => cameraHeight; set => cameraHeight = value; }
    public GameObject PfHealthbar { get => pfHealthbar; set => pfHealthbar = value; }
    public Canvas Canvas { get => canvas; set => canvas = value; }
    public GameState_ GameState { get => gameState; set => gameState = value; }
    public int PlayerWon { get => playerWon; set => playerWon = value; }

    public void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        NewGame();
    }

    void Update()
    {
    }

    private void NewGame()
    {
        gameState = GameState_.Playing;
        panelGameOver.SetActive(false);
        baseBlue.SetActive(true);
        baseRed.SetActive(true);
    }

    public void SetGameState(GameState_ gameState)
    {
        switch (gameState)
        {
            case GameState_.GameOver:
                panelGameOver.SetActive(true);
                break;
        }
        this.gameState = gameState;
    }

    private bool WantsToAttackEnemy(Vector2 destination)
    {
        int playerNumber = SelectedArmies[0].PlayerNumber;
        Collider[] colliders = Physics.OverlapSphere(new Vector3(destination.x, destination.y, 0), 10);

        foreach (var collider in colliders)
        {
            Soldier soldier = collider.GetComponent<Soldier>();
            if (soldier != null)
            {
                if (soldier.Army.PlayerNumber != playerNumber)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetArmyDestination(Vector2 destination)
    {
        bool attackingEnemy = WantsToAttackEnemy(destination);
        foreach (var army in SelectedArmies)
        {
            army.WantsToAttack = attackingEnemy;
            army.SetDestination(destination);
            if (!attackingEnemy)
            {
                army.UpdateAttackMode(false);
            }
        }
    }

    public void RemoveSelection()
    {
        foreach (var army in SelectedArmies)
        {
            army.IsSelected = false; 
            foreach (Soldier soldier in army.Soldiers)
            {
                soldier.ChangeSelection(false);
            }
        }
        SelectedArmies.Clear();
    }
}
