using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class CameraController : MonoBehaviour
    {
        public static GameObject camObject;
        public static bool isActivated = false;
        // Parameters
        public LayerMask layerMask;

        // Zoom level
        public float zoomLevel = 4;
        const float minZoom = 0;
        const float maxZoom = 15;

        // Drag
        private Vector3 dragOriginT;
        private Vector3 dragOriginR;

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
        const float minDepth = 3f;

        float hoverHeight;

        void Awake(){
            camObject = Camera.main.gameObject;
        }

        void Update()
        {
            if(!GameManager.isPaused){
                // Scroll action
                zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * 10f;
                zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);

                RotateCamera();
                MoveCamera();
            }
        }

        void MoveCamera()
        {
            Vector3 move = Vector3.zero;
            if (Input.GetMouseButtonDown(0))
            {
                dragOriginT = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                move = (Input.mousePosition - dragOriginT) * (((zoomLevel * 5f) + Options.cameraSpeed) / 1000f) * -1f;
                dragOriginT = Input.mousePosition;
            }
            transform.Translate(new Vector3(move.x, ZoomCamera(), move.y));
        }

        void RotateCamera()
        {
            Vector3 move = Vector3.zero;
            if (Input.GetMouseButtonDown(2))
            {
                dragOriginR = Input.mousePosition;
            }
            else if (Input.GetMouseButton(2))
            {
                move = (Input.mousePosition - dragOriginR) * (Options.cameraSensitivity / 100f);
                dragOriginR = Input.mousePosition;
            }
            transform.Rotate(new Vector3(0,move.x,0));
        }

        float ZoomCamera()
        {

            // Camera  X rotation
            var xRotation = minRot + zoomLevel * rotStep;
            camObject.transform.Rotate(xRotation - camObject.transform.rotation.eulerAngles.x, 0, 0);

            // Camera Fov
            var fov = minFov + zoomLevel * fovStep; ;
            Camera.main.fieldOfView = fov;

            // Camera Depth
            camObject.transform.position = transform.position - transform.forward * (minDepth + depthStep * zoomLevel);

            // Camera Y goal
            var old = hoverHeight;
            hoverHeight = minHeight + zoomLevel * heightStep;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, layerMask))
            {
                return hoverHeight - hit.distance;
            }
            else
            {
                return hoverHeight - old;
            }
        }
    }
}
