using System.Collections.Generic;
using Haply.Inverse.DeviceControllers;
using Haply.Inverse.DeviceData;
using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public Inverse3Controller inverse3;

    private readonly List<CubeForceFeedback> cubes = new();
    private readonly List<SphereForceFeedback> spheres = new();
    private readonly List<DangerZone> dangerZones = new();
    private readonly List<CurvedTubeForceFeedback> curvedTubes = new();

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
        if (inverse3 != null)
            inverse3.DeviceStateChanged -= OnDeviceStateChanged;
    }

    public void RegisterCube(CubeForceFeedback cube)
    {
        if (!cubes.Contains(cube))
            cubes.Add(cube);
    }

    public void RegisterSphere(SphereForceFeedback sphere)
    {
        if (!spheres.Contains(sphere))
            spheres.Add(sphere);
    }

    public void RegisterDangerZone(DangerZone zone)
    {
        if (!dangerZones.Contains(zone))
            dangerZones.Add(zone);
    }

    public void RegisterTube(CurvedTubeForceFeedback tube)
    {
        if (!curvedTubes.Contains(tube))
            curvedTubes.Add(tube);
    }

    void OnDeviceStateChanged(object sender, Inverse3EventArgs args)
    {
        Vector3 pos = args.DeviceController.CursorLocalPosition;
        Vector3 vel = args.DeviceController.CursorLocalVelocity;
        float radius = args.DeviceController.Cursor.Radius;

        Vector3 totalForce = Vector3.zero;
        float maxDanger = 0f;
        string warningMessage = "";

        // Check cubes
        foreach (var cube in cubes)
        {
            totalForce += cube.CalculateForce(pos, vel, radius);

            float cubeDanger = cube.NormalizedPenetration();
            if (cubeDanger > maxDanger)
            {
                maxDanger = cubeDanger;
                warningMessage = cube.warningMessage;
            }
        }

        // Check spheres (optional)
        foreach (var sphere in spheres)
        {
            totalForce += sphere.CalculateForce(pos, vel, radius);
        }

        // Check danger zones
        foreach (var zone in dangerZones)
        {
            float penetration = zone.GetPenetrationDepth(pos, radius);
            float zoneDanger = zone.NormalizedPenetration();
            if (zoneDanger > maxDanger)
            {
                maxDanger = zoneDanger;
                warningMessage = zone.warningMessage;
            }
        }

        // Check tubes
        foreach (var tube in curvedTubes)
        {
            totalForce += tube.CalculateForce(pos, vel, radius);
        }

        args.DeviceController.SetCursorLocalForce(totalForce);

        DangerOverlayUI.SetIntensity(maxDanger, warningMessage);
    }
}
