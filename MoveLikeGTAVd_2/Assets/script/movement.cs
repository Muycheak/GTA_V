using System.Collections;
using UnityEngine;

public class movement : MonoBehaviour
{

    #region Variables
    /*[HideInInspector] */ 
    float axisX = 0;
    float axisZ = 0;
    public Animator animator;
    CharacterController c;
    float verticalVelocity;
    public float gravity = 20.0f;
    public float speed = 3.0f;
    float tempSpeed;
    public float decreaseMovement = 2f;
    float tmpDecreaseMovement;
    public float jumpForce = 10.0f;
    bool allowJump = true;

    public float speedRotate = 325f;
    public Transform cameraController;

    Quaternion[] rotateDegree = new Quaternion[8];

    public float mixRunSpeed = 9;
    public float speedChangeAction = 0.8f;
    float limitSpeedChangeAction = 0;

    public float mouseSensitivity = 10f;

    private float xAxisClamp = 0;

    public Transform camera;

    public Transform third_per_cam;

    float mouseX;
    float mouseY;

    private bool first_per = true;

    public GameObject characher_obj;
    #endregion

    private void Start()
    {
        
        //connect character controller with the code
        c = GetComponent<CharacterController>();

        //store decreaseMovement's value in tmpDecreaseMovement's value, because decreaseMovement's value isn't always the same
        tmpDecreaseMovement = decreaseMovement;

        transform.Rotate(0, 0, 0);
        cameraController.Rotate(0, 0, 0);

        // set value to (r0, r45, r90, r135, r180, r225, r270, r315) + transform.eulerAngles.y
        for(int i = 0; i < 8; i++) rotateDegree[i] = Quaternion.Euler(0, i * 45 + transform.eulerAngles.y, 0);

        //store speed's value in tmpSpeed's value, because speed's value isn't always the same
        tempSpeed = speed;

        Cursor.lockState = CursorLockMode.Locked;


    }

    private void FixedUpdate()
    {
        //store aro key's value in axisX and axisZ variable
        axisX = Input.GetAxis("Horizontal");
        axisZ = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        //------------position of camera = position player. Note: cameraController isn't child class of player1------------------------------------------------
        Vector3 copyCameraPosition = new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z); 
        cameraController.position = copyCameraPosition;
        characher_obj.SetActive(!first_per);
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            first_per = true;
            characher_obj.SetActive(false);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3)) {
            first_per = false;
            characher_obj.SetActive(true);
        }

        if(first_per){

        Vector3 copyCamera = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z); 
        camera.position = copyCamera;
        }else {
        Vector3 third_copyCamera = new Vector3(third_per_cam.transform.position.x, third_per_cam.transform.position.y, third_per_cam.transform.position.z); 
        camera.position = third_copyCamera;

        }
        //------------gravity and jump------------------------------------------------
        // jump();

        //------------rotate charactor----------------------------------------
        rotation();

        //-----------rotate camera---------------------------------------------------
        CameraRotation();

        //------------allow animator and move the object in unity------------------------------------------------
        action();

    }

    //------------gravity and jump------------------------------------------------
    void jump()
    {
        if (c.isGrounded)
        {

            //when on the ground the movement doesn't decrease. => axisZ * speed / decreaseMovement = axisZ * speed / 1 = axisZ * speed => move normal
            decreaseMovement = 1.0f;

            //jump per 1.5 second
            if (Input.GetKeyDown(KeyCode.Space) && allowJump)
            {
                //the height of the jump depend on jumpForce's value
                verticalVelocity = jumpForce;

                allowJump = false;

                //1.5 second later allowJump = true
                //note: only this mothod delays, but the rest work instead
                StartCoroutine(DelayJump(1.5f));
            }
        }
        else
        {

            //when not on the ground the movement decreases. => axisZ * speed / decreaseMovement => move slower
            decreaseMovement = tmpDecreaseMovement;

            //gravity formulor: push object to the ground when it is on the air
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }

    //------------rotate camera and charactor----------------------------------------
    void rotation()
    {
        if(axisX == 0 && axisZ != 0) //move forward and backward
        {
            if (axisZ < 0) transform.rotation = Quaternion.Slerp(transform.rotation, rotateDegree[4], 0.075f); //move backward 
            else if (axisZ > 0) transform.rotation = Quaternion.Slerp(transform.rotation, rotateDegree[0], 0.075f); //move forward 
        }
        else if (axisX != 0 && axisZ == 0) // turn right and left
        {
            if(axisX > 0) transform.rotation = Quaternion.Slerp(transform.rotation, rotateDegree[2], 0.075f); //turn right
            else if (axisX < 0) transform.rotation = Quaternion.Slerp(transform.rotation, rotateDegree[6], 0.075f); //turn left
        }
        else if(axisX != 0 && axisZ != 0) //move strafe left(forward, backward) and right(forward, backward)
        {
            if (axisX > 0) //move strafe right(forward, backward)
            {
                if (axisZ > 0) transform.rotation = Quaternion.Slerp(transform.rotation, rotateDegree[1], 0.075f); //move strafe right forward
                else if (axisZ < 0) transform.rotation = Quaternion.Slerp(transform.rotation, rotateDegree[3], 0.075f); //move strafe right backward
            }
            else if(axisX < 0) //move strafe left(forward, backward)
            {
                if (axisZ > 0) transform.rotation = Quaternion.Slerp(transform.rotation, rotateDegree[7], 0.075f); //move strafe left forward
                else if (axisZ < 0) transform.rotation = Quaternion.Slerp(transform.rotation, rotateDegree[5], 0.075f); //move strafe left backward
            }
        }
    }

    //using mouse to look
    void CameraRotation()
    {
        //look up and down by using mouse
        xAxisClamp += mouseY;
        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
        }
        else if (xAxisClamp < -30.0f)
        {
            xAxisClamp = -30.0f;
            mouseY = 0.0f;
        }
        camera.Rotate(Vector3.left * mouseY);


        //look around 360 degree

        cameraController.Rotate(Vector3.up * mouseX);
        // set value to (r0, r45, r90, r135, r180, r225, r270, r315) + transform.eulerAngles.y
        for (int i = 0; i < 8; i++) rotateDegree[i] = Quaternion.Euler(0, i * 45 + cameraController.eulerAngles.y, 0);
    }

    //------------allow animator and move the object in unity. Has controlled idle walk and run animation here------------------------------------------------
    void action()
    {
        // Note : even turn left or turn right or move backward all of them move forward all the same. For example: press 'D' key to turn right => first rotate 90 dergee, and then go forward 

        //set value to animation of player 
        if ((Mathf.Abs(axisZ) == 1 || Mathf.Abs(axisX) == 1) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) //increase speed and enable run animation when press shift and aro key     
        {
            if (speed < mixRunSpeed) speed += speed * Time.deltaTime; // run faster than walk 
            animator.SetFloat("speed", 2, 0.75f, Time.deltaTime); //enable run animation
        }
        else if (Mathf.Abs(axisZ) == 1 || Mathf.Abs(axisX) == 1) // enable walk animation and give value to speed which get from old speed's value
        {
            speed = tempSpeed; // old speed's value
            animator.SetFloat("speed", 1, 0.75f, Time.deltaTime); //enable walk animation
        }
        else if (axisZ == 0 && axisX == 0) // enable walk animation and give value to speed which get from old speed's value
        {
            speed = tempSpeed; // old speed's value
            animator.SetFloat("speed", 0, 0.75f, Time.deltaTime); //enable idle animation
        }
        
        //set value to move the player
        if ((Mathf.Abs(axisZ) == 1 || Mathf.Abs(axisX) == 1) && limitSpeedChangeAction != 1) // start move
        {
            //increase limitSpeedChange's value smoothly from 0 to 1
            limitSpeedChangeAction += speedChangeAction * Time.deltaTime;
            if(limitSpeedChangeAction > 1) limitSpeedChangeAction = 1;
        }
        else if (axisZ == 0 && axisX == 0 && limitSpeedChangeAction != 0) // stop move
        {
            //decrease limitSpeedChange's value smoothly from 1 to 0
            limitSpeedChangeAction -= speedChangeAction * Time.deltaTime;
            if (limitSpeedChangeAction < 0) limitSpeedChangeAction = 0;
        }

        //accept the value of axis y and z
        Vector3 moveVector = transform.TransformDirection(0, verticalVelocity, limitSpeedChangeAction * speed / decreaseMovement);

        //move the player with value what it has set
        c.Move(moveVector * Time.deltaTime);
    }

    float degreeDetect(float degree)
    {
        degree %= 360;
        if (degree < 0) degree = 360 + degree;
        return degree;
    }

    IEnumerator DelayJump(float second) //for delay jump (1 jump per second) 
    {
        //wait ... second
        yield return new WaitForSeconds(second);

        //allow jump after finish waiting
        allowJump = true;
    }

}
