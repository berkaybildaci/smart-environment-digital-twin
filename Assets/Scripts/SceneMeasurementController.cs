using UnityEngine;

public class SceneMeasurementController : MonoBehaviour
{
    [SerializeField] private Transform agentTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // agentTransform = GameObject.FindGameObjectWithTag("Agent").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        if(agentTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, agentTransform.position);
        }
    }
}
