/*
 * Copyright 2024 Haply Robotics Inc. All rights reserved.
 */

using Haply.Inverse.DeviceControllers;   // Contains the Inverse3Controller class for haptic device control
using Haply.Inverse.DeviceData;          // Contains event argument definitions for haptic device events
using UnityEngine;                        // Core Unity functionality

namespace Haply.Samples.Tutorials._2_BasicForceFeedback
{
    /// <summary>
    /// This class provides force feedback when the haptic cursor interacts with a sphere in Unity.
    /// It calculates forces that allow the user to feel the surface of the sphere through the haptic device.
    /// </summary>
    public class TunnelForceFeedback : MonoBehaviour
    {

        private Vector3 _cylinderPosition;
        private float _innerRadius;
        private float _outerRadius;
        private float _cylinderHeight;

        // Reference to the Inverse3 haptic controller that manages the physical device
        public Inverse3Controller inverse3;

        [Range(0, 800)]
        // Stiffness of the force feedback - higher values create a harder/firmer surface feel
        public float stiffness = 300f;

        [Range(0, 3)]
        // Damping factor to reduce oscillations and improve stability of force feedback
        public float damping = 1f;

        // Cached position of the sphere (this GameObject) in world space
        private Vector3 _ballPosition;

        // Cached radius of the sphere, calculated from its scale
        private float _ballRadius;

        // Cached radius of the haptic cursor
        private float _cursorRadius;

        /// <summary>
        /// Stores the current positions and dimensions of the cursor and sphere.
        /// This data is cached for efficient access by the haptic thread.
        /// </summary>
        private void SaveSceneData()
        {
            var t = transform;
            // Store the position of the tunnel (cylinder) GameObject
            _cylinderPosition = t.position;
            // Calculate the inner radius (assuming uniform scaling for the cylinder)
            _innerRadius = t.lossyScale.x / 2f;
            // We do not need the outer radius for force calculation, but it's still useful for checks.
            _outerRadius = t.lossyScale.x / 2f;
            // Store the height of the tunnel
            _cylinderHeight = t.lossyScale.y;
            // Store the radius of the haptic cursor
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
        /// Calculates the force based on the cursor's position relative to the sphere.
        /// This is the core haptic algorithm that simulates physical contact with the sphere.
        /// </summary>
        /// <param name="cursorPosition">The position of the haptic cursor in world space.</param>
        /// <param name="cursorVelocity">The velocity vector of the cursor.</param>
        /// <param name="cursorRadius">The physical radius of the cursor.</param>
        /// <param name="otherPosition">The position of the sphere in world space.</param>
        /// <param name="otherRadius">The radius of the sphere.</param>
        /// <returns>The calculated force vector to be applied to the haptic device.</returns>
        private Vector3 ForceCalculation(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius,
    Vector3 cylinderPosition, float innerRadius, float cylinderHeight)
        {
            var force = Vector3.zero;

            // Calculate the vector from the cylinder center to the cursor
            var distanceVector = cursorPosition - cylinderPosition;

            // Calculate the distance between the centers
            var distance = distanceVector.magnitude;

            // Check if the cursor is within the inner wall (not inside the hole)
            var penetration = innerRadius + cursorRadius - distance;

            // Only apply force if the cursor is on the inner wall (i.e., within the cylinder, but not inside)
            if (penetration > 0)
            {
                // Check if the cursor is within the vertical bounds of the cylinder
                if (Mathf.Abs(cursorPosition.y - cylinderPosition.y) <= cylinderHeight / 2f)
                {
                    // Normalize the distance vector to get the direction of the force
                    var normal = distanceVector.normalized;

                    // Calculate the force magnitude based on penetration depth
                    force = normal * penetration * stiffness;

                    // Apply damping based on the cursor velocity (prevents oscillations)
                    force -= cursorVelocity * damping;
                }
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

            // Calculate the force based on the cursor's position relative to the tunnel (cylinder)
            var force = ForceCalculation(inverse3.CursorLocalPosition, inverse3.CursorLocalVelocity,
                _cursorRadius, _cylinderPosition, _innerRadius, _cylinderHeight);

            // Apply the calculated force to the haptic device
            inverse3.SetCursorLocalForce(force);
        }

    }
}