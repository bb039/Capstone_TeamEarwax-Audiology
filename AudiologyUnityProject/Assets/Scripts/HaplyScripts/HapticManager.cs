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
    private readonly List<MovingSphere> movingSpheres = new();
    private readonly List<PlaneForceFeedback> planes = new();
    private readonly List<CuretteHapticTrigger> curetteTriggers = new();
    private readonly List<CylinderForceFeedback> cylinders = new();


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

    public void RegisterMovingSphere(MovingSphere sphere)
    {
        if (!movingSpheres.Contains(sphere))
            movingSpheres.Add(sphere);
    }

    public void RegisterPlane(PlaneForceFeedback plane)
    {
        if (!planes.Contains(plane))
            planes.Add(plane);
    }

    public void RegisterCuretteTrigger(CuretteHapticTrigger trigger)
    {
        if (!curetteTriggers.Contains(trigger))
            curetteTriggers.Add(trigger);
    }

    public void RegisterCylinder(CylinderForceFeedback cylinder)
    {
        if (!cylinders.Contains(cylinder))
            cylinders.Add(cylinder);
    }


    void OnDeviceStateChanged(object sender, Inverse3EventArgs args)
    {
        Vector3 pos = args.DeviceController.CursorLocalPosition;
        Vector3 vel = args.DeviceController.CursorLocalVelocity;
        float radius = args.DeviceController.Cursor.Radius;

        Vector3 totalForce = Vector3.zero;
        float maxDanger = 0f;
        string warningMessage = "";

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

        foreach (var sphere in spheres)
        {
            totalForce += sphere.CalculateForce(pos, vel, radius);
        }

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

        foreach (var tube in curvedTubes)
        {
            totalForce += tube.CalculateForce(pos, vel, radius);
        }

        foreach (var sphere in movingSpheres)
        {
            totalForce += sphere.CalculateForce(pos, vel, radius);
        }

        foreach (var plane in planes)
        {
            totalForce += plane.CalculateForce(pos, vel, radius);
        }

        foreach (var trigger in curetteTriggers)
        {
            if (trigger.isTouchingCube)
            {
                totalForce += trigger.CalculateForce(pos, vel, radius);
            }
        }

        foreach (var cylinder in cylinders)
        {
            totalForce += cylinder.CalculateForce(pos, vel, radius);
        }

        args.DeviceController.SetCursorLocalForce(totalForce);

        DangerOverlayUI.SetIntensity(maxDanger, warningMessage);
    }
}
