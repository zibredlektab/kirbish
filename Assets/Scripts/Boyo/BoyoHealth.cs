using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyoHealth : MonoBehaviour {

    public int maxHealth = 6;
    public int startingLives = 3;
    
    private int curHealth;
    private int curLives;
    private bool gameOver = false;
        
    void Start() {
    
        curHealth = maxHealth;
        curLives = startingLives;
        
    }

    void Update() {
        if (curHealth <= 0) {
            EndGame();
        }
    }
    
    void OnGUI() {
        GUI.Box(new Rect(10,10,100,50), "Health");
        GUI.Label(new Rect(20,20,100,50), "" + curHealth);
        if (gameOver) {
            GUI.Box(new Rect(100,100,100,50), "Game Over");
        }
    }
    
    void EndGame() {
        gameObject.SendMessage("OnGameOver");
    }
    
    void OnGameOver() {
        gameOver = true;
    }
    
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Pain") {
            Debug.Log("ow!");
            curHealth--;
        }
    }
    
}