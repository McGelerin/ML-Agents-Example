using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [Header("Agents Settings")]
    public float movespeed;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Floor Settings")]
    [SerializeField] private Material winmaterial;
    [SerializeField] private Material losematerial;
    [SerializeField] private MeshRenderer floormeshRenderer;

    private float Targetx, Targetz;
    private float Agentsx, Agentsz;

    public override void OnEpisodeBegin()
    {
        Positions();
        transform.localPosition = new Vector3(Agentsx, 0.4f, Agentsz);
        target.localPosition = new Vector3(Targetx, 0.6f, Targetz);
    }

    private void Positions()
    {
        Targetx = Random.Range(-4.4f, 4.4f);
        Targetz = Random.Range(-4.4f, 4.4f);
        Agentsx = Random.Range(-4.4f, 4.4f);
        Agentsz = Random.Range(-4.4f, 4.4f);

        if (Mathf.Abs(Targetx - Agentsx)<1 || Mathf.Abs(Targetz - Agentsz) < 1)
        {
            Positions();
        }
    }



    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);


    }

    public override void OnActionReceived(ActionBuffers actions)
    {
      float moveX=actions.ContinuousActions[0];
      float movez=actions.ContinuousActions[1];
      transform.localPosition += new Vector3(moveX,0,movez)*Time.deltaTime*movespeed;
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Goal>(out Goal gol))
        {
            SetReward(1f);
            floormeshRenderer.material = winmaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {    
           SetReward(-1f);
            floormeshRenderer.material = losematerial;
            EndEpisode();
        }
    }
}
