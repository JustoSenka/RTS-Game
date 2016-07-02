using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class RtsCamera : MonoBehaviour
{
    public float moveSpeedHorizontal = 15f;
    public float moveSpeedVertical = 15f;
    public float zoomSpeed = 300f;

    public float cameraMinHeigh = 4f;
    public float cameraMaxHeigh = 30f;

    public Vector3 cameraFarAngle = new Vector3(50, 0, 0);
    public Vector3 cameraNearAngle = new Vector3(30, 0, 0);

    public float nearAngleThreshold = 10f;

    private float computedSpeedHorizontal;
    private float computedSpeedVertical;
    private float computedZoomSpeed;

    private float cameraZoom = 10;

    void Update()
    {
        ComputeSpeeds();
        ApplyCameraMovement();
        ApplyFancyRotationWhenCameraIsNearGround();
    }

    private void ApplyCameraMovement()
    {
        transform.position += new Vector3(computedSpeedHorizontal, 0, computedSpeedVertical);
        Vector3 zoomedPosition = Vector3.Lerp(transform.position, new Vector3(transform.position.x, cameraZoom, transform.position.z), 10 * Time.deltaTime);
        transform.position = zoomedPosition + new Vector3(0, 0, transform.position.y - zoomedPosition.y);
    }

    private void ApplyFancyRotationWhenCameraIsNearGround()
    {
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = Vector3.Lerp(cameraFarAngle, cameraNearAngle, (nearAngleThreshold - transform.position.y) / (nearAngleThreshold - cameraMinHeigh));
        transform.localRotation = rotation;
    }

    private void ComputeSpeeds()
    {
        computedSpeedVertical = Input.GetAxis("Vertical") * moveSpeedVertical * Time.deltaTime;
        computedSpeedHorizontal = Input.GetAxis("Horizontal") * moveSpeedHorizontal * Time.deltaTime;
        computedZoomSpeed = -Input.GetAxis("MouseScrollWheel") * zoomSpeed * Time.deltaTime;

#if !UNITY_EDITOR
        if (Input.mousePosition.x > Screen.width - 10) computedSpeedHorizontal = moveSpeedHorizontal * Time.deltaTime;
        if (Input.mousePosition.x < 10) computedSpeedHorizontal = - moveSpeedHorizontal * Time.deltaTime;
        if (Input.mousePosition.y > Screen.height - 10) computedSpeedVertical = moveSpeedVertical * Time.deltaTime;
        if (Input.mousePosition.y < 10) computedSpeedVertical = - moveSpeedVertical * Time.deltaTime;
#endif   
        cameraZoom += computedZoomSpeed;
        cameraZoom = (cameraZoom > cameraMaxHeigh) ? cameraMaxHeigh : cameraZoom;
        cameraZoom = (cameraZoom < cameraMinHeigh) ? cameraMinHeigh : cameraZoom;
    }
}
