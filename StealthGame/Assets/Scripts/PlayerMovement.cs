using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    #region Inspector vars
    [Tooltip("Speed of player movement")]
    [SerializeField]
    private float walkSpeed = 3;
    [Tooltip("Size of rays that detect walls")]
    [SerializeField]
    private float wallDetection = 0.5f;
    [Tooltip("Rotation speed of player")]
    [SerializeField]
    private float rotSpeed = 1.0f;
    [Tooltip("The max time you remain static when you reattach")]
    [SerializeField]
    private float attachRefresh = 0.3f;
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
    private float attachTimer;
    private bool downDetect = false;
    private bool startAttachTimer = false;
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
        //Obtain components
        rb = GetComponent<Rigidbody>();
        playerRotation = GetComponent<PlayerRotation>();
        raycastDetection = GetComponent<RaycastDetection>();
    }

    void Update()
    {
     

        #region Small if checks
        if (onWall)
        {
            //Do not use gravity
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            onGround = false;
        }
        else
        {
            rb.useGravity = true;
        }

        if (controlStaticTimer > 1.2f)
        {
            //Restore movement
            canMove = true;
            playerRotation.enabled = true;
        }

        if (fallOff)
        {
            rb.AddForce(Vector3.down);
        }

        if(startAttachTimer)
        {
            attachTimer += Time.deltaTime;
        }

        if(attachTimer > attachRefresh)
        {
            downDetect = false;
            startAttachTimer = false;
            attachTimer = 0.0f;
        }

        #endregion

        //Raycasting to detect wall
        DetectWalls();

        if (canMove)
        {
            //Reset static timer
            controlStaticTimer = 0.0f;

            //Calculate movement based on controller input normalised
            inputX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxisRaw("Vertical");

            Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
            Vector3 targetMoveAmount = moveDir * walkSpeed;
            moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
        }
        else
        {
            //Reset values and start timer
            inputX = 0.0f;
            inputY = 0.0f;
            moveAmount = Vector3.zero;
            controlStaticTimer += Time.deltaTime;
        }

        #region Old Camera rotation 
        ////Look rotation (option for controller or mouse)
        //transform.Rotate(Vector3.up * Input.GetAxis("RHorizontal") * 5);
        //verticalLookRotation += Input.GetAxis("RVertical") * 5;
        //
        //verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        //cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
        #endregion
    }

    private void LateUpdate()
    {
        //Rotation across walls
        RotatePlayer();

        //Rotate when you're back on the ground
        RotateBack();

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
                onGround = false;
            }

            //Up
            if (Physics.Raycast(transform.position, transform.up, out objectHit, 0.5f))
            {
                if(!onWall)
                {
                    Debug.Log("Up Raycast on: " + objectHit.collider);
                    Debug.DrawRay(transform.position, transform.forward, Color.blue);
                    canRotate = true;

                    //If hit wall or floor, change direction to the normal of the hit object
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
            }

            //Forward
            else if (Physics.Raycast(transform.position, transform.forward, out objectHit, wallDetection))
            {
                Debug.Log("Forward Raycast on: " + objectHit.collider);
                Debug.DrawRay(transform.position, transform.forward, Color.blue);
                canRotate = true;

                //If hit wall or floor, change direction to the normal of the hit object
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

                //If hit wall or floor, change direction to the normal of the hit object
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

                //If hit wall or floor, change direction to the normal of the hit object
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

                //If hit wall or floor, change direction to the normal of the hit object
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
            //Down
            else if (Physics.Raycast(transform.position, -transform.up, out objectHit, 0.5f) && !onWall && !onGround) 
            {
                Debug.Log("Downwards Raycast on: " + objectHit.collider);
                Debug.DrawRay(transform.position, transform.forward, Color.blue);
                

                //If hit wall or floor, change direction to the normal of the hit object
                if (objectHit.transform.tag == "Wall")
                {
                    downDetect = true;
                    detectFloor = false;
                }
                else if (objectHit.transform.tag == "Floor")
                {
                    downDetect = false;
                    detectFloor = true;
                }
            }
            else
            {
                //While rotating
                direction = Vector3.zero;
                canRotate = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (onGround)
        {
            //Reset values
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            fallOff = false;
            downDetect = false;
        }

        if (canMove)
        {
            //Apply movement to rigidbody
            localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + localMove);
        }
        else
        {
            //Reset values
            localMove = Vector3.zero;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void RotatePlayer()
    {

        //Mounting wall from down raycast
        if (Input.GetButtonDown("Fire1") && !onWall && downDetect && raycastDetection.InShadow == true)
        {
            onWall = true;
            fallOff = false;
            startAttachTimer = true;
            rotateToZero = false;
            changeRotateDir = true;
        }
        //Leaving Wall
        else if (Input.GetButtonDown("Fire1") && direction != Vector3.zero && detectFloor == true)
        {
            onWall = false;
            canMove = false;
            playerRotation.enabled = false;
            rotTimer = 0.0f;
            oldRot = transform.rotation;

            RotateDirection();

            rotate = true;
            direction = Vector3.zero;
            
        }
        //Mounting wall
        else if (Input.GetButtonDown("Fire1") && direction != Vector3.zero && raycastDetection.InShadow == true)
        {
            fallOff = false;
            canMove = false;
            onWall = true;
            playerRotation.enabled = false;
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

            if (transform.rotation != targetRot) /* Does not equal our target rotation */
            {
                //Slerp towards rotation
                transform.rotation = Quaternion.Slerp(oldRot, targetRot, rotTimer * rotSpeed);
            }
        }

        if (rotTimer > 1)
        {
            //Reset
            rotate = false;
            transform.rotation = targetRot;
            changeRotateDir = true;
            rotTimer = 0.0f;
        }
    }

    private void DetachOffWall()
    {
        if (onWall && !canRotate && !downDetect)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //Player falls and rotates back to land on ground
                rb.AddForce(Vector3.down);
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, 1.0f);
                changeRotateDir = true;
                direction = Vector3.up;
                onWall = false;
                rotateToZero = true;
                RotateDirection();
            }
        }
    }

    private void FallOutOfShadow()
    {
        if (onWall && raycastDetection.InShadow == false)
        {
            //Player falls and rotates back to land on ground
            rb.AddForce(Vector3.down);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 1.0f);
            changeRotateDir = true;
            direction = Vector3.up;
            onWall = false;
            rotateToZero = true;
            RotateDirection();
        }
    }

    private void FallOffWall()
    {
        if (onWall && fallOff)
        {
            //Player falls and rotates back to land on ground
            rb.AddForce(Vector3.down);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 1.0f);
            changeRotateDir = true;
            direction = Vector3.up;
            onWall = false;
            rotateToZero = true;
            RotateDirection();
        }
    }

    private void RotateBack()
    {
        if (!onWall && rotateToZero && onGround)
        {
            //Slerp to pos
            rotZeroTimer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotZeroTimer * rotSpeed);
        }

        if (rotZeroTimer > 1)
        {
            //Reset
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
