using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartSetUp : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = new GameManager();
        gameManager.InitializeGame();
    }
}
