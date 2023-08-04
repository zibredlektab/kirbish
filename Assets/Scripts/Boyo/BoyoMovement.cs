using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoyoMovement : MonoBehaviour
{

    public InputActionAsset actions;
    public GameObject meshRoot;
    public Color floatColor;
    public float gravityScale = 3;
    public float jumpSpeed = 20;
    public float moveSpeed = 6;
    public float floatDownSpeed = 5;
    public float totalFloatTime = 5.0F;
    

    private InputAction moveAction;
    private Rigidbody rb;
    private Transform meshTransform;
    public bool onGround = false;
    public bool jumping = false;
    public bool floating = false;
    private float mass;
    private float remainingFloatTime;
    private float bounceCount = 1;
    private float timeFalling;
    private GameObject groundObject;
    private Material mat;
    private Color standardColor;
    public bool recoiling = false; // is boyo being repulsed after pain?
    private bool suction = false;
    

    void Start() {
        moveAction = actions.FindActionMap("gameplay").FindAction("move"); // find the Move action and store it
        actions.FindActionMap("gameplay").FindAction("jump").performed += OnJump;
        
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;
        remainingFloatTime = totalFloatTime;
        mat = meshRoot.GetComponent<Renderer>().material;
        standardColor = mat.color;
        meshTransform = meshRoot.transform;
    }

    // Update is called once per frame
    void Update() {
        
        if (floating) {
            if (remainingFloatTime <= 0) {
                EndFloat();
            } else {
                remainingFloatTime -= Time.deltaTime; // subtract time from remaining float time
            }
        }
        
    }
    
    
    // Physics goes here
    private void FixedUpdate() {
        
        Vector2 moveVector = moveAction.ReadValue<Vector2>();
        float horizontalInput = moveVector.x;
        
        // Rotation
        float direction = meshTransform.eulerAngles.y;
                
        if (((direction < 1 || direction > 359) && horizontalInput < 0) || ((Mathf.Abs(direction) > 179 && Mathf.Abs(direction) < 181) && horizontalInput > 0)) {
            // boyo is facing opposite of input, needs to flip around
            meshTransform.Rotate(new Vector3(meshTransform.eulerAngles.x, 180, meshTransform.eulerAngles.z));
        }        
        
        // Horizontal movement
        if (remainingFloatTime < 2) horizontalInput *= 0.5F; // slower movement when air starts to run out
        if (suction) horizontalInput *= 0.5F; // slower movement while suctioning
        if (recoiling) horizontalInput = 0; // no movement while being repulsed
        
        transform.Translate(new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime);
    
        if (!floating) {
            rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass); // scale up gravity so we fall faster
            if (!onGround) timeFalling += Time.deltaTime; // track how long we've been falling;
        } else if (rb.velocity.y > (-1 * floatDownSpeed)){
            rb.AddForce(Vector3.down * 20);
        }
    
        if (rb.velocity.y > jumpSpeed) rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z); // max vertical speed cap

    }
    
    
    void OnCollisionEnter(Collision collision) {
        //Debug.Log("collision with " + collision.gameObject.name);
        if (collision.GetContact(0).normal.y > .5) { // only bottom-facing normals count as ground collisions
            onGround = true;
            jumping = false;
            groundObject = collision.gameObject;
            remainingFloatTime = totalFloatTime;
            
            if (floating) EndFloat();
            
            if (recoiling) recoiling = false;
        
            if (timeFalling > 1) {
                bounceCount = 2;
            }
            
            timeFalling = 0.0F;
        
            if (bounceCount > 0) {
                rb.AddForce(Vector3.up * 5 * bounceCount, ForceMode.Impulse);
                bounceCount--;
            }
        }
    }
    
    
    void OnCollisionExit(Collision collision) {
        if (onGround && groundObject == collision.gameObject) {
            onGround = false;
        }
    }
    
    
    private void EndFloat() {
        timeFalling = 0.0F;
        floating = false;
        rb.useGravity = true;
        mat.color = standardColor;
    }
    
    
    
    private void OnJump(InputAction.CallbackContext context) {
        if (onGround) {
            jumping = true;
            bounceCount = 1;
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse); // start accelerating up
        } else if (jumping || floating) {
            floating = true;
            jumping = false;
            rb.AddForce(Vector3.up * jumpSpeed * 0.75F, ForceMode.Impulse); // float also pushes up, but not as much as a jump
            rb.useGravity = false; // disable gravity
            mat.color = floatColor;
        }
    }
    
    
    
    void OnAttack() {
        if (floating) EndFloat();
        suction = true;
    }
    
    void OnAttackStop() {
        suction = false;
    }
    
    void OnGameOver() {
        Time.timeScale = 0;
    }
    
    void OnPain(Vector3 direction) {
        float ydir = 7;
        float xdir = 3;
        
        if (direction.y != 0) {
            ydir = direction.y * 7;
        }
        
        if (direction.x != 0) {
            xdir = direction.x * 6;
            
        }
        
        Vector3 repulsion = new Vector3(xdir, ydir, 0);
        
        recoiling = true;
        rb.AddForce(repulsion, ForceMode.Impulse); // push back from pain
        bounceCount = 1;
    }
    
    
    void OnEnable()
    {
        actions.FindActionMap("gameplay").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("gameplay").Disable();
    }
}
