/// <summary>
/// Handles the camera attached to player. Rotates the camera when user move the mouse.
/// 
/// Attached to player camera object inside player. Support 3rd person.
/// </summary>


// Adapted from Unity standard assets -> character -> first person
using System;
using UnityEngine;

[Serializable]
public class PlayerLook : MonoBehaviour
{
	public float XSensitivity = 2f;
	public float YSensitivity = 2f;
	public bool clampVerticalRotation = true;
	public float MinimumX = -90F;
	public float MaximumX = 90F;
	public bool smooth;
	public float smoothTime = 5f;
	public bool lockCursor = true;

	public bool AllowThirdPerson = true;
	public KeyCode ZoomIn = KeyCode.Mouse2;
	public KeyCode ZoomOut = KeyCode.Mouse3;
	public float MinCameraDistance = 3f;
	public float MaxCameraDistance = 20f;
	public float zoomStep = 1f;

	private Transform m_CharacterTarget;
	private Transform m_CameraTarget;
	private Quaternion m_CharacterTargetRot;
	private Quaternion m_CameraTargetRot;
	private bool m_cursorIsLocked = true;
	private bool m_cursorLockedInitially = true;
	private float m_zoom = 0f;
	private Transform m_actualCam;

	private bool m_UISMExist = true;

	public void Start()
	{
		m_CharacterTarget = transform.parent;
		m_CameraTarget = transform;
		m_CharacterTargetRot = m_CharacterTarget.localRotation;
		m_CameraTargetRot = m_CameraTarget.localRotation;
		m_actualCam = transform.Find ("Camera");
		m_cursorLockedInitially = lockCursor;

		m_UISMExist = UIStateManager.controller != null;
	}


	public void Update()
	{
		if (!m_UISMExist || UIStateManager.controller.inGame) {
			float yRot = Input.GetAxis ("Mouse X") * XSensitivity;
			float xRot = Input.GetAxis ("Mouse Y") * YSensitivity;


			

			yRot += Input.GetAxis (Utilities.JOYSTICK_RIGHT_HOR) * XSensitivity;
			xRot += Input.GetAxis (Utilities.JOYSTICK_RIGHT_VER) * YSensitivity;

			m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
			m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

			if (clampVerticalRotation)
				m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

			if (smooth) {
				m_CharacterTarget.localRotation = Quaternion.Slerp (m_CharacterTarget.localRotation, m_CharacterTargetRot,
					smoothTime * Time.deltaTime);
				m_CameraTarget.localRotation = Quaternion.Slerp (m_CameraTarget.localRotation, m_CameraTargetRot,
					smoothTime * Time.deltaTime);
			} else {
				m_CharacterTarget.localRotation = m_CharacterTargetRot;
				m_CameraTarget.localRotation = m_CameraTargetRot;
			}

			UpdateCameraZoom ();
		}
		UpdateCursorLock();
	}

	public void SetPlayerRotation(Quaternion rot){
		m_CharacterTargetRot = rot;
		m_CameraTargetRot = Quaternion.Euler (0f, 0f, 0f);
		m_CharacterTarget.localRotation = rot;
		m_CameraTarget.localRotation = Quaternion.Euler (0f, 0f, 0f);
	}

	public void SetCursorLock(bool value)
	{
		lockCursor = value;
		if(!lockCursor)
		{//we force unlock the cursor if the user disable the cursor locking helper
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void UpdateCursorLock()
	{
		//if the user set "lockCursor" we check & properly lock the cursos
		if (m_UISMExist) {
			if (UIStateManager.controller.inGame && m_cursorLockedInitially) {
				SetCursorLock (true);
				m_cursorIsLocked = true;
			} else if (!UIStateManager.controller.inGame) {
				SetCursorLock (false);
			}
		}
		if (lockCursor)
			InternalLockUpdate ();
	}

	private void InternalLockUpdate()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			m_cursorIsLocked = false;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			m_cursorIsLocked = true;
		}

		if (m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else if (!m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

		angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}

	void UpdateCameraZoom() {
		if (!AllowThirdPerson) {
			return;
		}
		float zoom = Input.GetAxis ("Mouse ScrollWheel");
		if (zoom > 0f || Input.GetKeyDown (ZoomIn)) {
			m_zoom -= zoomStep;
			if (m_zoom < MinCameraDistance) {
				m_zoom = 0f;
				m_actualCam.localPosition = new Vector3 (0f, 0f, 0f);
				return;
			} 
		}
		else if (zoom < 0f || Input.GetKeyDown (ZoomOut)) {
			if (m_actualCam.localPosition.z == 0f) {
				m_actualCam.localPosition = new Vector3 (0f, 0f, -MinCameraDistance);
				m_zoom = MinCameraDistance;
				return;
			} 
			m_zoom = Math.Min (m_zoom + zoomStep, MaxCameraDistance);
		}
		m_actualCam.localPosition = Vector3.Lerp (m_actualCam.localPosition, new Vector3 (0f, 0f, -m_zoom), 0.2f);
	}

}