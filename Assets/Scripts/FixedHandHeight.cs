// Attach this to your hand object
using UnityEngine;

public class FixedHandHeight : MonoBehaviour
{
    public float fixedLocalY = 0f;

    void LateUpdate()
    {
        Vector3 pos = transform.localPosition;
        pos.y = fixedLocalY;
        transform.localPosition = pos;
    }
}