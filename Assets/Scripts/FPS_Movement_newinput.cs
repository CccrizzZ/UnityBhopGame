using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class FPS_Movement_newinput : MonoBehaviour
{

    public float WalkSpeed;
    public float RunSpeed;
    public float JumpForce;    
    public bool isRunning;
    public bool isJumping;
    public bool isGrounded;
    public bool canMove;
    Rigidbody rbRef;
    Vector2 InputVector;
    Vector3 MoveDirection;
    float CurrentSpeed;
    public float Health;
    public Text HealthIndicator;

    void Start()
    {
        rbRef = GetComponent<Rigidbody>();
        
        HealthIndicator.GetComponent<Text>().text = Health.ToString();
        
        isJumping = false;
        isRunning = false;
        canMove = true;
    }


    void Update()
    {
        if (transform.position.y < -15)
        {
            respwawn();
            return;
        }

        if(canMove)
        {

            // determine player direction
            MoveDirection = transform.forward * InputVector.y + transform.right * InputVector.x;


            if(!isGrounded)
            {
                rbRef.velocity += MoveDirection / 20;
            }
    
    
            // if is jumping dont move
            if(isJumping) return;
            // if no input dont move
            if(InputVector.magnitude <= 0) return;


            // determine walking or running 
            if (isRunning)
            {
                CurrentSpeed = RunSpeed;
            }
            else
            {
                CurrentSpeed = WalkSpeed;
            }





            // make movement vector
            Vector3 movement = MoveDirection * (CurrentSpeed * Time.deltaTime);

            // apply movement
            transform.position += movement;

            // rbRef.MovePosition(transform.position + movement * 10);

        }

    }


    public void OnMove(InputValue value)
    {
        
        // get input vector from input value
        InputVector = value.Get<Vector2>();



        // print(MoveDirection);


    }


    public void OnRun(InputValue input)
    {
        if (input.Get().ToString()=="1")
        {
            isRunning = true;
            // PlayerAnimator.SetBool(RunHash, PController.IsRunning);
        }
        else
        {
            isRunning = false;
            // PlayerAnimator.SetBool(RunHash, PController.IsRunning);
        }
    }


    public void OnJump(InputValue input)
    {
        if(!isJumping && isGrounded)
        {
            // reset velocity
            rbRef.velocity = Vector3.zero;

            // set jump bool
            isJumping = input.isPressed;
            // PlayerAnimator.SetBool(JumpHash, input.isPressed);

            // add force to rigidbody
            rbRef.AddForce((transform.up + MoveDirection + MoveDirection) * JumpForce , ForceMode.Impulse);
            
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        // respawn if fall into the void
        if (other.gameObject.CompareTag("Trigger"))
        {
            respwawn();
            return;
        }   


        if(other.gameObject.CompareTag("Ground")||other.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
            rbRef.velocity = Vector3.zero;
        }

        // do nothing if not jumping
        if (!other.gameObject.CompareTag("Ground") && !isJumping) return;


        // stop jumping if jumping 
        isJumping = false;
        // PlayerAnimator.SetBool(JumpHash, false);

        Debug.Log(other.gameObject.tag);

        if(!isGrounded && other.gameObject.CompareTag("GroundIndicator"))
        {
            canMove = false;

        }
    }



    void respwawn()
    {
        if (Health > 0)
        {
            // reduce health
            Health -= 20f;
            HealthIndicator.GetComponent<Text>().text = Health.ToString();
            
        }
        else
        {
            // Restart
        }

        // reset all platform material
        var tempArray = GameObject.FindGameObjectsWithTag("Platform");
        foreach (var platform in tempArray)
        {
            platform.GetComponent<StaticPlatformScript>().ResetColor();
        }

        // reset rigidbody
        rbRef.velocity = Vector3.zero;
        rbRef.angularVelocity = Vector3.zero;


        // reset player transform
        GetComponent<FPS_Camera_newinput>().ResetCameraRot();
        transform.position = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
        transform.rotation = Quaternion.Euler(0,0,0);
    }



}