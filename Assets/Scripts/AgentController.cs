using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AgentController : MonoBehaviour
{
    public List<GameObject> startingPoints = new List<GameObject>();
    public int currentPointIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.position = startingPoints[currentPointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if(currentPointIndex + 1 < startingPoints.Count)
            {
                currentPointIndex++;
                gameObject.transform.position = startingPoints[currentPointIndex].transform.position;
            } else
            {
                currentPointIndex = 0;
                gameObject.transform.position = startingPoints[currentPointIndex].transform.position;
            }
        }
    }
}
