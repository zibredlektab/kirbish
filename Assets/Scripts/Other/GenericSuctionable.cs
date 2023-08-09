using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSuctionable : MonoBehaviour {

    public bool isItem = false; // items are always sucked in & swallowed, even if suction stops. Enemies have to be fully suctioned and deliberately swallowed
    public float suctionSpeedMax = 10f;
    
    private bool suction = false;
    public bool continueSuction = false;
    
    private Transform boyoTransform;

    void Start() {
        
    }

    void Update() {
        
    }
    
    void FixedUpdate() {
        if (suction || (isItem && continueSuction)) {
        
        
            Vector3 directionToBoyo = new Vector3(0,0,0);
            Vector3 boyoPosition = boyoTransform.position;
            
            if (isItem) {
                continueSuction = true;
                //directionToBoyo.y = suctionSpeedMax / ((boyoPosition.y - transform.position.y)/2);
                gameObject.GetComponent<Collider>().isTrigger = true;
            }
            
            // Find direction to player
            float distanceToBoyo = boyoPosition.x - transform.position.x;
            
            directionToBoyo.x = suctionSpeedMax / (distanceToBoyo/2); // Enemy should move towards the player, and get faster as it gets closer

            // Fix direction to face player
            if (!isItem) { // only enemies turn to face player, items stay as they are
                if (directionToBoyo.x > 0) {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0F, transform.eulerAngles.z);
                } else {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180F, transform.eulerAngles.z);
                }                
                directionToBoyo.x = Mathf.Abs(directionToBoyo.x); // now that we are facing player, direction should only be positive
            }
            
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            
            rb.Move(new Vector3(rb.position.x + (directionToBoyo.x * Time.deltaTime), rb.position.y + (directionToBoyo.y * Time.deltaTime), rb.position.z), rb.rotation); // Move towards player
        }
    }
    
    
    private void OnSuction(Transform boyoTrans) {
        //Debug.Log("Suctionable " + gameObject.name + " is getting sucked in!");
        suction = true;
        boyoTransform = boyoTrans;
    }
    
    private void OnSuctionStop() {
        Debug.Log("Suctionable " + gameObject.name + " is no longer getting sucked in.");
        suction = false;
    }
}
