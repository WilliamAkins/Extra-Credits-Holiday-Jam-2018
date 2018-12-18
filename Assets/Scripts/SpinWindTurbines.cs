using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWindTurbines : MonoBehaviour
{
    private float rotationSpeed = 25.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(6.0f * rotationSpeed * Time.deltaTime, 0, 0);
    }
}
