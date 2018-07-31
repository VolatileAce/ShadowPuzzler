using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    #region Inspector vars
    [SerializeField] private float walkSpeed = 6;
    [SerializeField] private float wallDetection = 0.5f;
    [SerializeField] private float rotSpeed = 1.0f;
    #endregion

    #region Private vars
    private Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    private Rigidbody rb;
    private Vector3 direction = Vector3.zero;
    private bool rotate = false;
    private Quaternion oldRot;
    private Quaternion targetRot;
    private float rotTimer;
    private RaycastDetection raycastDetection;
    private bool detectFloor = false;
    private bool canMove = true;
    private float controlStaticTimer;
    private float inputX;
    private float inputY;
    private bool onWall = false;
    private bool changeRotateDir = true;
    private Vector3 localMove;
    private bool canRotate = false;
    private bool rotateToZero = false;
    private float rotZeroTimer = 0.0f;
    private bool onGround = false;
    private bool fallOff = false;
    private PlayerRotation playerRotation;
    #endregion

    #region Get Set
    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value; }
    }
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerRotation = GetComponent<PlayerRotation>();
        raycastDetection = GetComponent<RaycastDetection>();
    }

    void Update()
    {
        //Raycasting to detect wall
        DetectWalls();

        if (canMove)
        {
            controlStaticTimer = 0.0f;

            //Calculate movement
            inputX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxisRaw("Vertical");

            Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
            Vector3 targetMoveAmount = moveDir * walkSpeed;
            moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
        }
        else
        {
            inputX = 0.0f;
            inputY = 0.0f;
            moveAmount = Vector3.zero;
            controlStaticTimer += Time.deltaTime;
        }

        if (onWall)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            onGround = false;
        }
        else
        {
            rb.useGravity = true;
        }

        if (controlStaticTimer > 1.2f)
        {
            canMove = true;
            playerRotation.enabled = true;
        }

        #region Old Camera rotation 
        ////Look rotation (option for controller or mouse)
        //if (!controller)
        //{
        //    transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
        //    verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        //}
        //else
        //{
        //    transform.Rotate(Vector3.up * Input.GetAxis("RHorizontal") * mouseSensitivityX);
        //    verticalLookRotation += Input.GetAxis("RVertical") * mouseSensitivityY;
        //}
        //
        //verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        //cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
        #endregion
    }

    private void LateUpdate()
    {
        //Rotation across walls
        RotatePlayer();

        if(fallOff)
        {
           FallOffWall();
        }

        FallOutOfShadow();

        DetachOffWall();
    }

    private void DetectWalls()
    {
        RaycastHit objectHit;

        //Shoot raycast in four directions
        if (canMove)
        {
            //Downwards detection
            if (!Physics.Raycast(transform.position, -transform.up, out objectHit, 0.5f))
            {
                fallOff = true;
            }

            //Forward
            if (Physics.Raycast(transform.position, transform.forward, out objectHit, wallDetection))
            {
                Debug.Log("Forward Raycast on: " + objectHit.collider);
                Debug.DrawRay(transform.position, transform.forward, Color.blue);
                canRotate = true;

                if (objectHit.transform.tag == "Wall")
                {
                    direction = objectHit.normal.normalized;
                    detectFloor = false;
                }
                else if (objectHit.transform.tag == "Floor")
                {
                    direction = objectHit.normal.normalized;
                    detectFloor = true;
                }
            }

            //Right
            else if (Physics.Raycast(transform.position, transform.right, out objectHit, wallDetection))
            {
                Debug.Log("Right Raycast on: " + objectHit.collider);
                Debug.DrawRay(transform.position, transform.right, Color.blue);
                canRotate = true;

                if (objectHit.transform.tag == "Wall")
                {
                    direction = objectHit.normal.normalized;
                    detectFloor = false;
                }
                else if (objectHit.transform.tag == "Floor")
                {
                    direction = objectHit.normal.normalized;
                    detectFloor = true;
                }
            }

            //Back
            else if (Physics.Raycast(transform.position, -transform.forward, out objectHit, wallDetection))
            {
                Debug.Log("Back Raycast on: " + objectHit.collider);
                Debug.DrawRay(transform.position, -transform.forward, Color.blue);
                canRotate = true;

                if (objectHit.transform.tag == "Wall")
                {
                    direction = objectHit.normal.normalized;
                    detectFloor = false;
                }
                else if (objectHit.transform.tag == "Floor")
                {
                    direction = objectHit.normal.normalized;
                    detectFloor = true;
                }
            }

            //Left
            else if (Physics.Raycast(transform.position, -transform.right, out objectHit, wallDetection))
            {
                Debug.Log("Left Raycast on: " + objectHit.collider);
                Debug.DrawRay(transform.position, -transform.right, Color.blue);
                canRotate = true;

                if (objectHit.transform.tag == "Wall")
                {
                    direction = objectHit.normal.normalized;
                    detectFloor = false;
                }
                else if (objectHit.transform.tag == "Floor")
                {
                    direction = objectHit.normal.normalized;
                    detectFloor = true;
                }
            }
            else
            {
                direction = Vector3.zero;
                canRotate = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (onGround)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            fallOff = false;
        }

        if (canMove)
        {
            //Apply movement to rigidbody
            localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + localMove);
        }
        else
        {
            localMove = Vector3.zero;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void RotatePlayer()
    {
        //Leaving Wall
        if (Input.GetButtonDown("Fire1") && direction != Vector3.zero && detectFloor == true)
        {
            canMove = false;
            playerRotation.enabled = false;
            rotTimer = 0.0f;
            oldRot = transform.rotation;

            RotateDirection();

            rotate = true;
            direction = Vector3.zero;
            onWall = false;
        }
        //Mounting wall
        else if (Input.GetButtonDown("Fire1") && direction != Vector3.zero && raycastDetection.InShadow == true)
        {
            canMove = false;
            playerRotation.enabled = false;
            onWall = true;
            rotTimer = 0.0f;
            oldRot = transform.rotation;

            RotateDirection();

            #region Old Detect
            //Looking into wall
            //if (Vector3.Dot(transform.forward, direction) < -0.95f)
            //{
            //    targetRot = Quaternion.LookRotation(Vector3.up, direction);
            //}
            ////Looking Away From Wall
            //else if (Vector3.Dot(transform.forward, direction) > 0.95f)
            //{
            //    targetRot = Quaternion.LookRotation(Vector3.down, direction);
            //}
            ////Looking Across the wall
            //else if (Vector3.Dot(transform.forward, direction) < 0.05f &&
            //    Vector3.Dot(transform.forward, direction) > -0.05f)
            //{
            //    targetRot = Quaternion.LookRotation(transform.forward, direction);
            //}
            #endregion

            rotate = true;
            direction = Vector3.zero;
        }


        if (rotate == true)
        {
            rotTimer += Time.deltaTime;

            if (transform.rotation != targetRot)
            {
                transform.rotation = Quaternion.Slerp(oldRot, targetRot, rotTimer * rotSpeed);
            }
        }

        if (rotTimer > 1)
        {
            rotate = false;
            transform.rotation = targetRot;
            changeRotateDir = true;
            rotTimer = 0.0f;
        }
    }

    private void DetachOffWall()
    {
        if (onWall && !canRotate)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                rb.AddForce(Vector3.down);
                changeRotateDir = true;
                direction = Vector3.up;
                onWall = false;
                rotateToZero = true;
                RotateDirection();
            }
        }

        if (!onWall && rotateToZero)
        {
            rotZeroTimer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotZeroTimer * rotSpeed);
        }

        if (rotZeroTimer > 1)
        {
            rotateToZero = false;
            transform.rotation = targetRot;
            changeRotateDir = true;
            rotZeroTimer = 0;
        }
    }

    private void FallOutOfShadow()
    {
        if (onWall && raycastDetection.InShadow == false)
        {
            rb.AddForce(Vector3.down);
            changeRotateDir = true;
            direction = Vector3.up;
            onWall = false;
            rotateToZero = true;
            RotateDirection();
        }

        if (!onWall && rotateToZero)
        {
            rotZeroTimer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotZeroTimer * rotSpeed);
        }

        if (rotZeroTimer > 1)
        {
            rotateToZero = false;
            transform.rotation = targetRot;
            changeRotateDir = true;
            rotZeroTimer = 0;
        }
    }

    private void FallOffWall()
    {
        if (onWall && fallOff)
        {
            rb.AddForce(Vector3.down);
            changeRotateDir = true;
            direction = Vector3.up;
            onWall = false;
            rotateToZero = true;
            RotateDirection();
        }

        if (!onWall && rotateToZero)
        {
            rotZeroTimer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotZeroTimer * rotSpeed);
        }

        if (rotZeroTimer > 1)
        {
            rotateToZero = false;
            transform.rotation = targetRot;
            changeRotateDir = true;
            rotZeroTimer = 0;
        }
    }

    private void RotateDirection()
    {
        if (changeRotateDir)
        {
            //Looking into floor
            if (Vector3.Dot(transform.forward, direction) < -0.95f)
            {
                targetRot = Quaternion.LookRotation(transform.up, direction);
                changeRotateDir = false;
            }
            //Looking Away From floor
            else if (Vector3.Dot(transform.forward, direction) > 0.95f)
            {
                targetRot = Quaternion.LookRotation(-transform.up, direction);
                changeRotateDir = false;
            }
            //Looking Across the floor
            else if (Vector3.Dot(transform.forward, direction) < 0.05f &&
                Vector3.Dot(transform.forward, direction) > -0.05f)
            {
                targetRot = Quaternion.LookRotation(transform.forward, direction);
                changeRotateDir = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Floor")
        {
            onGround = true;
        }
    }
}
