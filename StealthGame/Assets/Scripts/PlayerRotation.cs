using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerRotation : MonoBehaviour {

    Quaternion desiredRotation;
    bool isTurning;
    public float rotationSpeed;

    // Use this for initialization
    void Start ()
    {
        isTurning = false;
	}
	// Update is called once per frame
	void Update ()
    {
        // Debug.Log("Desired rotation: " + desiredRotation.eulerAngles);
        // Debug.Log("Current rotation: " + transform.rotation.eulerAngles);
        // Debug.Log("Current local rotation: " + transform.localEulerAngles);

        if (Input.GetAxisRaw("RHorizontal") > 0 && !isTurning)
        {
            //set desired rotation
            desiredRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
            //desiredRotation.eulerAngles = transform.eulerAngles + transform.up * 90;//new Vector3(0, 90, 0);
            //desiredRotation = Quaternion.LookRotation(Vector3.right, Vector3.zero);

            isTurning = true;
        }
        //left on the right stick rotates camera left
        if (Input.GetAxisRaw("RHorizontal") < 0 && !isTurning)
        {
            //set desired rotation
            desiredRotation = transform.rotation * Quaternion.Euler(0, -90, 0);
            //desiredRotation.eulerAngles = transform.eulerAngles - transform.up * 90;//new Vector3(0, 90, 0);
            //desiredRotation.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 90, transform.localEulerAngles.z);
            //desiredRotation = Quaternion.LookRotation(-Vector3.right, Vector3.zero);
            isTurning = true;
        }

        if (isTurning)
        {
            //slerp the rotation to desired rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, desiredRotation, rotationSpeed * Time.deltaTime);
            //if the distance from desired to start is minor
            if (Vector3.Distance(transform.rotation.eulerAngles, desiredRotation.eulerAngles) < 1.0f)
            {
                //stop turning
                transform.rotation = desiredRotation;
                isTurning = false;
            }
        }
    }
}
