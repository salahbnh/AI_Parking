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

    public override void Initialize(){
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        Debug.Log("starting now");
    }
    public override void OnEpisodeBegin(){
        _suffleScript.ShuffleParkingSpots();
        transform.localPosition = new Vector3(-3f, 0.7f,-23f);
        
        GameObject[] emptySpots = GameObject.FindGameObjectsWithTag("EmptySpot");

        _target = new Transform[emptySpots.Length];

        for (int i = 0; i < emptySpots.Length; i++)
        {
            _target[i] = emptySpots[i].transform;
        }    
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_target[0].localPosition);
        sensor.AddObservation(_target[1].localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions){
        float moveForward = actions.ContinuousActions[0];
        float moveRotate = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * _moveSpeedForward * moveForward * Time.deltaTime);
        transform.Rotate(0f, moveRotate *_rotateSpeed, 0f, Space.Self);
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
        AddReward(-0.001f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "EmptySpot"){
            Debug.Log("collision with Sphere");
            AddReward(5f);
            EndEpisode();

        }
        if (other.gameObject.tag  == "Wall"){
            Debug.Log("collision with Wall");
            AddReward(-1f);
            EndEpisode();
        }
        if (other.gameObject.tag  == "OtherCar"){
            Debug.Log("collision with Car");
            AddReward(-1f);
            EndEpisode();
        }
    }
}
