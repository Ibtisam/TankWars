using UnityEngine;

public class HBRotation : MonoBehaviour
{
    Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - camTransform.position);
    }
}
