using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    //[SerializeField]
    //float InwardsScrollMax = 10.0f;

    //[SerializeField]
    //float OutwardsScrollMax = 50.0f;

    //[SerializeField]
    //float ScrollSensitivity = 5.0f;

    //[SerializeField]
    //float MouseSensitivity = 5.0f;

    [SerializeField]
    Camera camera;

    //Vector3 ScrollVector;
    Vector3 pivotPoint;
    ManageMainInfo manageMainInfo;
    //Vector3 Vel = Vector3.zero;
    //Vector3 Pan = new Vector3(1, 1, 0);

    //public float smoothTime = 0.3f;
    //float CameraVel = 0;
    //float PanVel = 0;
    //bool Moving = false;
    //float DistPPCam = 70.0f;

    float ScrollZoom;
    float SmoothScroll;
    float MouseY;
    float MouseX;
    float SmoothX;
    float SmoothY;

    float MiddleMiddleX = 0.0f;
    float MiddleMiddleY = 0.0f;


    //Vector3 ScreenRotationTarget;

    [SerializeField]
    float MouseSensitivityPan = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        manageMainInfo = GameObject.Find("EventManager").GetComponent<ManageMainInfo>();
        GameObject[] tiles = GameObject.Find("EventManager").GetComponent<GenerateTiles>().getTiles();
        GameObject middleTile = tiles[(int)Mathf.Ceil(((tiles.Length - 1) * 0.5f))];
        pivotPoint = new Vector3(middleTile.transform.position.x, middleTile.transform.position.y, middleTile.transform.position.z);
        MiddleMiddleX = pivotPoint.x;
        MiddleMiddleY = pivotPoint.y;

        Debug.Log(pivotPoint);
        //transform.LookAt(pivotPoint);
    }

    // Update is called once per frame
    void Update()
    {
        /*
         *  Scroll movement for the camera. Scroll Sensitivity is modified in the inspector.
         */
        if (manageMainInfo.getGamePaused() == false)
        {
            ScrollZoom += Input.GetAxis("Mouse ScrollWheel") * 20;
            ScrollZoom = Mathf.Clamp(ScrollZoom, -40, 14);

            SmoothScroll = Mathf.Lerp(SmoothScroll, ScrollZoom, Time.deltaTime * 5);

            //ScrollVector = Vector3.forward * (Input.GetAxis("Mouse ScrollWheel") * ScrollSensitivity) * Time.deltaTime;

            //if (transform.position.y - ScrollVector.z > OutwardsScrollMax)
            //    ScrollVector.z = -(OutwardsScrollMax - transform.position.y);
            //else if (transform.position.y - ScrollVector.z < InwardsScrollMax)
            //    ScrollVector.z = -(InwardsScrollMax - transform.position.y);
            //if (ScrollVector != new Vector3 (0,0,0))
            //{
            //    transform.position += ScrollVector;
            //    transform.Translate(ScrollVector);
            //}


            /*
             * Nah we're always gonna be using left ;)
             */


            if (Input.GetButton("Left Mouse Down"))
            {
                SmoothX = Input.GetAxis("Mouse X") * 5;
                SmoothY = Input.GetAxis("Mouse Y") * 5;

                MouseX += SmoothX;
                MouseY += SmoothY;

                if (SmoothX > 0.5f || SmoothX < -0.5f || SmoothY > 0.5f || SmoothY < -0.5f)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
            }
            else
            {
                SmoothX *= 0.9f;
                SmoothY *= 0.9f;

                MouseX += SmoothX;
                MouseY += SmoothY;
            }
            MouseY = Mathf.Clamp(MouseY, 5, 60);

            transform.Translate(20, 0, 20);
            transform.position = Quaternion.Euler(MouseY, MouseX, 0) * new Vector3((pivotPoint.x + 0), (pivotPoint.y + 4), (pivotPoint.z + (-50 + SmoothScroll)));
            transform.rotation = Quaternion.LookRotation(Vector3.up * 4 - transform.position);

            //{
            //    CameraVel = Input.GetAxis("Mouse X") * MouseSensitivity;
            //    Moving = true;
            //}
            //transform.RotateAround(pivotPoint, Vector3.up, CameraVel);

            //if (Moving)
            //{
            //    if (CameraVel > 0.2 || CameraVel < -0.2)
            //    {
            //        Cursor.lockState = CursorLockMode.Confined;
            //        Cursor.visible = false;
            //    }


            //    if (CameraVel > 0.001)
            //        CameraVel -= Time.deltaTime;
            //    else if (CameraVel < -0.001)
            //        CameraVel += Time.deltaTime;
            //    else
            //        Moving = false;

            //    if (CameraVel > 1)
            //        CameraVel -= (Time.deltaTime * 30.0f);

            //    else if (CameraVel < -1)
            //        CameraVel += (Time.deltaTime * 30.0f);
            //}

            /*
             *  Horizontal movement using A and D keys.
             */

            //float input = Input.GetAxisRaw("Horizontal");
            //if (input != 0)
            //{
            //    transform.RotateAround(pivotPoint, Vector3.up, -input);
            //}

            ///*
            // *  Pan movement for the camera using Left Mouse Button.
            // */

            //if (Input.GetButton("Left Mouse Down"))
            //{
            //    PanVel = Input.GetAxis("Mouse Y") * MouseSensitivityPan;
            //    if (transform.rotation.eulerAngles.x < 7.0f)
            //    {
            //        transform.Rotate(Time.deltaTime * 7, 0, 0);
            //        transform.Translate(0, 0, 0);
            //    }
            //    else if (transform.rotation.eulerAngles.x > 60.0f)
            //    {
            //        transform.Rotate(-Time.deltaTime * 7, 0, 0);
            //        transform.Translate(0, 0, 0);
            //    }
            //    else
            //    {
            //        transform.Rotate(PanVel, 0, 0);
            //        transform.Translate(0, PanVel, 0);
            //    }
            //}

            //float input2 = Input.GetAxisRaw("Vertical");
            //if (transform.rotation.eulerAngles.x < 7.0f)
            //{
            //    transform.Rotate(Time.deltaTime * 7, 0, 0);
            //    transform.Translate(0, 0, 0);


            //}
            //else if (transform.rotation.eulerAngles.x > 60.0f)
            //{
            //    transform.Rotate(-Time.deltaTime * 7, 0, 0);
            //    transform.Translate(0, 0, 0);

            //}
            ////if (input2 != 0)
            //else
            //{
            //    transform.Rotate(input2, 0, 0);
            //    transform.Translate(0, input2, 0);
            //}
            ////Debug.Log(transform.position - pivotPoint);
        }
    }

    private void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Confined && !Input.anyKey)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
}
