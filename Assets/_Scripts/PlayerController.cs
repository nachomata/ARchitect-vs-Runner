using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    private Rigidbody _rigidbody;
    [SerializeField] private DynamicJoystick _joystick;
    [SerializeField] private GameObject _playerModel;
    public Animator _animator;
    public Transform cameraTransform;
    public Transform ParentTransform;
    public float speedScale = 1f;
    [SerializeField] private float _moveSpeed = 1.5f;
    public bool useGravity = false;
    bool inair = true;
    public GameObject ground;
    public float gravityMultiplier = 2f;

    public GameObject normalUI, winningUI;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ParentTransform = transform.parent;
        _rigidbody = GetComponent<Rigidbody>();
        _animator = _playerModel.GetComponent<Animator>();
    }

    private void Update()
    {
        MovePlayer();
        if (useGravity)
            _rigidbody.AddForce(calculateDirectionToGround() * 9.81f * gravityMultiplier, ForceMode.Acceleration);

        if (transform.position.y < -10)
        {
            _animator.SetBool("Death_b", true);
            _animator.SetBool("Death_b", false);
            if(ActionManager.Instance != null)
                StartCoroutine(ActionManager.Instance.OnDamage());
        }

    }

    void OncollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("Ground"))
        {
            print("luodi");
            inair = false;
        }
    }

    void OncollisionStay(Collision collision){
        if(collision.gameObject.CompareTag("Ground"))
        {
            print("luodi");
            inair = false;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            inair = true;
        }

    }

    private void setDeathToFalse()
    {
        _animator.SetBool("Death_b", false);
    }

    public GameObject joystick;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Invoke("setDeathToFalse", 1.5f);
            _animator.SetBool("Death_b", true);
            _animator.SetBool("Death_b", false);
            if(ActionManager.Instance != null)
                StartCoroutine(ActionManager.Instance.OnDamage());
        }else if (other.CompareTag("Key"))
        {
            Destroy(other.gameObject);
        }else if (other.CompareTag("Winning"))
        {
            normalUI.SetActive(false);
            winningUI.SetActive(true);
            joystick.SetActive(false);
        }
    }

    private void MovePlayer()
    {
        float moveHorizontal = _joystick.Horizontal;
        float moveVertical = _joystick.Vertical;
        Vector3 inputDirection = new Vector3(moveHorizontal, 0.0f, moveVertical);
        
        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 cameraForward = ParentTransform.InverseTransformDirection(cameraTransform.forward);
            Vector3 cameraRight = ParentTransform.InverseTransformDirection(cameraTransform.right);

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = inputDirection.z * cameraForward + inputDirection.x * cameraRight;
            moveDirection.Normalize();

            Vector3 localMoveDir = moveDirection * inputDirection.magnitude * _moveSpeed * speedScale * Time.deltaTime;
            transform.localPosition += localMoveDir;

            float targetAngle = Mathf.Atan2(localMoveDir.x, localMoveDir.z) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0, targetAngle, 0);
        }

        else
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        _animator.SetFloat("Speed_f", inputDirection.magnitude);
    }

    Vector3 calculateDirectionToGround()
    {
        // Vector3 toObject = transform.position - ground.transform.position;
        // Vector3 projectedToPlane = toObject - Vector3.Dot(toObject, ground.transform.up) * ground.transform.up;
        // Vector3 nearestPoint = ground.transform.position + projectedToPlane;
        // Vector3 directionToGround = (nearestPoint - transform.position).normalized;

        // return directionToGround;
        return -transform.parent.up;
    }

    public void enableGravity()
    {
        Debug.Log("Gravity enabled");
        useGravity = true;
    }

    public void disableGravity()
    {
        Debug.Log("Gravity disabled");
        useGravity = false;
    }

    public void SetPlayerPosition(Vector3 newPosition)
    {
        // Vector3 pos = ParentTransform.InverseTransformPoint(newPosition);
        // transform.localPosition = ParentTransform.InverseTransformPoint(pos);
        
        transform.position = newPosition;
    }
}