using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{
    public void OnTargetReach()
    {
        transform.position = new Vector3(Random.Range(-9.0f, 9.0f), transform.position.y, Random.Range(-9.0f, 9.0f));
    }
}
