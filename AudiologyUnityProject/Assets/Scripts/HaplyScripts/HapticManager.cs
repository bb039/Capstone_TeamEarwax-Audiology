using System.Collections.Generic;
using Haply.Inverse.DeviceControllers;
using Haply.Inverse.DeviceData;
using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public Inverse3Controller inverse3;

    private readonly List<CubeForceFeedback> cubes = new();
    private readonly List<SphereForceFeedback> spheres = new();

    private void OnEnable()
    {
        if (inverse3 == null)
        {
            inverse3 = FindFirstObjectByType<Inverse3Controller>();
        }

        inverse3.DeviceStateChanged += OnDeviceStateChanged;
    }

    private void OnDisable()
    {
        inverse3.DeviceStateChanged -= OnDeviceStateChanged;
    }

    /// <summary>
    /// Called by each cube to register itself for haptic interaction.
    /// </summary>
    public void RegisterCube(CubeForceFeedback cube)
    {
        if (!cubes.Contains(cube))
            cubes.Add(cube);
    }

    /// <summary>
    /// Called by each sphere to register itself for haptic interaction.
    /// </summary>
    public void RegisterSphere(SphereForceFeedback sphere)
    {
        if (!spheres.Contains(sphere))
            spheres.Add(sphere);
    }

    /// <summary>
    /// Called whenever the device updates. It gathers all active force zones and combines their effects.
    /// </summary>
    private void OnDeviceStateChanged(object sender, Inverse3EventArgs args)
    {
        Vector3 pos = args.DeviceController.CursorLocalPosition;
        Vector3 vel = args.DeviceController.CursorLocalVelocity;
        float radius = args.DeviceController.Cursor.Radius;

        Vector3 totalForce = Vector3.zero;

        // Gather forces from cubes
        foreach (var cube in cubes)
        {
            totalForce += cube.CalculateForce(pos, vel, radius);
        }

        // Gather forces from spheres
        foreach (var sphere in spheres)
        {
            totalForce += sphere.CalculateForce(pos, vel, radius);
        }

        // Apply the combined force to the device
        args.DeviceController.SetCursorLocalForce(totalForce);
    }
}
