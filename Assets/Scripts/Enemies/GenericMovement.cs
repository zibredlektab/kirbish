using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovement : MonoBehaviour {


    public float moveSpeed = 5;
    public float turnDuration = 2;
    
    private Vector3 targetAngles;
    private Vector3 startAngles;
    private bool rotating = false;
    private float timeRotationStarted;
    private Rigidbody rb;
    

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        if (rotating) {        
            transform.eulerAngles = Vector3.Lerp(startAngles, targetAngles, (Time.time - timeRotationStarted) / turnDuration); // Rotate a little bit each frame
            
            // Check if target rotation has been reached (approximately)
            if (transform.eulerAngles.y < 1 || transform.eulerAngles.y > 359) {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0F, transform.eulerAngles.z);
                rotating = false;
            } else if (transform.eulerAngles.y > 179 && transform.eulerAngles.y < 181) {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180F, transform.eulerAngles.z);
                rotating = false;
            }
        } else {
            transform.Translate(new Vector3(moveSpeed, 0, 0) * Time.deltaTime); // Move forwards
        }
   }
    
    void OnCollisionEnter(Collision collision) {
        // Turn around when an obstacle is hit
        if (!rotating) {
            rotating = true;
            targetAngles = transform.eulerAngles + 180F * Vector3.up;
            startAngles = transform.eulerAngles;
            timeRotationStarted = Time.time;
        }
    }
    
    void OnTriggerEnter(Collider collider) {
        //Debug.Log("Enemy " + gameObject.name + " entered trigger " + collider.gameObject.name);
    }
}


