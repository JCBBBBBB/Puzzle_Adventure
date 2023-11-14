
using puzzle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    private Camera cam;
    private float targetZoom;
    public float zoomFactor = 3f;
    [SerializeField] public float zoomLerpSpeed = 10f;

    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;



    public Quaternion startQuaternion;
    public GameObject Target;

    public float offsetX = 3f;            // 카메라의 x좌표
    public float offsetY = 5f;           // 카메라의 y좌표
    public float offsetZ = 3f;          // 카메라의 z좌표


    public float CameraSpeed = 10.0f;       // 카메라의 속도
    Vector3 TargetPos;                      // 타겟의 위치



    public GameObject player;        //Public variable to store a reference to the player game object


    private Vector3 offset;

    private Vector3 originalPos;





    void Start()
    {
        cam = Camera.main;
        targetZoom = cam.fieldOfView;
        startQuaternion = transform.rotation;
        offset = transform.position - player.transform.position;
        originalPos = new Vector3(Target.transform.position.x, Target.transform.position.y, Target.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Wheel();
        Reset();
        Follow();
        Init();


    }
    void Wheel()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        targetZoom -= scrollData * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, 5f, 60f);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomLerpSpeed);

    }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }
    void LateUpdate()
    {
        // If Control and Alt and Middle button? ZOOM!

        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        if (Input.GetMouseButton(1))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            target.Translate(Vector3.up * -Input.GetAxis("Mouse Y") * panSpeed);
        }

        ////////Orbit Position

     
       
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;

    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }






    void Reset()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            
            Target.transform.position = originalPos;

        }

    }
    void Follow()
    {
        // 타겟의 x, y, z 좌표에 카메라의 좌표를 더하여 카메라의 위치를 결정
        TargetPos = new Vector3(
            Target.transform.position.x + offsetX,
            Target.transform.position.y + offsetY,
            Target.transform.position.z + offsetZ
            );

        // 카메라의 움직임을 부드럽게 하는 함수(Lerp)
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * CameraSpeed);
    }


}
