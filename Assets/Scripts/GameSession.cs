using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    int score = 0;
    bool shieldActive = false, boostActive = false, onehitActive = false;
    [SerializeField] int shieldScore = 2000;
    [SerializeField] int boostScore = 5000;
    [SerializeField] int onehitScore = 10000;
    [SerializeField] Shield shield;
    Player player;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        SetUpSingleton();
        player = FindObjectOfType<Player>();
    }

    private void SetUpSingleton()
    {
        int sessions = FindObjectsOfType<GameSession>().Length;

        if (sessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public int GetScore()
    {
        return score;
    }

    private void Update()
    {
        if (!shieldActive && score > shieldScore)
        {
            Instantiate(shield, player.transform.position, Quaternion.identity);
            shieldActive = true;
        }

        if(!boostActive && score > boostScore)
        {
            player.Boost();
            boostActive = true;
        }

        if (!onehitActive && score > onehitScore)
        {
            player.OneHit();
            onehitActive = true;
        }
    }

    public void AddToScore(int value)
    {
        score += value;
    }

    public void ResetGame()
    {
        Destroy(gameObject);
    }
    
}
