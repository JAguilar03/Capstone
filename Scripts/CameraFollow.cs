using UnityEngine;

// This script makes the camera follow the player smoothly with an optional offset.
// The camera's position is updated every frame, keeping the player in view with the specified smoothing speed.

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    // public float smoothSpeed = 0.05f;
    public Vector3 offset;

    void Start()
    {
        target = PlayerManager.playerInstance.transform;
    }

    // This finds the player and follows it
    void LateUpdate()
    {
        if (target == null) {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10) + offset;
        // Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // transform.position = smoothedPosition;
        transform.position = desiredPosition;
    }
}
