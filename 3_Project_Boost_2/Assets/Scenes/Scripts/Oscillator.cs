using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f; 
    [Range(0, 1)] [SerializeField] float movementFactor;    //0 means not moved, 1 means completely moved

    Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }    //to remove period = 0 error 
        float cycle = Time.time / period;
        float tau = Mathf.PI * 2;
        float rawSineWave = Mathf.Sin(cycle * tau);
        movementFactor = rawSineWave / 2 + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
