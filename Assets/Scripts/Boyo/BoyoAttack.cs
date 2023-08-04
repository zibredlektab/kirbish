using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoyoAttack : MonoBehaviour
{
    public GameObject breathInRegion;
    public InputActionAsset actions;
    
    
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
        attacking = true;
    }
    
    void AttackStop() {
        Debug.Log("no longer attacking");
        attacking = false;
    }
}
