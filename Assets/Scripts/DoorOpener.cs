using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Transform leftDoor;   // Assign the left door panel
    public Transform rightDoor;  // Assign the right door panel
    public Vector3 leftOpenRotation = new Vector3(0, -90, 0);  // Left door swings left
    public Vector3 rightOpenRotation = new Vector3(0, 90, 0);  // Right door swings right
    public float openDuration = 1.5f;

    private Quaternion leftClosedRot, leftOpenRot;
    private Quaternion rightClosedRot, rightOpenRot;
    private bool isOpen = false;

    void Start()
    {
        if (leftDoor != null)
        {
            leftClosedRot = leftDoor.localRotation;
            leftOpenRot = leftClosedRot * Quaternion.Euler(leftOpenRotation);
        }
        if (rightDoor != null)
        {
            rightClosedRot = rightDoor.localRotation;
            rightOpenRot = rightClosedRot * Quaternion.Euler(rightOpenRotation);
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            StartCoroutine(OpenDoorRoutine());
        }
    }

    private System.Collections.IEnumerator OpenDoorRoutine()
    {
        float timer = 0f;
        while (timer < openDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / openDuration);
            if (leftDoor != null)
                leftDoor.localRotation = Quaternion.Slerp(leftClosedRot, leftOpenRot, t);
            if (rightDoor != null)
                rightDoor.localRotation = Quaternion.Slerp(rightClosedRot, rightOpenRot, t);
            yield return null;
        }
        if (leftDoor != null)
            leftDoor.localRotation = leftOpenRot;
        if (rightDoor != null)
            rightDoor.localRotation = rightOpenRot;
    }
}