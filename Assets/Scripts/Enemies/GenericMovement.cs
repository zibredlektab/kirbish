using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovement : MonoBehaviour {


    public float moveSpeed = 5;
    public float turnSpeed = 2;
    
    private Vector3 targetAngles;
    private Vector3 startAngles;
    private bool rotating = false;
    private float timeRotationStarted;

    void Start() {
        
    }

    void FixedUpdate() {
        if (rotating) {        
            transform.eulerAngles = Vector3.Lerp(startAngles, targetAngles, (Time.time - timeRotationStarted) / turnSpeed);

            Debug.Log("angle is " + transform.eulerAngles.y);
            if (transform.eulerAngles.y < 1 || transform.eulerAngles.y > 359) {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0F, transform.eulerAngles.z);
                rotating = false;
            } else if (transform.eulerAngles.y > 179 && transform.eulerAngles.y < 181) {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180F, transform.eulerAngles.z);
                rotating = false;
            } else {
                Debug.Log("still rotating");
            }
        } else {
            transform.Translate(new Vector3(moveSpeed, 0, 0) * Time.deltaTime);
        }
   }
    
    void OnCollisionEnter(Collision collision) {
        rotating = true;
        targetAngles = transform.eulerAngles + 180F * Vector3.up;
        startAngles = transform.eulerAngles;
        timeRotationStarted = Time.time;
    }
}


