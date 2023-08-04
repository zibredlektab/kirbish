using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoyoAttack : MonoBehaviour {

    public GameObject breatheInRegion;
    public InputActionAsset actions;
    public GameObject mouth;
    public GameObject projectile;
    public GameObject meshRoot;
    
    private InputAction attackAction;
    private bool attacking = false;
    private GameObject currentlyAttacking; // this should be an array, as multiple enemies can be attacked simultaneously
    private int mouthFull = 0; // 0 = normal, 1 = just caught enemy, 2 = ready to swallow or shoot out, 3 = just shot out
    
    void Start() {
        attackAction = actions.FindActionMap("gameplay").FindAction("attack"); // find the Attack action and store it
    }

    void Update() {
    
        float attack = attackAction.ReadValue<float>(); // get attack button state
        
        if (attack > 0) { // attack button is being pressed
        
            if (mouthFull == 2) {
                // spit out blob
                float blobDirection = 1f;
                float meshDirection = meshRoot.transform.eulerAngles.y;
                if (meshDirection < 1 || meshDirection > 359) {
                    blobDirection = 1f;
                } else if (meshDirection > 179 && meshDirection < 181) {
                    blobDirection = -1f;
                }
                Vector3 blobPosition = new Vector3(transform.position.x + blobDirection, transform.position.y, transform.position.z);
                GameObject blob = Instantiate(projectile, blobPosition, new Quaternion(0,0,0,0));
                blob.GetComponent<GenericProjectile>().direction = blobDirection;
                
                mouth.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                mouthFull = 3;
            } else {
                AttackStart();
            }
        } else if (attacking) { // attack button no longer being pressed, stop attacking
            AttackStop();
        } else if (mouthFull == 1) { // we caught an enemy
            mouth.transform.localScale = new Vector3(0.5f, mouth.transform.localScale.y, 0.5f);
            mouthFull = 2;
        } else if (mouthFull == 3) { // we just spat out an enemy
            mouthFull = 0;
        }
    }
    
    void AttackStart() {
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
        gameObject.BroadcastMessage("OnAttackStop");
    }
    
    void OnAttackStop() {
        attacking = false;
        breatheInRegion.SetActive(false);
        mouth.transform.localScale = new Vector3(mouth.transform.localScale.x, mouth.transform.localScale.y, 0.25f);
        
        if (currentlyAttacking != null) { // we had an enemy in our suction region
            currentlyAttacking.BroadcastMessage("OnSuctionStop"); // tell them suction has ended
            if (mouthFull == 0) currentlyAttacking = null; // we didn't catch an enemy
        }
    }
    
    void OnTriggerStay(Collider collider) {
        if (attacking && collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            currentlyAttacking = collider.gameObject.transform.parent.gameObject;
            currentlyAttacking.BroadcastMessage("OnSuction", transform.position);
            if (Mathf.Abs(transform.position.x - currentlyAttacking.transform.position.x) < 0.5F) { // enemy has been sucked into our danger zone
                mouthFull = 1;
                Destroy(currentlyAttacking);
                currentlyAttacking = null;
                AttackStop();
            }
        }
    }
}
