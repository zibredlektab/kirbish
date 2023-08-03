using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyoInteractor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    void OnTriggerEnter(Collider collider) {        
       // transform.parent.BroadcastMessage("OnPain", new Vector3(0,0,0));
        Debug.Log("On Trigger Enter from interactor, with " + collider.gameObject.name);
        
    }
    
}
