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
        GameObject toEat = collider.gameObject;
        while (toEat.transform.parent != null) toEat = toEat.transform.parent.gameObject;
        if (toEat.tag == "Suctionable") {
            GenericSuctionable suctionable = toEat.GetComponent<GenericSuctionable>();
            if ((suctionable.isItem && suctionable.continueSuction) || suction) {
                Debug.Log("Suctionable object " + toEat.name + " has reached mouth");
                mainObject.SendMessage("OnEaten", toEat);
            }
        }
    }
    
    void OnAttack() {
        suction = true;
    }
    
    void OnAttackStop() {
        suction = false;
    }
}
