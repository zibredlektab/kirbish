using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovement : MonoBehaviour {


    public float moveSpeed = 5;
    public float turnDuration = 2;
    
    private Vector3 targetAngles;
    private Vector3 startAngles;
    private bool shouldRotate = true; // has a collision occurred?
    private bool rotating = false; // are we currently rotating?
    private float timeRotationStarted;
    private Rigidbody rb;
    
    private bool suction = false;
    private Vector3 boyoPosition;
    

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        if (suction) {
            
            // Find direction to player
            Vector3 directionToBoyo = new Vector3(0,0,0);
            float distanceToBoyo = boyoPosition.x - transform.position.x;
            
            directionToBoyo.x = (moveSpeed * 2) / distanceToBoyo; // Enemy should move towards the player, and get faster as it gets closer

            // Fix direction to face player
            rotating = false;
            if (directionToBoyo.x > 0) {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0F, transform.eulerAngles.z);
            } else {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180F, transform.eulerAngles.z);
            }

            directionToBoyo.x = Mathf.Abs(directionToBoyo.x); // now that we are facing player, direction should only be positive
            
            transform.Translate(directionToBoyo * Time.deltaTime); // Move towards player     
            
        } else if (!suction && shouldRotate && !rotating) {
            startAngles = transform.eulerAngles;
            if (startAngles.y == 0F) targetAngles.y = 180F;
            if (startAngles.y == 180F) targetAngles.y = 0F;
            rotating = true;
            
        } else if (rotating) {
            if (shouldRotate) {
                // executes on first frame of rotation only                
                timeRotationStarted = Time.time;
                shouldRotate = false;
            }
            transform.eulerAngles = Vector3.Lerp(startAngles, targetAngles, (Time.time - timeRotationStarted) / turnDuration); // Rotate a little bit each frame
            
            // Check if target rotation has been reached
            if (Mathf.Abs(transform.eulerAngles.y - targetAngles.y) < 1) {
                rotating = false;
                // Ensure we're always facing 100% left or right    
                if (transform.eulerAngles.y < 1 || transform.eulerAngles.y > 359) {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0F, transform.eulerAngles.z);
                } else if (transform.eulerAngles.y > 179 && transform.eulerAngles.y < 181) {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180F, transform.eulerAngles.z);
                }
            }
        } else {
            transform.Translate(new Vector3(moveSpeed, 0, 0) * Time.deltaTime); // Move forwards
        }
   }
    
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) return; // ignore ground
        
        // Turn around when an obstacle is hit
        if (!rotating && !shouldRotate) {
            shouldRotate = true;
        }
    }
    
    void OnTriggerEnter(Collider collider) {
        //Debug.Log("Enemy " + gameObject.name + " entered trigger " + collider.gameObject.name);
    }
    
    private void OnSuction(Vector3 boyoPos) {
        Debug.Log("Enemy " + gameObject.name + " is getting sucked in!");
        suction = true;
        boyoPosition = boyoPos;
    }
    
    private void OnSuctionStop() {
        Debug.Log("Enemy " + gameObject.name + " is no longer getting sucked in.");
        suction = false;
    }
}


