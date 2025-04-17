/*
 * Copyright 2024 Haply Robotics Inc. All rights reserved.
 */

using Haply.Inverse.DeviceControllers;
using Haply.Inverse.DeviceData;
using UnityEngine;

namespace Haply.Samples.Tutorials._2_BasicForceFeedback
{
    public class CylinderForceFeedback : MonoBehaviour
    {
        public Inverse3Controller inverse3;

        [Range(0, 800)]
        public float stiffness = 300f;
        [Range(0, 3)]
        public float damping = 1f;

        private Vector3 _cylinderPosition;
        private float _cylinderRadius;
        private float _cylinderHeight;
        private float _cursorRadius;

        /// <summary>
        /// Stores the cylinder position and dimensions.
        /// </summary>
        private void SaveSceneData()
        {
            var t = transform;
            _cylinderPosition = t.position;
            _cylinderRadius = t.lossyScale.x / 2f; // X scale represents diameter
            _cylinderHeight = t.lossyScale.y; // Y scale represents height
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
        /// Calculates the force pushing the cursor away from the outer surface of the cylinder.
        /// </summary>
        private Vector3 ForceCalculation(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius)
        {
            Vector3 force = Vector3.zero;

            // Convert cursor to local space relative to cylinder
            Vector3 localCursorPos = cursorPosition - _cylinderPosition;

            // Compute radial distance from center axis (ignoring Y for height check)
            Vector3 radialVector = new Vector3(localCursorPos.x, 0, localCursorPos.z);
            float radialDistance = radialVector.magnitude;
            float penetrationRadial = radialDistance - (_cylinderRadius + cursorRadius);

            // Height constraints (must be within the top and bottom of the cylinder)
            float halfHeight = _cylinderHeight / 2f;
            bool withinHeight = (localCursorPos.y >= -halfHeight) && (localCursorPos.y <= halfHeight);

            if (penetrationRadial < 0 && withinHeight)
            {
                // Push outward from the cylinder walls
                Vector3 normal = radialVector.normalized;
                force += normal * -penetrationRadial * stiffness;
            }

            // Apply damping to smooth force response
            force -= cursorVelocity * damping;

            return force;
        }

        /// <summary>
        /// Event handler that calculates and sends force to the device.
        /// </summary>
        private void OnDeviceStateChanged(object sender, Inverse3EventArgs args)
        {
            var inverse3 = args.DeviceController;

            // Calculate force for outside the cylinder
            var force = ForceCalculation(inverse3.CursorLocalPosition, inverse3.CursorLocalVelocity, _cursorRadius);

            inverse3.SetCursorLocalForce(force);
        }
    }
}
