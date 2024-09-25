using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class CubeAgent : Agent
{
    [SerializeField] private float _moveSpeedForward;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Transform[] _target;

    [SerializeField] private ShuffleScript _suffleScript;
    private Rigidbody rb;
    // private bool isFullyInside = false;

    // Reference to the two ray sensor objects (Forward and Backward)
    // [SerializeField] private RayPerceptionSensorComponent3D _forwardRaySensor;
    // [SerializeField] private RayPerceptionSensorComponent3D _backwardRaySensor;

    public override void Initialize(){
        Debug.Log("** start episode **");
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin(){
        // isFullyInside = false; 
        _suffleScript.ShuffleParkingSpots();
        transform.localPosition = new Vector3(-3f, 0.7f, -23f);
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(new Vector2(transform.localPosition.x, transform.localPosition.z));
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(new Vector2(_target[0].localPosition.x, _target[0].localPosition.z));
        sensor.AddObservation(new Vector2(_target[1].localPosition.x, _target[1].localPosition.z));

        //  // Perceive forward rays
    //     RayPerceptionInput forwardInput = _forwardRaySensor .GetRayPerceptionInput();
    //     RayPerceptionOutput forwardOutput = RayPerceptionSensor.Perceive(forwardInput);

    //     // // Process the forward ray outputs
    //     foreach (var result in forwardOutput.RayOutputs)
    //     {
    //         ProcessRayHit(result);
    //     }

    //     // Perceive backward rays
    //      RayPerceptionInput backwardInput = _backwardRaySensor.GetRayPerceptionInput();
    //      RayPerceptionOutput backwardOutput = RayPerceptionSensor.Perceive(backwardInput);

    //      // Process the backward ray outputs
    //      foreach (var result in backwardOutput.RayOutputs)
    //      {
    //          ProcessRayHit(result);
    //      }
    // }
    //  private void ProcessRayHit(RayPerceptionOutput.RayOutput result)
    //  {
    //      if (result.HitTaggedObject && result.HitTagIndex == 0 && result.HitFraction <= 0.001)
    //      {
    //          AddReward(-0.0001f); // Penalize for hitting a wall
    //      }
    //      else if (result.HitTaggedObject && result.HitTagIndex == 1 && result.HitFraction <= 0.001)
    //      {
    //          AddReward(-0.0001f); // Penalize for hitting another car
    //      }
    //      else if (result.HitTaggedObject && result.HitTagIndex == 2 && result.HitFraction <= 0.005)
    //      {
    //          AddReward(0.001f); // Reward for detecting empty parking spot
    //      }
    //      else if (result.HitTaggedObject && result.HitTagIndex == 3 && result.HitFraction <= 0.005)
    //      {
    //          AddReward(0.001f); // Reward for detecting empty parking spot
    //      }
     }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int move = actions.DiscreteActions[0];  // Movement action
        int rotate = actions.DiscreteActions[1]; // Rotation action

        // Execute movement and rotation based on the decision
        if (move == 1)  // Move forward
        {
            rb.MovePosition(transform.position + transform.forward * _moveSpeedForward * Time.deltaTime);
        }
        else if (move == 2)  // Move backward
        {
            rb.MovePosition(transform.position - transform.forward * _moveSpeedForward * Time.deltaTime);
        }

        // Handle rotation
        if (rotate == 1)  // Rotate left
        {
            transform.Rotate(0f, -_rotateSpeed * Time.deltaTime, 0f, Space.Self);
        }
        else if (rotate == 2)  // Rotate right
        {
            transform.Rotate(0f, _rotateSpeed * Time.deltaTime, 0f, Space.Self);
        }

        // Small negative reward to encourage efficiency
        AddReward(-0.00005f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        // Heuristic for movement (0 = no movement, 1 = forward, 2 = backward)
        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActions[0] = 2;
        }
        else
        {
            discreteActions[0] = 0;  // No movement
        }

        // Heuristic for rotation (0 = no rotation, 1 = rotate left, 2 = rotate right)
        if (Input.GetKey(KeyCode.A))
        {
            discreteActions[1] = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActions[1] = 2;
        }
        else
        {
            discreteActions[1] = 0;  
        }
    }

    // Handle collisions and triggers (remains unchanged)
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "EmptySpot"){
            Debug.Log("Car parked successfully ! ");
            AddReward(6f);
            EndEpisode();
        }
        if (other.gameObject.tag == "FullSpot"){
            Debug.Log("Car HIT FullSpot ! ");
            AddReward(0.0005f);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Wall"){
            Debug.Log("hit wall ! ");
            AddReward(-1f);
            EndEpisode();
        }
        if (other.gameObject.tag == "OtherCar"){
            Debug.Log("hit OtherCar ! ");
            AddReward(-1f);
            EndEpisode();
        }
    }
}

    // private void OnTriggerStay(Collider other) {
    //     // Logic for checking if the agent is fully inside the parking spot
    //     if (other.gameObject.tag == "EmptySpot") {
    //         Bounds carBounds = GetComponent<Collider>().bounds;
    //         Bounds spotBounds = other.bounds;
    //         float tolerance = 0.5f;

    //         bool isInsideX = carBounds.min.x >= spotBounds.min.x - tolerance && carBounds.max.x <= spotBounds.max.x + tolerance;
    //         bool isInsideZ = carBounds.min.z >= spotBounds.min.z - tolerance && carBounds.max.z <= spotBounds.max.z + tolerance;

    //         if (isInsideX && isInsideZ) {
    //             if (!isFullyInside) {
    //                 isFullyInside = true;
    //                 Debug.Log("Car parked successfully ! ");
    //                 AddReward(6f);
    //                 EndEpisode(); // End the episode when fully parked
    //             }
    //         }
    //     }
//     // }
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.MLAgents;
// using Unity.MLAgents.Sensors;
// using Unity.MLAgents.Actuators;


// public class CarMove : Agent
// {
//     public Transform[] parkingSpots; // Les places de parking dans le parking (y compris celles avec le tag "EmptySpot")
//     public float moveSpeed = 5f;
//     public float turnSpeed = 200f;
//     private Transform targetSpot;  // La place de parking vide vers laquelle la voiture va se déplacer

//     public override void OnEpisodeBegin()
//     {
//         // Réinitialisation de la position de la voiture
//         transform.localPosition = new Vector3(0, 0.5f, -10);

//         // Sélectionner une place de parking vide aléatoirement
//         targetSpot = GetRandomEmptyParkingSpot();
//     }

//     public override void CollectObservations(VectorSensor sensor)
//     {
//         // Observation de la position de la voiture par rapport à la place de parking cible
//         sensor.AddObservation(Vector3.Distance(transform.localPosition, targetSpot.localPosition));

//         // Direction relative entre la voiture et la place de parking cible
//         Vector3 directionToSpot = targetSpot.localPosition - transform.localPosition;
//         sensor.AddObservation(transform.InverseTransformDirection(directionToSpot));
//     }

//     public override void OnActionReceived(ActionBuffers actions)
//     {
//         float move = actions.ContinuousActions[0]; // Avancer/reculer
//         float turn = actions.ContinuousActions[1]; // Tourner

//         // Déplacement de la voiture
//         transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);
//         transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);
//     }

//     public override void Heuristic(in ActionBuffers actionsOut)
//     {
//         var continuousActions = actionsOut.ContinuousActions;
//         continuousActions[0] = Input.GetAxis("Vertical");
//         continuousActions[1] = Input.GetAxis("Horizontal");
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("EmptySpot"))
//         {
//             SetReward(1.0f);  // Récompense pour s'être garé dans une place vide
//             EndEpisode();  // Terminer l'épisode
//         }
//         else if (other.CompareTag("Obstacle"))
//         {
//             SetReward(-1.0f);  // Pénalité pour avoir heurté un obstacle
//             EndEpisode();
//         }
//     }

//     // Fonction pour obtenir une place de parking vide aléatoire
//     private Transform GetRandomEmptyParkingSpot()
//     {
//         // Trouver toutes les places avec le tag "EmptySpot"
//         GameObject[] emptySpots = GameObject.FindGameObjectsWithTag("EmptySpot");

//         // Sélectionner une place de parking vide aléatoirement parmi les deux disponibles
//         if (emptySpots.Length > 0)
//         {
//             int randomIndex = Random.Range(0, emptySpots.Length);
//             return emptySpots[randomIndex].transform;
//         }

//         return null;  // Si aucune place vide n'est trouvée
//     }
// }
