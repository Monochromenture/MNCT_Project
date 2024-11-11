using UnityEngine;

namespace Visu
{
    public class VisuCameraFollow : MonoBehaviour
    {
        [Tooltip("The target object followed by the Camera")]
        public Transform target;
        [Tooltip("The camera that will follow the target. " +
                 "If empty, it will check if this object has a camera component. " +
                 "If not, then Main Camera is assigned automatically.")]
        public Camera myCamera;

        private Vector3 velocity = Vector3.zero;
        private Vector3 initialCameraPosition;
        public Vector3 cameraOffset;
        public float smoothFactor = 0.2f;

        [Tooltip("The minimum bounds for the camera's position")]
        public Vector2 minBounds;
        [Tooltip("The maximum bounds for the camera's position")]
        public Vector2 maxBounds;

        void Awake()
        {
            // Check if target is set
            if (target == null)
            {
                Debug.LogError("Target not selected.");
                enabled = false; // Disable the script if target is not set
                return;
            }

            // Call the method to find the camera
            FindCamera();

            // Find the initial position of the camera
            initialCameraPosition = myCamera.transform.position - target.position;
        }

        void FindCamera()
        {
            if (myCamera != null)
                return;

            if (TryGetComponent(out Camera foundCamera))
            {
                myCamera = foundCamera;
                Debug.Log("Camera assigned automatically to the component of the object.");
                return;
            }
            else if ((myCamera = Camera.main) != null)
            {
                Debug.Log("Main Camera assigned automatically.");
                return;
            }

            Debug.LogError("No camera found!");
            enabled = false;
        }

        private void LateUpdate()
        {
            if (target == null || myCamera == null)
                return;

            // Find next position based on target's position and camera offset
            Vector3 nextPosition = target.position + initialCameraPosition + cameraOffset;

            // Smooth movement
            Vector3 smoothedPosition = Vector3.SmoothDamp(myCamera.transform.position, nextPosition, ref velocity, smoothFactor);

            // Clamp the position within the specified bounds
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);

            // Update camera position to follow target
            myCamera.transform.position = smoothedPosition;
        }
    }
}
