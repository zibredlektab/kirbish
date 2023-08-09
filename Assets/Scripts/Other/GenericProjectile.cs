using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectile : MonoBehaviour {

    public float speed = 8;

    void Start() {        

    }

    void FixedUpdate() {
        transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime);
    }
    
    void OnCollisionEnter(Collision collision) {
        Debug.Log("projectile " + gameObject.name + " collided with " + collision.gameObject.name);
        Destroy(gameObject);
    }
    
    public void setSpeed(float newSpeed) {
        speed = newSpeed;
    }
}
