using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CameraFPV : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Camera camera;
    
    [Header("Parameters")]
    [SerializeField] private float xMax;
    [SerializeField] private float xMin;
    [Range(1.0f,10.0f)]
    [SerializeField] private float xSensitivity; 
    [Range(1.0f,10.0f)]
    [SerializeField] private float ySensitivity;
    
    private float _yRot, _xRot;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<Camera>();
        var camRot = camera.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //Input
        _xRot += Input.GetAxis("Mouse Y")*xSensitivity;
        _yRot += Input.GetAxis("Mouse X")*ySensitivity;
        //Clamp
        _xRot = Mathf.Clamp(_xRot, xMin, xMax);
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        transform.parent.rotation = Quaternion.Euler(0, _yRot, 0); // rotate body
        camera.transform.rotation = Quaternion.Euler(-_xRot,_yRot,0); // rotate camera
    }
}
