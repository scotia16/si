using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState
{
    PreGame,
    InProgress,
    PostGame
}

public class GameController : MonoBehaviour {

    public int scorePerEnemy;
    public GameObject invaderControllerObject;
    public GameObject playerControllerObject;

    private InvaderController invaderController;
    private PlayerController playerController;
    private int score;
    
    private bool userWon = true;
    private GameState gameState = GameState.PreGame;

	void Start ()
    {
        if (invaderControllerObject == null || playerControllerObject == null)
        {
            Debug.LogError("Missing player and invader controller prefab references");
        }

        invaderController = invaderControllerObject.GetComponent<InvaderController>();
        playerController = playerControllerObject.GetComponent<PlayerController>();

        if (invaderController == null || playerController == null)
        {
            Debug.LogError("Missing player or invader controller");
        }
	}
	
	void Update ()
    {
        if(gameState == GameState.InProgress && !invaderController.EnemiesExist())
        {
            EndGame(true);
        }
	}

    public void AddToScore()
    {
        score += scorePerEnemy;
    }

    public void UserDestroyed()
    {
        invaderController.StopAssult();
        EndGame(false);
    }

    private void EndGame(bool userWon)
    {
        gameState = GameState.PostGame;
        this.userWon = userWon;
        playerController.Reset();
        invaderController.Reset();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(225, 20, 70, 20), "Score: ");
        GUI.Label(new Rect(270, 20, 70, 20), score.ToString());
        GUI.Label(new Rect(215, 650, 70, 20), "Health: ");
        GUI.Label(new Rect(265, 650, 70, 20), playerController.Health.ToString());
        GUI.Label(new Rect(165, 680, 190, 20), "Move - Left/Right | Fire - Space");
        if (gameState == GameState.InProgress) return;
        
        if (GUI.Button(new Rect(210, 220, 80, 20), "Start"))
        {
            score = 0;
            invaderController.StartAssult();
            gameState = GameState.InProgress;
        }

        if(gameState == GameState.PostGame)
        {
            string resultText = userWon ? "YOU WIN" : "GAME OVER";
            GUI.Label(new Rect(210, 500, 110, 20), resultText);
        }
    }
}
