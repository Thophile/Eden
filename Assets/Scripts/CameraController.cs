using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static GameObject camObject;
    public static bool isActivated = false;
    // Parameters
    public LayerMask layerMask;
    static float speed = 15.0f;
    static float sensitivity = 0.8f;

    // Zoom level
    public static float zoomLevel = 4;
    const float minZoom = 0;
    const float maxZoom = 9;
    
    // Rotation
    const float rotStep = 2f;
    const float minRot = 15f;

    // Translation
    const float heightStep = 0.5f;
    const float minHeight = 1f;

    // Field of view
    const float fovStep = 5f;
    const float minFov = 20f;

    // Camera depth
    const float depthStep = .5f;
    const float minDepth = 1f;

    public float hoverHeight;

    void Awake(){
        camObject = Camera.main.gameObject;
    }

    void Update()
    {
        if(isActivated){
            // Scroll action
            zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * 10f;
            zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);

            // Camera  X rotation
            var xRotation =  minRot + zoomLevel * rotStep;
            camObject.transform.Rotate(xRotation - camObject.transform.rotation.eulerAngles.x, 0, 0);

            // Camera Fov
            var fov = minFov + zoomLevel * fovStep;;
            Camera.main.fieldOfView = fov;

            // Camera Depth
            camObject.transform.position = transform.position - transform.forward * (minDepth + depthStep * zoomLevel);


            // Camera Y goal
            var old = hoverHeight;
            hoverHeight = minHeight + zoomLevel * heightStep;
            float translationY = 0;
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast (transform.position, -Vector3.up, out hit,Mathf.Infinity , layerMask)) {
                translationY = hoverHeight - hit.distance;
            }else{
                translationY = hoverHeight - old;
            }
            
            // Camera X & Z translations
            float translationX = Input.GetAxis("Horizontal") * speed * (zoomLevel + 1) * Time.deltaTime;
            float translationZ = Input.GetAxis("Vertical") * speed * (zoomLevel + 1) * Time.deltaTime;

            // Camera Y Rotation
            float yRotation = Input.GetAxis("Rotation") * sensitivity;

            transform.Translate(translationX, translationY, translationZ);
            transform.Rotate(0, yRotation, 0);
        }
    }
}
