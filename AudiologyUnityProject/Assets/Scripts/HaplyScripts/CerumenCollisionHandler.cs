using UnityEngine;

public class DespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovingSphere>() != null)
        {
            Destroy(other.gameObject);
        }
    }
}
