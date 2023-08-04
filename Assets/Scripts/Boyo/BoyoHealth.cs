using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyoHealth : MonoBehaviour {

    public GameObject meshRoot;
    public int maxHealth = 6;
    public int startingLives = 3;
    public Color painColor;
    
    private int curHealth;
    private int curLives;
    private bool gameOver = false;
    private Color standardColor;
    private Material mat;
    private float lerpTime = 0;
    private bool recovering = false;
    private bool attacking = false;
        
    void Start() {
    
        curHealth = maxHealth;
        curLives = startingLives;
        mat = meshRoot.GetComponent<Renderer>().material;
        standardColor = mat.color;
    }

    void Update() {
        if (curHealth <= 0) {
            EndGame();
        }
        
        if (recovering && mat.color != standardColor) {
            lerpTime += Time.deltaTime * 1.5F;
            Color lerpedColor = Color.Lerp(painColor, standardColor, lerpTime);
            mat.color = lerpedColor;
        } else {
            recovering = false;
            lerpTime = 0;
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
    
    void OnPain(Vector3 vector) {
        curHealth--;
        mat.color = painColor;
        recovering = true;
    }
    
    
    void OnTriggerEnter(Collider collider) {    
        
        if (collider.gameObject.CompareTag("Pain")) {
            if (!recovering && !attacking) {
                Vector3 collisionDirection = new Vector3(0,0,0);
            
                if (transform.position.x - collider.transform.position.x > 0) {
                    collisionDirection.x = 1;
                } else {
                    collisionDirection.x = -1;
                }
            
                gameObject.SendMessage("OnPain", collisionDirection);
            }
        }
    }
    
    void OnAttack() {
        attacking = true;
    }
    
    void OnAttackStop() {
        attacking = false;
    }
}