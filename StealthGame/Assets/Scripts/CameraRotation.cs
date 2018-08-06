using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {

    [SerializeField]
    private float clampAmount = 5.0f;
    [SerializeField]
    private float cameraSpeed = 4.0f;
    [SerializeField]
    private float cameraToOriginSpeed = 0.2f;

    private float verticalLookRotation;
    private Vector3 currLocalEulerAngles;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("RVertical") == 0)
        {
            //Lerp back to origin
            verticalLookRotation = Mathf.Lerp(verticalLookRotation, 0.0f, cameraToOriginSpeed);
        }

        //Get the vertical rotation based on input, clamp it and adjust
        verticalLookRotation += Input.GetAxis("RVertical") * cameraSpeed;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -clampAmount, clampAmount);
        Vector3 currAngle = transform.localEulerAngles;
        transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
}
