using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    [SerializeField] private float torque = 1.0f;

    [SerializeField] private float minSpeedBeforeTorque = 0.3f;
    
    [SerializeField] private float minSpeedBeforeIdle = 0.2f;

    private Rigidbody carRigidBody;

    public bool IsAutonomous { get; set; } = false;
    public Direction CurrentDirection { get; set; } = Direction.Idle;

    public enum Direction
    {
        Idle,
        MoveForward,
        MoveBackward,
        TurnLeft,
        TurnRight
    }

    void Awake() 
    {
        carRigidBody = GetComponent<Rigidbody>();
    }

    void Update() 
    {
        if(carRigidBody.velocity.magnitude <= minSpeedBeforeIdle)
        {
            CurrentDirection = Direction.Idle;
        }    
    }
    

    void FixedUpdate() => ApplyMovement();

      public void ApplyMovement()
    {
        if(Input.GetKey(KeyCode.UpArrow) || (CurrentDirection == Direction.MoveForward && IsAutonomous))
        {
            carRigidBody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        }

        if(Input.GetKey(KeyCode.DownArrow) || (CurrentDirection == Direction.MoveBackward && IsAutonomous))
        {
            carRigidBody.AddForce(-transform.forward * speed, ForceMode.VelocityChange);
        }

        if((Input.GetKey(KeyCode.LeftArrow) && canApplyTorque()) || (CurrentDirection == Direction.TurnLeft && IsAutonomous))
        {
            carRigidBody.AddTorque(transform.up * -torque);
        }

        if(Input.GetKey(KeyCode.RightArrow) && canApplyTorque() || (CurrentDirection == Direction.TurnRight && IsAutonomous))
        {
            carRigidBody.AddTorque(transform.up * torque);
        }
    }

    public bool canApplyTorque()
    {
        Vector3 velocity = carRigidBody.velocity;
        return Mathf.Abs(velocity.x) >= minSpeedBeforeTorque || Mathf.Abs(velocity.z) >= minSpeedBeforeTorque;
    }
}