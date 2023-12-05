using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState_
{
    Playing,
    GameOver
}

public class Game : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject pfHealthbar;
    [SerializeField] GameObject pfProgressBar;
    [SerializeField] GameObject panelGameOver;
    [SerializeField] TextMeshProUGUI textPlayerWon;
    [SerializeField] float cameraHeight = 100;
    [SerializeField] Player[] players;      // array of players, contains human player at index 0 and computer player at index 1
    public static Game Instance;
    GameState_ gameState;
    int playerWon;
    private List<GameObject> trees = new List<GameObject>();

    public float CameraHeight { get => cameraHeight; set => cameraHeight = value; }
    public GameObject PfHealthbar { get => pfHealthbar; set => pfHealthbar = value; }
    public Canvas Canvas { get => canvas; set => canvas = value; }
    public GameState_ GameState { get => gameState; set => gameState = value; }
    public int PlayerWon { get => playerWon; set => playerWon = value; }
    public List<GameObject> Trees { get => trees; set => trees = value; }
    public GameObject PfProgressBar { get => pfProgressBar; set => pfProgressBar = value; }
    public Player[] Players { get => players; set => players = value; }

    public void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        HumanPlayer.Instance.Player = players[0];
        NewGame();
    }
    

    void Update()
    {
    }

    private void NewGame()
    {
        players[0].NewGame();
        players[1].NewGame();
        Trees.Clear();
        GameObject.Find("Scripts/TreeSpawner").GetComponent<TreeSpawner>().SpawnTrees();
        gameState = GameState_.Playing;
        panelGameOver.SetActive(false);
    }

    public void SetGameState(GameState_ gameState)
    {
        switch (gameState)
        {
            case GameState_.GameOver:
                textPlayerWon.text = "Player " + playerWon + " has won the game.";
                panelGameOver.SetActive(true);
                break;
        }
        this.gameState = gameState;
    }

    public GameObject SelectClosestTree(Vector3 position)
    {
        GameObject closestTree = null;
        float closestDistance = float.MaxValue;
        foreach (GameObject tree in trees)
        {
            float distance = (new Vector2(position.x, position.y) - new Vector2(tree.transform.position.x, tree.transform.position.z)).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTree = tree;
            }
        }
        trees.Remove(closestTree);

        return closestTree;
    }

    public void OnCreateDemo1()
    {
        DemoScene.Instance.Create();
    }

}
