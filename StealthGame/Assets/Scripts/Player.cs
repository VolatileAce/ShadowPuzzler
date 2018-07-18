using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float maxSpeed;
    public float speedMod;
    public float rotateSpeed;

    public float shadowSpeed;
    public float shadowMod;

    public bool canMeld = false;
    
    public static bool isMelded = false;
    private bool showIsMelded;
    public static bool grounded = true;

    private bool canWallWalk;

    public bool isDead;
    public bool isTurning;
    private bool currentShadowState = false;
    private bool previousShadowState = false;
    private enum Forms
    {
        Melded = 0,
        UnMelded = 1,
    }

    private Forms currentForm;

    private XboxController controller;

    Rigidbody rb;

    ParticleSystem ps;

    private float moveDirection;
    private float lookDirection;

    private Vector3 leftStickDirection;
    private Vector3 rightStickDirection;

    public Mesh Unmelded;
    public Mesh Melded;

    private Mesh myMesh;

    public GameObject leftCheck;
    public GameObject rightCheck;
    public GameObject backCheck;
    public GameObject frontCheck;
    public GameObject topCheck;

    public GameObject particles;

    public float rotationSpeed;
    private Quaternion desiredRotation;

    public Transform platform001;
    public Transform platform002;

    [Header ("Win/Lose Panels")]
    public GameObject endPanel;
    public Text winLoseText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponentInChildren<ParticleSystem>();
        myMesh = Unmelded;
        currentForm = Forms.UnMelded;
        isDead = false;
        desiredRotation = new Quaternion();
        endPanel.SetActive(false);
        GetComponent<Player>().enabled = true;
        isMelded = false;
        grounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        currentShadowState = canMeld;
        if(isDead)
        {
            Debug.Log("you died");
        }
        GetComponentInChildren<MeshFilter>().mesh = myMesh;

//        Vector3 movement = new Vector3(0, rotateSpeed, 0);

        switch (currentForm)
        {
            case Forms.Melded:
                myMesh = Melded;
                break;

            case Forms.UnMelded:
                myMesh = Unmelded;
                break;
        }

            if (!isMelded)
            {
                rb.useGravity = true;
                Debug.Log("normal, G: " + grounded);
            //forward on the left stick moves player forward
            if (XCI.GetAxis(XboxAxis.LeftStickY, controller) > 0)
            {
                rb.velocity += transform.forward * (moveSpeed / speedMod);
            }
            //baclward on the left stick moves player bakcward
            if (XCI.GetAxis(XboxAxis.LeftStickY, controller) < 0)
            {
                rb.velocity += transform.forward * -(moveSpeed / speedMod);
            }
            //right on the left stick moves player right
            if (XCI.GetAxis(XboxAxis.LeftStickX, controller) > 0)
            {
                rb.velocity += transform.right * (moveSpeed / speedMod);
            }
            //left on the left stick moves player left
            if (XCI.GetAxis(XboxAxis.LeftStickX, controller) < 0)
            {
                rb.velocity += transform.right * -(moveSpeed / speedMod);
            }
        }
            if (isMelded)
            {
                Debug.Log("wall walk, G: " + grounded);
                if (isMelded)
                {
                //forward on the left stick moves player forward
                if (XCI.GetAxis(XboxAxis.LeftStickY, controller) > 0)
                {
                    rb.velocity += transform.forward * (shadowSpeed / speedMod);
                }
                //baclward on the left stick moves player bakcward
                if (XCI.GetAxis(XboxAxis.LeftStickY, controller) < 0)
                {
                    rb.velocity += transform.forward * -(shadowSpeed / speedMod);
                }
                //right on the left stick moves player right
                if (XCI.GetAxis(XboxAxis.LeftStickX, controller) > 0)
                {
                    rb.velocity += transform.right * (shadowSpeed / speedMod);
                }
                //left on the left stick moves player left
                if (XCI.GetAxis(XboxAxis.LeftStickX, controller) < 0)
                {
                    rb.velocity += transform.right * -(shadowSpeed / speedMod);
                }
                rb.useGravity = false;
                    //forward on the left stick moves player up
                    if (XCI.GetButton(XboxButton.LeftBumper, controller))
                    {
                        Debug.Log("up");
                        transform.Translate(Vector3.up * (shadowSpeed / shadowMod));
                    }
                //baclward on the left stick moves player down
                if (XCI.GetButton(XboxButton.RightBumper, controller))
                {
                        transform.Translate(Vector3.down * (shadowSpeed / shadowMod));
                    }
    //                //right on the left stick moves player right
    //                if (XCI.GetAxis(XboxAxis.LeftStickX, controller) > 0)
    //                {
    //                    transform.Translate(Vector3.right * (shadowSpeed / shadowMod));
    //                }
    //                //left on the left stick moves player left
    //                if (XCI.GetAxis(XboxAxis.LeftStickX, controller) < 0)
    //                {
    //                    transform.Translate(Vector3.left * (shadowSpeed / shadowMod));
    //                }
                }
            }

            //right on the right stick rotates camera right
            if (XCI.GetAxis(XboxAxis.RightStickX, controller) > 0 && !isTurning)
            {
         
                //    transform.Rotate(transform.up * rotateSpeed);
                //transform.Rotate(0, 90, 0);
                
                //set desired rotation
                desiredRotation.eulerAngles = transform.eulerAngles + new Vector3(0, 90, 0);
                isTurning = true;
                
            }
            //left on the right stick rotates camera left
            if (XCI.GetAxis(XboxAxis.RightStickX, controller) < 0 && !isTurning)
            {
                //    transform.Rotate(transform.up * -rotateSpeed);
                //transform.Rotate(0, -90, 0);
                
                //set desired rotation
                desiredRotation.eulerAngles = transform.eulerAngles - new Vector3(0, 90, 0);
                isTurning = true;
            }
        
        //check to see if right stick is at 0 to reset isTurning bool
        //if (XCI.GetAxis(XboxAxis.RightStickX, controller).Equals(0))
        //{
        //    //    transform.Rotate(transform.up * -rotateSpeed);
        //    isTurning = false;
        //}
        //press A while in shadows to meld
        if (XCI.GetButtonUp(XboxButton.A) && canMeld == true)
        {
            ChangeForm();
            if (rb.useGravity == false)
            {
                rb.useGravity = true;
            }
        }

        //if turning
        if(isTurning)
        {
            //slerp the rotation to desired rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
            //if the distance from desired to start is minor
            if(Vector3.Distance(transform.rotation.eulerAngles, desiredRotation.eulerAngles) < 1.0f)
            {
                //stop turning
                transform.rotation = desiredRotation;
                isTurning = false;
            }
        }


        //Caps the max velocity
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
        if(!previousShadowState && !currentShadowState && currentForm == Forms.Melded)
        {
            ChangeForm();
        }
        previousShadowState = currentShadowState;
        if (!canMeld)
        {
            ps.Stop();
            rb.useGravity = true;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Shadow")
        {
            canMeld = true;
            Debug.Log("Can Meld");
            ps.Play();

        }
        if (other.tag == "Wall")
        {
            if (isMelded)
            {
                grounded = false;
            }
        }
        if (other.tag == "Net")
        {
            endPanel.SetActive(true);
            GetComponent<Player>().enabled = false;
            winLoseText.text = ("You Got Caught");
        }
        if (other.tag == "WIN")
        {
            endPanel.SetActive(true);
            GetComponent<Player>().enabled = false;
            winLoseText.text = ("You Escaped");
        }
        if (other.tag == "Lock")
        {
            transform.parent = platform001;
        }
        if (other.tag == "Lock2")
        {
            transform.parent = platform002;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Shadow")
        {
            if (currentForm == Forms.Melded && !previousShadowState)
            {
                rb.useGravity = true;
                canMeld = false;
                ChangeForm();
                ps.Stop();
            }
            else
            {
                rb.useGravity = true;
                canMeld = false;
                Debug.Log("Can't Meld");
            }
        }
            if (other.tag == "Wall")
            {
                grounded = true;
            }
        if (other.tag == "Lock")
        {

            transform.parent = null;
        }
        if (other.tag == "Lock2")
        {

            transform.parent = null;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Shadow")
        {
            canMeld = true;
            ps.Play(true);
        }
        
    }
    private void ChangeForm()
    {
        //press A to meld
        //        if (isMelded == false)
        if (currentForm == Forms.UnMelded)
        {
            //            myMesh = Melded;
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            Debug.Log("Melded");
            isMelded = true;
            leftCheck.SetActive(true);
            rightCheck.SetActive(true);
            backCheck.SetActive(true);
            frontCheck.SetActive(true);
            topCheck.SetActive(true);
            currentForm = Forms.Melded;
            return;
        }
        //press A to unmeld
        else if (currentForm == Forms.Melded)
        {
            //            myMesh = Unmelded;
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            Debug.Log("Unmelded");
            isMelded = false;
            leftCheck.SetActive(false);
            rightCheck.SetActive(false);
            backCheck.SetActive(false);
            frontCheck.SetActive(false);
            topCheck.SetActive(false);
            currentForm = Forms.UnMelded;
            return;
        }
    }
}
