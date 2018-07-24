using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour {

    #region Private Variables
    private Rigidbody rb;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        //Disable rigidbody gravity and rotation as this is simulated in GravityAttractor script
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
