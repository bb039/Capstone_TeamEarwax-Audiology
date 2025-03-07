/*
 * Copyright 2024 Haply Robotics Inc. All rights reserved.
 */

using Haply.Inverse.DeviceControllers;
using Haply.Inverse.DeviceData;
using UnityEngine;

namespace Haply.Samples.Tutorials._2_BasicForceFeedback
{
    /// <summary>
    /// This class provides force feedback when the haptic cursor interacts with a cube in Unity.
    /// It calculates forces that allow the user to feel the surface of the cube through the haptic device.
    /// </summary>
    public class CubeForceFeedback : MonoBehaviour
    {
        // Reference to the Inverse3 haptic controller that manages the physical device
        public Inverse3Controller inverse3;

        [Range(0, 800)]
        // Stiffness of the force feedback - higher values create a harder/firmer surface feel
        public float stiffness = 300f;

        [Range(0, 3)]
        // Damping factor to reduce oscillations and improve stability of force feedback
        public float damping = 1f;

        // Cached position of the cube (this GameObject) in world space
        private Vector3 _cubePosition;
        
        // Cached half extents of the cube (half width, height, depth)
        private Vector3 _cubeHalfExtents;
        
        // Cached radius of the haptic cursor
        private float _cursorRadius;

        /// <summary>
        /// Stores the current positions and dimensions of the cursor and cube.
        /// This data is cached for efficient access by the haptic thread.
        /// </summary>
        private void SaveSceneData()
        {
            var t = transform;
            // Store the position of this cube GameObject
            _cubePosition = t.position;
            
            // Calculate the half extents of the cube (assumes the cube has a BoxCollider)
            if (TryGetComponent<BoxCollider>(out var boxCollider))
            {
                // Use the BoxCollider's size multiplied by transform scale
                _cubeHalfExtents = Vector3.Scale(boxCollider.size * 0.5f, t.lossyScale);
            }
            else
            {
                // Fallback: use the transform's scale directly
                _cubeHalfExtents = t.lossyScale * 0.5f;
            }

            // Store the cursor's physical radius
            _cursorRadius = inverse3.Cursor.Radius;
        }

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Sets up initial references and event listeners.
        /// </summary>
        private void Awake()
        {
            // If inverse3 isn't assigned in the Inspector, try to find it in the scene
            inverse3 ??= FindFirstObjectByType<Inverse3Controller>();
            
            // When the haptic device is ready, cache our scene data
            inverse3.Ready.AddListener((device, args) => SaveSceneData());
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// Subscribes to haptic device events.
        /// </summary>
        private void OnEnable()
        {
            // Subscribe to device state change events to calculate forces
            inverse3.DeviceStateChanged += OnDeviceStateChanged;
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// Unsubscribes from events to prevent memory leaks.
        /// </summary>
        private void OnDisable()
        {
            // Clean up event subscriptions
            inverse3.DeviceStateChanged -= OnDeviceStateChanged;
            // Release the device to free resources
            inverse3.Release();
        }

        /// <summary>
        /// Finds the closest point on a cube to a given position.
        /// Used to determine where the cursor contacts the cube.
        /// </summary>
        /// <param name="position">The position to find closest point to (usually cursor position)</param>
        /// <param name="cubePosition">The center position of the cube</param>
        /// <param name="cubeHalfExtents">Half the width, height, and depth of the cube</param>
        /// <returns>The closest point on the cube's surface (or inside it)</returns>
        private Vector3 GetClosestPointOnCube(Vector3 position, Vector3 cubePosition, Vector3 cubeHalfExtents)
        {
            // Convert position to cube's local space
            Vector3 localPos = position - cubePosition;
            
            // Clamp each component to the cube's bounds
            Vector3 closestLocal = new Vector3(
                Mathf.Clamp(localPos.x, -cubeHalfExtents.x, cubeHalfExtents.x),
                Mathf.Clamp(localPos.y, -cubeHalfExtents.y, cubeHalfExtents.y),
                Mathf.Clamp(localPos.z, -cubeHalfExtents.z, cubeHalfExtents.z)
            );
            
            // Check if the clamped position is inside the cube
            bool isInside = closestLocal == localPos;
            
            if (isInside)
            {
                // If cursor is inside, find the closest face
                float xDist = cubeHalfExtents.x - Mathf.Abs(localPos.x);
                float yDist = cubeHalfExtents.y - Mathf.Abs(localPos.y);
                float zDist = cubeHalfExtents.z - Mathf.Abs(localPos.z);
                
                // Select the closest face
                if (xDist <= yDist && xDist <= zDist)
                {
                    // X face is closest
                    closestLocal.x = cubeHalfExtents.x * Mathf.Sign(localPos.x);
                }
                else if (yDist <= xDist && yDist <= zDist)
                {
                    // Y face is closest
                    closestLocal.y = cubeHalfExtents.y * Mathf.Sign(localPos.y);
                }
                else
                {
                    // Z face is closest
                    closestLocal.z = cubeHalfExtents.z * Mathf.Sign(localPos.z);
                }
            }
            
            // Convert back to world space
            return closestLocal + cubePosition;
        }

        /// <summary>
        /// Calculates the force based on the cursor's position relative to the cube.
        /// This is the core haptic algorithm that simulates physical contact with the cube.
        /// </summary>
        /// <param name="cursorPosition">The position of the haptic cursor in world space</param>
        /// <param name="cursorVelocity">The velocity vector of the cursor</param>
        /// <param name="cursorRadius">The physical radius of the cursor</param>
        /// <param name="cubePosition">The position of the cube in world space</param>
        /// <param name="cubeHalfExtents">Half the width, height, and depth of the cube</param>
        /// <returns>The calculated force vector to be applied to the haptic device</returns>
        private Vector3 CubeForceCalculation(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius,
            Vector3 cubePosition, Vector3 cubeHalfExtents)
        {
            // Initialize force to zero - no force by default
            var force = Vector3.zero;
            
            // Find closest point on cube to cursor center
            Vector3 closestPoint = GetClosestPointOnCube(cursorPosition, cubePosition, cubeHalfExtents);
            
            // Get direction and distance from closest point to cursor
            Vector3 directionVector = cursorPosition - closestPoint;
            float distance = directionVector.magnitude;
            
            // Calculate penetration (positive if cursor overlaps with cube)
            float penetration = cursorRadius - distance;
            
            // Only apply forces if there is penetration (collision)
            if (penetration > 0)
            {
                // Calculate normal direction (from cube to cursor)
                // If distance is zero, use a fallback direction (up)
                Vector3 normal = distance > 0 ? directionVector / distance : Vector3.up;
                
                // Calculate force based on penetration and stiffness
                // Deeper penetration = stronger force
                force = normal * penetration * stiffness;
                
                // Apply damping based on cursor velocity
                // This helps prevent oscillations and improves stability
                force -= cursorVelocity * damping;
            }
            
            return force;
        }

        /// <summary>
        /// Event handler called when the haptic device's position changes.
        /// This calculates and applies the appropriate force feedback.
        /// </summary>
        /// <param name="sender">The Inverse3 data object.</param>
        /// <param name="args">The event arguments containing the device data.</param>
        private void OnDeviceStateChanged(object sender, Inverse3EventArgs args)
        {
            var inverse3 = args.DeviceController;
            
            // Calculate the force based on current cursor and cube positions
            var force = CubeForceCalculation(inverse3.CursorLocalPosition, inverse3.CursorLocalVelocity,
                _cursorRadius, _cubePosition, _cubeHalfExtents);

            // Apply the calculated force to the haptic device
            inverse3.SetCursorLocalForce(force);
        }
    }
}