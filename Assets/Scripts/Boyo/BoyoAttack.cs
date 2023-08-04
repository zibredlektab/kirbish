using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoyoAttack : MonoBehaviour
{
    public GameObject breatheInRegion;
    public InputActionAsset actions;
    public GameObject mouth;
    
    private InputAction attackAction;
    private bool attacking = false;
    
    void Start() {
        attackAction = actions.FindActionMap("gameplay").FindAction("attack"); // find the Attack action and store it
    }

    void Update() {
        if (attackAction.ReadValue<float>() > 0) {
            AttackStart();
        } else if (attacking) {
            AttackStop();
        }
    }
    
    void AttackStart() {
        Debug.Log("attacking");
        breatheInRegion.SetActive(true);
        gameObject.BroadcastMessage("OnAttack");
    }
    
    void OnAttack() {
        if (!attacking) {
            attacking = true;
            mouth.transform.localScale = new Vector3(mouth.transform.localScale.x, mouth.transform.localScale.y, 0.75f);        
        }
    }
    
    void AttackStop() {
        Debug.Log("no longer attacking");
        gameObject.BroadcastMessage("OnAttackStop");
    }
    
    void OnAttackStop() {
        attacking = false;
        breatheInRegion.SetActive(false);
        mouth.transform.localScale = new Vector3(mouth.transform.localScale.x, mouth.transform.localScale.y, 0.25f);
    }
}
