/*
 * Copyright 2024 Haply Robotics Inc. All rights reserved.
 */

using Haply.Inverse.DeviceControllers;
using Haply.Inverse.DeviceData;
using UnityEngine;

namespace Haply.Samples.Tutorials._2_BasicForceFeedback
{
    public class CubeForceFeedback : MonoBehaviour
    {
        public Inverse3Controller inverse3;

        [Range(0, 800)]
        // Stiffness of the force feedback.
        public float stiffness = 300f;

        [Range(0, 3)]
        public float damping = 1f;

        private Vector3 _cubePosition;
        private Vector3 _cubeSize;  // Size of the cube (width, height, depth)
        private float _cursorRadius;

        /// <summary>
        /// Stores the cursor and cube transform data for access by the haptic thread.
        /// </summary>
        private void SaveSceneData()
        {
            var t = transform;
            _cubePosition = t.position;
            _cubeSize = t.lossyScale; // Get the size (width, height, depth) of the cube

            _cursorRadius = inverse3.Cursor.Radius;
        }

        /// <summary>
        /// Saves the initial scene data cache.
        /// </summary>
        private void Awake()
        {
            inverse3 ??= FindFirstObjectByType<Inverse3Controller>();
            inverse3.Ready.AddListener((device, args) => SaveSceneData());
        }

        /// <summary>
        /// Subscribes to the DeviceStateChanged event.
        /// </summary>
        private void OnEnable()
        {
            inverse3.DeviceStateChanged += OnDeviceStateChanged;
        }

        /// <summary>
        /// Unsubscribes from the DeviceStateChanged event.
        /// </summary>
        private void OnDisable()
        {
            inverse3.DeviceStateChanged -= OnDeviceStateChanged;
            inverse3.Release();
        }

        /// <summary>
        /// Calculates the force based on the cursor's position and a cube's position.
        /// </summary>
        /// <param name="cursorPosition">The position of the cursor.</param>
        /// <param name="cursorVelocity">The velocity of the cursor.</param>
        /// <param name="cursorRadius">The radius of the cursor.</param>
        /// <param name="cubePosition">The center position of the cube.</param>
        /// <param name="cubeSize">The size of the cube (width, height, depth).</param>
        /// <returns>The calculated force vector.</returns>
        private Vector3 ForceCalculation(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius,
            Vector3 cubePosition, Vector3 cubeSize)
        {
            var force = Vector3.zero;

            // Calculate the closest point on the cube to the cursor
            Vector3 closestPoint = new Vector3(
                Mathf.Clamp(cursorPosition.x, cubePosition.x - cubeSize.x / 2f, cubePosition.x + cubeSize.x / 2f),
                Mathf.Clamp(cursorPosition.y, cubePosition.y - cubeSize.y / 2f, cubePosition.y + cubeSize.y / 2f),
                Mathf.Clamp(cursorPosition.z, cubePosition.z - cubeSize.z / 2f, cubePosition.z + cubeSize.z / 2f)
            );

            // Calculate the distance from the cursor to the closest point on the cube
            var distanceVector = cursorPosition - closestPoint;
            var distance = distanceVector.magnitude;
            var penetration = cursorRadius - distance;

            if (penetration > 0)
            {
                // Normalize the distance vector to get the direction of the force
                var normal = distanceVector.normalized;

                // Calculate the force based on penetration
                force = normal * penetration * stiffness;

                // Apply damping based on the cursor velocity
                force -= cursorVelocity * damping;
            }

            return force;
        }

        /// <summary>
        /// Event handler that calculates and sends the force to the device when the cursor's position changes.
        /// </summary>
        /// <param name="sender">The Inverse3 data object.</param>
        /// <param name="args">The event arguments containing the device data.</param>
        private void OnDeviceStateChanged(object sender, Inverse3EventArgs args)
        {
            var inverse3 = args.DeviceController;
            // Calculate the force between the cursor and the cube
            var force = ForceCalculation(inverse3.CursorLocalPosition, inverse3.CursorLocalVelocity,
                _cursorRadius, _cubePosition, _cubeSize);

            inverse3.SetCursorLocalForce(force);
        }
    }
}
