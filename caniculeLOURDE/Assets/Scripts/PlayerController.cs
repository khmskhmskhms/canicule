using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [HeaderAttribute("References")]
    private CameraSwitch switcher;
    private Transform cameraTransform;
    private Rigidbody rb;
    private Animator myAnimator;

    [HeaderAttribute("Variables")]
    [SerializeField][Range(0f, 100f)] private float walkSpeed;
    [SerializeField][Range(0f, 1f)] private float damping;
    private Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    private Vector3 direction;
    [SerializeField][Range(0f, 10f)] private float lookSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        switcher = (CameraSwitch)FindObjectOfType(typeof(CameraSwitch));

        if (switcher == null)
        {
            print("Il manque un CameraSwitch sur la scène");
        }

        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
        rb = GetComponent<Rigidbody>();
        myAnimator = GetComponent <Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 cameraRight = cameraTransform.right;
        Vector3 cameraFront = cameraTransform.forward;
        cameraRight.y = 0;
        cameraFront.y = 0;
        cameraRight.Normalize();
        cameraFront.Normalize();

        // Rotation
        Vector3 moveDirection = cameraRight * inputX + cameraFront * inputY;

        if (inputX != 0 || inputY !=0)
        {
            float newRot = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(newRot, Vector3.up), Time.deltaTime * lookSpeed);
        }

        // Déplacement
        Vector3 targetMoveAmount = moveDirection.normalized * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, damping);
        Vector3 nextPosition = rb.position + moveAmount * Time.deltaTime;        
        rb.MovePosition(nextPosition);

        UpdateAnimator();
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CameraCollider")
        {
            Cinemachine.CinemachineVirtualCamera newCamera = other.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            switcher.ChangeCamera(newCamera);
        }
    }

    private void UpdateAnimator()
    {
        float magnitude = moveAmount.magnitude / walkSpeed;
        myAnimator.SetFloat("speed", magnitude);
    }
}
