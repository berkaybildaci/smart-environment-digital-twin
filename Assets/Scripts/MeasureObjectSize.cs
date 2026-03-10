using UnityEngine;

public class MeasureObjectSize : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 objectSize = GetComponent<Renderer>().bounds.size * 100f;
        Debug.Log($"Object Name: {gameObject.name} \nObject Size: {objectSize.x}cm x {objectSize.y}cm x {objectSize.z}cm");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
