using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [HeaderAttribute("References")]
    private CameraSwitch switcher;
    private Transform cameraTransform;
    private Collider myCollider;
    private Rigidbody rb;
    private Animator myAnimator;

    [HeaderAttribute("Variables")]
    [SerializeField] [Range(0f, 100f)] private float walkSpeed = 7.5f;
    [SerializeField] [Range(0f, 1f)] private float damping = 0.25f;
    [SerializeField] [Range(0f, 10f)] private float lookSpeed = 5f;
    private float inputY;
    private float inputX;
    private Vector3 moveDirection;
    private Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    private Vector3 direction;
    private Vector3 oldInput = Vector3.zero;
    private Vector3 oldMoveAmount;
    private bool cameraChange = false;

    [SerializeField] private const float maxHealth = 10f;
    [SerializeField] [Range(0f, maxHealth)] private float health = maxHealth;
    [SerializeField] private bool shadowed = false;
    [SerializeField] private bool hasWater = false;

    // Start is called before the first frame update
    void Start()
    {
        switcher = (CameraSwitch)FindObjectOfType(typeof(CameraSwitch));

        if (switcher == null)
        {
            print("Il manque un CameraSwitch sur la scène");
        }

        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
        myCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        MovePlayer();
        ShadowBehaviour();
        UpdateAnimator();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "CameraCollider":

                NewCameraUpdate(other.gameObject);

                break;

            case "ShadowCollider":

                if (!shadowed)
                {
                    shadowed = true;
                }

                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "ShadowCollider":

                if (!Physics.Raycast(myCollider.bounds.center, -transform.up, myCollider.bounds.extents.magnitude, LayerMask.GetMask("Shadows")))
                {
                    if (shadowed)
                    {
                        shadowed = false;
                    }
                }

                break;
        }
    }

    private void CheckInput ()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && hasWater)
        {
            UseWater();
        }
    }

    private void MovePlayer()
    {
        // On modifie les inputs en fonction de la caméra
        Vector3 cameraRight = cameraTransform.right;
        Vector3 cameraFront = cameraTransform.forward;
        cameraRight.y = 0;
        cameraFront.y = 0;
        cameraRight.Normalize();
        cameraFront.Normalize();
        moveDirection = cameraRight * inputX + cameraFront * inputY;

        // Ce que le joueur doit parcourir
        Vector3 targetMoveAmount = moveDirection.normalized * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, damping);

        if (!cameraChange)
        {
            // Rotation
            if (inputX != 0 || inputY != 0)
            {
                float newRot = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(newRot, Vector3.up), Time.deltaTime * lookSpeed);
            }

            // Déplacement
            Vector3 nextPosition = rb.position + moveAmount * Time.deltaTime;
            rb.MovePosition(nextPosition);
        }
        // Changement de caméra
        else
        {
            // si le joueur change d'input, le déplacement est réinitialisé
            if (hasInputChanged())
            {
                cameraChange = false;
                oldInput = Vector3.zero;
            }
            // le joueur continue de se déplacer dans la direction précédente
            else
            {
                rb.MovePosition(rb.position + oldMoveAmount);
            }
        }
    }

    private void UpdateAnimator()
    {
        float magnitude = moveAmount.magnitude / walkSpeed;
        myAnimator.SetFloat("speed", magnitude);
    }

    private void NewCameraUpdate(GameObject nextCamera)
    {
        Cinemachine.CinemachineVirtualCamera newCamera = nextCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        switcher.ChangeCamera(newCamera);

        // On change le oldInput seulement si le joueur a changé de bouton
        if (hasInputChanged())
        {
            Vector3 newInput = new Vector3(inputX, 0f, inputY);
            oldInput = newInput;
            oldMoveAmount = moveAmount * Time.deltaTime;
        }

        cameraChange = true;
    }

    private bool hasInputChanged()
    {
        Vector3 currentInput = new Vector3(inputX, 0f, inputY);

        if (oldInput != currentInput)
        {
            return true;
        }

        return false;
    }

    private void ShadowBehaviour()
    {
        if (!shadowed)
        {
            health -= Time.deltaTime;
        }
        else
        {
            health += Time.deltaTime;
        }

        health = Mathf.Clamp(health, 0, maxHealth);
    }

    private void UseWater()
    {
        health += maxHealth / 2;
        hasWater = false;
    }
}