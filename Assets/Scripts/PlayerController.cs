using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [HeaderAttribute("References")]
    private Transform cameraTransform;
    private Rigidbody rb;

    [HeaderAttribute("Variables")]
    [SerializeField][Range(0f, 100f)] private float walkSpeed;
    [SerializeField][Range(0f, 1f)] private float damping;
    private Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");


        // Déplacement
        Vector3 moveDirection = new Vector3(inputX, 0f, inputY).normalized;
        Vector3 targetMoveAmount = moveDirection * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, damping);

    }

    void FixedUpdate()
    {
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);
    }
}
