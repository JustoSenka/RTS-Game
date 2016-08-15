using UnityEngine;
using System.Collections;
using System;

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

	private bool moveToDesiredPosition = false;
	private Vector3 desiredCameraPosition;
	private Vector3 oldMousePosition;
	private Vector3 oldCameraPosition;

	void Update()
    {
        ComputeSpeeds();
		ApplyCameraMovement();
        ApplyFancyRotationWhenCameraIsNearGround();
		ApplyMidleMouseButtonMovementSpeed();
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
        computedZoomSpeed = -Input.GetAxis("MouseScrollWheel") * zoomSpeed * 0.015f;

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

	private void ApplyMidleMouseButtonMovementSpeed()
	{
		if (Input.GetKeyDown(KeyCode.Mouse2))
		{
			oldMousePosition = Input.mousePosition;
			oldCameraPosition = transform.position;
			desiredCameraPosition = Common.GetWorldMousePoint(LayerMask.GetMask("Ground For Scrolling"))
				+ new Vector3(0, cameraZoom, -cameraZoom * Mathf.Tan((90 - transform.rotation.eulerAngles.x) * Mathf.Deg2Rad) - 2);
		}

		// Middle Mouse Click
		if (Input.GetKeyUp(KeyCode.Mouse2) && Vector3.Distance(Input.mousePosition, oldMousePosition) < 3f)
			moveToDesiredPosition = true;

		// Mouse drag
		if (Input.GetKey(KeyCode.Mouse2) && Vector3.Distance(Input.mousePosition, oldMousePosition) > 3f)
		{
			var mask = LayerMask.GetMask("Ground For Scrolling");
			transform.position = oldCameraPosition - Common.GetWorldMousePoint(mask) + Common.GetWorldMousePoint(mask, oldMousePosition);
		}

		if (moveToDesiredPosition)
			transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, 12 * Time.deltaTime);

		// Check if arrived at desired position
		if (Vector3.Distance(transform.position, desiredCameraPosition) < 0.5f)
			moveToDesiredPosition = false;
	}
}
