using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSuctionable : MonoBehaviour {

    public bool isItem = false; // items are always sucked in & swallowed, even if suction stops. Enemies have to be fully suctioned and deliberately swallowed
    public float suctionSpeedMax = 10f;
    
    private bool suction = false;
    private Vector3 boyoPosition;

    void Start() {
        
    }

    void Update() {
        
    }
    
    void FixedUpdate() {
        if (suction) {
            if (isItem) {
                GetComponent<BoxCollider>().isTrigger = true;
            }
            // Find direction to player
            Vector3 directionToBoyo = new Vector3(0,0,0);
            float distanceToBoyo = boyoPosition.x - transform.position.x;
            
            directionToBoyo.x = suctionSpeedMax / distanceToBoyo; // Enemy should move towards the player, and get faster as it gets closer

            // Fix direction to face player
            if (directionToBoyo.x > 0) {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0F, transform.eulerAngles.z);
            } else {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180F, transform.eulerAngles.z);
            }

            directionToBoyo.x = Mathf.Abs(directionToBoyo.x); // now that we are facing player, direction should only be positive
            
            transform.Translate(directionToBoyo * Time.deltaTime); // Move towards player 
        } else {
            if (isItem) {
                GetComponent<BoxCollider>().isTrigger = false;
            }
        }
    }
    
    
    private void OnSuction(Vector3 boyoPos) {
        Debug.Log("Suctionable " + gameObject.name + " is getting sucked in!");
        suction = true;
        boyoPosition = boyoPos;
    }
    
    private void OnSuctionStop() {
        Debug.Log("Suctionable " + gameObject.name + " is no longer getting sucked in.");
        suction = false;
    }
}
