using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyoMovement : MonoBehaviour
{

    public float gravityScale = 3;
    public float jumpSpeed = 20;
    public float moveSpeed = 6;
    public float floatDownSpeed = 5;
    public float totalFloatTime = 5.0F;
    

    private Rigidbody rb;
    private bool onGround = false;
    private bool jumping = false;
    private bool floating = false;
    private float mass;
    private float remainingFloatTime;
    private float bounceCount = 1;
    private float timeFalling;
    private GameObject groundObject;
    
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;
        remainingFloatTime = totalFloatTime;
    }

    // Update is called once per frame
    void Update() {
        // Horizontal movement
        float horizontalInput = Input.GetAxis("Horizontal");
        if (remainingFloatTime < 2) horizontalInput *= 0.5F;
        transform.Translate(new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime);
        
        if (floating) {
            if (remainingFloatTime <= 0) {
                endFloat();
            } else {
                remainingFloatTime -= Time.deltaTime; // subtract time from remaining float time
            }
        }
        
    }
    
    private void endFloat() {
        Debug.Log("ending float");
        timeFalling = 0.0F;
        floating = false;
        rb.useGravity = true;
    }
    
    private void FixedUpdate() {
        if (!floating) {
            rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass); // scale up gravity so we fall faster
            if (!onGround) timeFalling += Time.deltaTime; // track how long we've been falling;
        } else if (rb.velocity.y > (-1 * floatDownSpeed)){
            rb.AddForce(Vector3.down * 20);
        }
        
        if (rb.velocity.y > jumpSpeed) rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z); // max vertical speed cap
    }
    
    
    void OnCollisionEnter(Collision collision) {
        if (collision.GetContact(0).normal.y > .9) { // only bottom-facing normals count as ground collisions
            Debug.Log("on ground");
            onGround = true;
            jumping = false;
            groundObject = collision.gameObject;
            remainingFloatTime = totalFloatTime;
            
            if (floating) endFloat();
        
            if (timeFalling > 1) {
                bounceCount = 2;
                Debug.Log("should bounce twice");
            }
            
            timeFalling = 0.0F;
        
            if (bounceCount > 0) {
                Debug.Log("bouncing");
                rb.AddForce(Vector3.up * 5 * bounceCount, ForceMode.Impulse);
                bounceCount--;
            }
        }
    }
    
    
    void OnCollisionExit(Collision collision) {
        if (onGround && groundObject == collision.gameObject) {
            onGround = false;
            Debug.Log("no longer on ground");
        }
    }
    
    
    
    void OnMove() {
    }
    
    
    
    void OnJump() {
        if (onGround) {
            Debug.Log("jump");
            jumping = true;
            bounceCount = 1;
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse); // start accelerating up
        } else if (jumping || floating) {
            Debug.Log("floating");
            floating = true;
            jumping = false;
            rb.AddForce(Vector3.up * jumpSpeed * 0.75F, ForceMode.Impulse); // float also pushes up, but not as much as a jump
            rb.useGravity = false; // disable gravity
        }
    }
    
    
    
    void OnAttack() {
        Debug.Log("attack");
        if (floating) endFloat();
    }
    
    
}
