using UnityEngine;

public class SpawnSphereScript : MonoBehaviour
{
    public GameObject spherePrefab;
    public Transform managerObject;
    public int amountToSpawn = 10;

    void Start()
    {

        float radius = managerObject.localScale.x * 0.5f - 0.1f; 

        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 spawnPos = GetRandomEdgePosition(radius);
            //Vector3 spawnPos = GetRandomPosition(radius);
            Instantiate(spherePrefab, spawnPos, Quaternion.identity);
        }
    }

    Vector3 GetRandomEdgePosition(float radius)
    {
        float height = managerObject.localScale.y;
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float x = Mathf.Cos(angle) * radius;
        //float y = Mathf.Sin(angle) * radius;
        float y = Random.Range(-height / 2, 0);
        //float z = Mathf.Sin(angle) * radius;
        float z = Random.Range((float)(-managerObject.localScale.z + 0.2), (float)(managerObject.localScale.z - 0.2));
        Vector3 localPos = new Vector3(x, y, z);

        return managerObject.position + localPos;
    }

    Vector3 GetRandomPosition(float radius)
    {
        float height = managerObject.localScale.y;

        float angle = Random.Range(0f, Mathf.PI * 2);


        float r = radius * Mathf.Sqrt(Random.value);
        float x = r * Mathf.Cos(angle);

        float z = r * Mathf.Sin(angle);
        float y = Random.Range(-height, height);

        Vector3 localPos = new Vector3(x, y, z);

        return managerObject.TransformPoint(localPos);
    }
}