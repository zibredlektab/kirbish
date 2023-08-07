using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoyoAttack : MonoBehaviour {

    public GameObject suctionRegion;
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

    void FixedUpdate() {
    
        float attack = attackAction.ReadValue<float>(); // get attack button state
        
        if (attack > 0) { // attack button is being pressed
        
            if (mouthFull == 2) {
                // spit out blob
                float projectileDirection = 1f;
                float meshDirection = meshRoot.transform.eulerAngles.y;
                if (meshDirection < 1 || meshDirection > 359) {
                    projectileDirection = 1f;
                } else if (meshDirection > 179 && meshDirection < 181) {
                    projectileDirection = -1f;
                }
                Debug.Log("Projectile Direction should be " + projectileDirection);
                Vector3 projectilePosition = new Vector3(transform.position.x + projectileDirection, transform.position.y, transform.position.z);
                //GameObject blob = Instantiate(projectile, projectilePosition, new Quaternion(0,0,0,0));
                //blob.GetComponent<GenericProjectile>().direction = projectileDirection;
                
                // Set position & direction of projectile
                currentlyAttacking.SetActive(true);
                currentlyAttacking.transform.position = projectilePosition;
                currentlyAttacking.transform.rotation = meshRoot.transform.rotation;
                
                currentlyAttacking = null;

                mouth.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                mouthFull = 3;
            } else if (mouthFull == 0){
                AttackStart();
            }
        } else if (attacking) { // attack button no longer being pressed, stop attacking
            AttackStop();
        } else if (mouthFull == 1) { // we caught an enemy & released the attack button
            mouthFull = 2;
        } else if (mouthFull == 3) { // we just spat out an enemy & released the attack button
            mouthFull = 0;
        }
    }
    
    void AttackStart() {
        suctionRegion.SetActive(true);
        gameObject.BroadcastMessage("OnAttack");

    }
    
    void OnEaten(GameObject eaten) { // called by BoyoMouth when a suctionable object intersects with the mouth
        AttackStop();
        
        GameObject toEat = eaten;
        while (toEat.transform.parent != null) toEat = toEat.transform.parent.gameObject;
        
        Debug.Log("Eating object " + eaten.name);
        
        currentlyAttacking = toEat;
        currentlyAttacking.transform.parent = transform;
        currentlyAttacking.SetActive(false);
        
        // Remove all movement scripts & interactors from applicable eaten objects
        if (!currentlyAttacking.GetComponent<GenericSuctionable>().isItem) {
            Destroy(currentlyAttacking.GetComponent<GenericMovement>()); // Stop object from moving - this needs to be checked for before destruction, items don't have this
            Destroy(currentlyAttacking.transform.Find("Interactor").gameObject); // Stop object from interacting with player
        }
        
        // Reduce the scale of eaten object
        currentlyAttacking.transform.localScale = new Vector3 (currentlyAttacking.transform.localScale.x * 0.75f, currentlyAttacking.transform.localScale.y * 0.75f, currentlyAttacking.transform.localScale.z * 0.75f);

        // Stop physics on eaten object
        currentlyAttacking.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        currentlyAttacking.GetComponent<Rigidbody>().isKinematic = true;
        
        // Remove all tags from the eaten object
        for (int i = 0; i < currentlyAttacking.transform.childCount; i++) { 
            currentlyAttacking.transform.GetChild(i).tag = "Untagged";
        }
        
        // Make the object a projectile
        currentlyAttacking.AddComponent(typeof(GenericProjectile));
        
        mouthFull = 1;
        mouth.transform.localScale = new Vector3(0.5f, mouth.transform.localScale.y, 0.5f);
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
        suctionRegion.SetActive(false);
        mouth.transform.localScale = new Vector3(mouth.transform.localScale.x, mouth.transform.localScale.y, 0.25f);
        
        if (currentlyAttacking != null) { // we had an enemy in our suction region
            currentlyAttacking.BroadcastMessage("OnSuctionStop"); // tell them suction has ended
            if (mouthFull == 0) currentlyAttacking = null; // we didn't catch an enemy
        }
    }
    
    void OnTriggerStay(Collider collider) {
        GameObject collisionObject = collider.gameObject;
        while (collisionObject.transform.parent != null) collisionObject = collisionObject.transform.parent.gameObject; // make sure we're working with the top-level object
        
        if (attacking && collisionObject.CompareTag("Suctionable")) { // We are currently in attack mode, and there is a suctionable object in our range
            
            // Cast a ray that only hits objects on the Landscape layer, to determine if anything is in the way of the current suctionable object
            // Ray starts 0.5 units behind the players transform, so that it will continue to register the suctioned object up until the moment it is eaten
            // Ray extends 4.95 units ahead of player to account for the above 0.5, and reach the end of the 4.45 unit suction region
            
            RaycastHit hit;
            int noPlayer = ~LayerMask.GetMask("Player"); // Get the inverse of the layer mask for Player objects, so our ray can't hit the suction region or any other part of boyo
            
            //Debug.DrawRay(new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z), meshRoot.transform.TransformDirection(Vector3.right) * 4.95f, Color.yellow);
            
            bool ray = Physics.Raycast(new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z), meshRoot.transform.TransformDirection(Vector3.right), out hit, 4.95f, noPlayer);
            
            if (ray && hit.collider.gameObject.layer == LayerMask.NameToLayer("Landscape")) {
                // There is a non-suctionable object in the way
                
                if (currentlyAttacking != null) {
                    currentlyAttacking.BroadcastMessage("OnSuctionStop");
                    currentlyAttacking = null;
                }
                
            } else {
                
                // Nothing is in the way, apply suction
                currentlyAttacking = collisionObject.gameObject;
                currentlyAttacking.BroadcastMessage("OnSuction", transform.position);
            }
        }
    }
}
