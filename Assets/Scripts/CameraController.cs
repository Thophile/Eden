using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static GameObject camera;

    static float speed = 15.0f;
    static float sensitivity = 0.2f;

    // Zoom level
    static float zoomLevel = 6;
    const float minZoom = 0;
    const float maxZoom = 10;

    // Rotation
    const float rotStep = 5f;
    const float minRot = 10f;

    // Translation
    const float heightStep = 5f;
    const float minHeight = 5f;

    // Field of view
    const float fovStep = 10f;
    const float minFov = 20f;

    static float hoverHeight;

    void Start(){
        camera = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        // Scroll action
        zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * 10f;
        zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);

        // Camera  X rotation
        var xRotation =  minRot + zoomLevel * rotStep;
        camera.transform.Rotate(xRotation - camera.transform.rotation.eulerAngles.x, 0, 0);

        // Camera Fov
        var fov = minFov + zoomLevel * fovStep;;
        Camera.main.fieldOfView = fov;

        // Camera Y goal
        hoverHeight = minHeight + zoomLevel * heightStep;
        float translationY = 0;
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast (transform.position, -Vector3.up, out hit)) {
            translationY = hoverHeight - hit.distance;
        }
        
        // Camera X & Z translations
        float translationX = Input.GetAxis("Horizontal") * speed * (zoomLevel + 1) * Time.deltaTime;
        float translationZ = Input.GetAxis("Vertical") * speed * (zoomLevel + 1) * Time.deltaTime;

        // Camera Y Rotation
        float yRotation = Input.GetAxis("Rotation") * sensitivity * (zoomLevel + 1);

        transform.Translate(translationX, translationY, translationZ);
        transform.Rotate(0, yRotation, 0);
        
    }
}
