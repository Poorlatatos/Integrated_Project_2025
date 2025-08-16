using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 1/08/2025
    * Description: Door opener script for Unity
      When the player finished collecting all items in the checklist, it will open the doors
    */
    public Transform leftDoor;   /// Assign the left door panel
    public Transform rightDoor;  /// Assign the right door panel
    public Vector3 leftOpenRotation = new Vector3(0, -90, 0);  /// Left door swings left
    public Vector3 rightOpenRotation = new Vector3(0, 90, 0);  /// Right door swings right
    public float openDuration = 1.5f; /// Duration for doors to fully open

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

    public void OpenDoor() /// Open the door
    {
        if (!isOpen)
        {
            isOpen = true;
            StartCoroutine(OpenDoorRoutine());
        }
    }

    private System.Collections.IEnumerator OpenDoorRoutine() /// Handle door opening animation
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