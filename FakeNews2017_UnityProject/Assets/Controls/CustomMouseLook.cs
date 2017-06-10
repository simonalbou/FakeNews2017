using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[Serializable]
public class MouseLook
{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

	public bool touchInput;
	public bool dependOnAltKey = false;

    public void Init(Transform character, Transform camera)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
    }
	
    public void LookRotation(Transform character, Transform camera)
    {
		float yRot = 0;
		float xRot = 0;

		if (!touchInput)
		{
			yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
			xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
		}
		else
		{
			float deltaX = 0;
			float deltaY = 0;

			if(Input.touchCount > 0)
			{
				for(int i=0; i<Input.touchCount; i++)
				{
					Touch tch = Input.touches[i];
					if(tch.phase == TouchPhase.Moved)
					{
						// applying Mathf.Sign() might be necessary
						deltaX += tch.deltaPosition.x;
						deltaY += tch.deltaPosition.y;
					}
				}
			}

			yRot = deltaX * XSensitivity;
			xRot = deltaY * YSensitivity;			
		}

		if (dependOnAltKey && !Input.GetKey(KeyCode.LeftAlt)) yRot = xRot = 0;

        //m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
		m_CharacterTargetRot = Quaternion.Euler(CorrectEuler(character.localEulerAngles + Vector3.up * yRot));
		//m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
		m_CameraTargetRot = Quaternion.Euler(CorrectEuler(camera.localEulerAngles + Vector3.left * xRot));

        if(clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

        if(smooth)
        {
            character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }
    }

	Vector3 CorrectEuler(Vector3 input)
	{
		Vector3 output = new Vector3(CorrectAngle(input.x), CorrectAngle(input.y), CorrectAngle(input.z));

		return output;
	}

	float CorrectAngle(float input)
	{
		return input > 0 ? input % 360 : ((input + 720) % 360);
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

}


public class CustomMouseLook : MonoBehaviour
{
	public Transform playerTransform, self;

	[SerializeField]
	private MouseLook _look;

	private bool disabledForDebug = false;

	void OnEnable()
	{
		_look.Init(playerTransform, self);
	}

	void Update()
	{
		if(!disabledForDebug)
			_look.LookRotation(playerTransform, self);

		//if (Input.GetKeyDown(KeyCode.LeftShift)) disabledForDebug = !disabledForDebug;
	}
}