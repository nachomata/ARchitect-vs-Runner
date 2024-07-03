using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private DynamicJoystick _joystick;
    [SerializeField] private GameObject _playerModel;
    private Animator _animator;

    [SerializeField] private float _moveSpeed = 5f;
    public bool useGravity = false;
    public GameObject ground;
    public float gravityMultiplier = 2f;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = _playerModel.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        if (useGravity)
            _rigidbody.AddForce(calculateDirectionToGround() * 9.81f * gravityMultiplier, ForceMode.Acceleration);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            _animator.SetBool("Death_b", true);

        }else if (other.CompareTag("Key"))
        {
            Destroy(other.gameObject);
        }
    }

    private void MovePlayer()
    {
        float moveHorizontal = _joystick.Horizontal;
        float moveVertical = _joystick.Vertical;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement = transform.TransformDirection(movement);

        if (movement != Vector3.zero)
        {
            _rigidbody.MovePosition(transform.position + movement * _moveSpeed * Time.deltaTime);
            _playerModel.transform.rotation = Quaternion.LookRotation(movement);
        }
        _animator.SetFloat("Speed_f", movement.magnitude);
    }

    Vector3 calculateDirectionToGround()
    {
        Vector3 toObject = transform.position - ground.transform.position;
        Vector3 projectedToPlane = toObject - Vector3.Dot(toObject, ground.transform.up) * ground.transform.up;
        Vector3 nearestPoint = ground.transform.position + projectedToPlane;
        Vector3 directionToGround = (nearestPoint - transform.position).normalized;

        return directionToGround;
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
}