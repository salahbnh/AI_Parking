using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies; 
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using static CarController;

public class CarAgent : Agent
{
    [SerializeField] private ShuffleScript shuffleScript;
    [SerializeField] private Transform[] _target;
    [SerializeField] private float ParkReward = 2f;
    [SerializeField] private float CollisionReward = 0.1f;

    private Vector3 originalPosition;

    private BehaviorParameters behaviorParameters;

    private CarController carController;

    private Rigidbody carControllerRigidBody;

    public override void Initialize()
    {
        originalPosition = transform.localPosition;
        behaviorParameters = GetComponent<BehaviorParameters>();
        carController = GetComponent<CarController>();
        carControllerRigidBody = carController.GetComponent<Rigidbody>();
        shuffleScript.ShuffleParkingSpots();
    }

    public override void OnEpisodeBegin()
    {
        ResetParkingLotArea();
    }

    private void ResetParkingLotArea()
    {
        // important to set car to automonous during default behavior
        carController.IsAutonomous = behaviorParameters.BehaviorType == BehaviorType.Default;
        transform.localPosition = originalPosition;
        transform.localRotation = Quaternion.identity;
        carControllerRigidBody.velocity = Vector3.zero;
        carControllerRigidBody.angularVelocity = Vector3.zero;

        // reset which cars show or not show
        shuffleScript.ShuffleParkingSpots();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);

        sensor.AddObservation(_target[0].localPosition);
        sensor.AddObservation(_target[0].rotation);

        sensor.AddObservation(_target[1].localPosition);
        sensor.AddObservation(_target[1].rotation);

        sensor.AddObservation(carControllerRigidBody.velocity);
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        var direction = Mathf.FloorToInt(actions.DiscreteActions[0]);

        switch (direction)
        {
            case 0: // idle
                carController.CurrentDirection = Direction.Idle;
                break;
            case 1: // forward
                carController.CurrentDirection = Direction.MoveForward;
                break;
            case 2: // backward
                carController.CurrentDirection = Direction.MoveBackward;
                break;
            case 3: // turn left
                carController.CurrentDirection = Direction.TurnLeft;
                break;
            case 4: // turn right
                carController.CurrentDirection = Direction.TurnRight;
                break;
        }

        AddReward(-0.0001f);
    }

    public void GivePoints(float amount = 1.0f, bool isFinal = false)
    {
        AddReward(amount);

        if(isFinal)
        {
            EndEpisode();
        }
    }

    public void TakeAwayPoints(float amount = 0.01f )
    {
        AddReward(-amount);
        
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        if(Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = 1;
        }

        if(Input.GetKey(KeyCode.DownArrow))
        {
            discreteActions[0] = 2;
        }

        if(Input.GetKey(KeyCode.LeftArrow) && carController.canApplyTorque())
        {
            discreteActions[0] = 3;
        }

        if(Input.GetKey(KeyCode.RightArrow) && carController.canApplyTorque())
        {
            discreteActions[0] = 4;
        }
    }


    private void OnTriggerEnter(Collider other) {
    if (other.gameObject.tag == "EmptySpot")
    {
        // float carRotationY = transform.eulerAngles.y; // Car's Y-axis rotation
        // float emptySpotRotationY = other.transform.eulerAngles.y; // Empty spot's Y-axis rotation

        // // Margin between both rotations
        // bool sameRotation = Mathf.Abs(Mathf.DeltaAngle(carRotationY, emptySpotRotationY)) <= 40f;

        // //Opposite Margin between both rotations
        // bool oppositeRotation = Mathf.Abs(Mathf.DeltaAngle(carRotationY, emptySpotRotationY - 180f)) <= 40f;

        // // If the car has the same or opposite rotation 
        // if (sameRotation || oppositeRotation)
        // {
            Debug.Log("Car parked successfully with correct rotation!");
            GivePoints(ParkReward, true);
        // }
        // else
        // {
        //     TakeAwayPoints(CollisionReward/1000);
        //     Debug.Log("Incorrect rotation. Car not parked.");
        // }
    }
        if (other.gameObject.tag == "FullSpot"){
            Debug.Log("Car HIT FullSpot ! ");
            GivePoints(ParkReward /4   ,false);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Wall"){
            Debug.Log("hit wall ! ");
            TakeAwayPoints(CollisionReward);
        }
        if (other.gameObject.tag == "OtherCar"){
            Debug.Log("hit OtherCar ! ");
            TakeAwayPoints(CollisionReward);
        }
    }
}