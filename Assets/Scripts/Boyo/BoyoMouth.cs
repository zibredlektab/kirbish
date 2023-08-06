using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyoMouth : MonoBehaviour {

    private GameObject mainObject;
    private bool suction = false;

    void Start() {
        mainObject = transform.parent.parent.gameObject;
    }

    void Update() {
        
    }
    
    void OnTriggerEnter(Collider collider) {
        Debug.Log("Object " + collider.gameObject.name + " has reached mouth");
        if (suction && collider.gameObject.tag == "Suctionable") {
            Debug.Log("Suctionable object " + collider.gameObject.name + " has reached mouth");
            mainObject.SendMessage("OnEaten", collider.gameObject);
        }    
    }
    
    void OnAttack() {
        suction = true;
    }
    
    void OnAttackStop() {
        suction = false;
    }
}
