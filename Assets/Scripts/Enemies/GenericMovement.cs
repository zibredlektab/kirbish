using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovement : MonoBehaviour {


    public float moveSpeed = 5;
    public float turnDuration = 2;
    
    private Vector3 targetAngles;
    private Vector3 startAngles;
    public bool shouldRotate = true; // has a collision occurred?
    public bool rotating = false; // are we currently rotating?
    private float timeRotationStarted;
    private Rigidbody rb;
    
    public bool suction = false;
    public bool moving = false;
    public float xPosition;

    

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        moving = false;
        xPosition = transform.position.x;
        
        if (suction) {
            // Don't move if suction is happening (handled by GenericMovement)
            rotating = false;
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
                Debug.Log("Target rotation has been reached");
                rotating = false;
                // Ensure we're always facing 100% left or right    
                if (transform.eulerAngles.y < 1 || transform.eulerAngles.y > 359) {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0F, transform.eulerAngles.z);
                } else if (transform.eulerAngles.y > 179 && transform.eulerAngles.y < 181) {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180F, transform.eulerAngles.z);
                }
            }
        } else {
            moving = true;
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
    
    void OnCollisionStay(Collision collider) {
        OnCollisionEnter(collider);
    }
    
    void OnTriggerEnter(Collider collider) {
        //Debug.Log("Enemy " + gameObject.name + " entered trigger " + collider.gameObject.name);
    }
    
    private void OnSuction(Vector3 boyoPos) {
        suction = true;
    }
    
    private void OnSuctionStop() {
        suction = false;
    }
}


