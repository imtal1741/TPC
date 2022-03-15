using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Camera cam;
    RaycastHit _camHit;

    public GameObject character;
    public GameObject cameraCenter;
    public Animator anim;
    public float sensitivity;
    public float yOffset;
    public Vector3 camDist;
    public float collisionSensitivity = 2.5f;

    public float camInputX;
    float mouseXSmooth;
    public float camInputY;

    public static class States
    {
        public const string CamInputX = "CamInputX";
        public const string MouseX = "Mouse X";
        public const string MouseY = "Mouse Y";
    }

    void Start()
    {
        cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        camInputX = Input.GetAxis(States.MouseX);
        camInputY = Input.GetAxis(States.MouseY);
        if (Input.GetMouseButton(1))
            mouseXSmooth = Mathf.Lerp(mouseXSmooth, camInputX, Time.deltaTime * 10);
        else
            mouseXSmooth = 0;
        anim.SetFloat(States.CamInputX, mouseXSmooth, 0, Time.deltaTime * 2);

        var camPosOffset = character.transform.position;
        cameraCenter.transform.position = new Vector3(camPosOffset.x, camPosOffset.y + yOffset, camPosOffset.z);

        var rotation = cameraCenter.transform.rotation;
        rotation = Quaternion.Euler(rotation.eulerAngles.x - camInputY * sensitivity / 2,
            rotation.eulerAngles.y + camInputX * sensitivity, rotation.eulerAngles.z);
        cameraCenter.transform.rotation = rotation;

        var camPos = cam.transform;
        camPos.localPosition = camDist;

        if (Physics.Linecast(cameraCenter.transform.position, camPos.position, out _camHit))
        {
            camPos.position = _camHit.point;
            var localPos = camPos.localPosition;
            localPos = new Vector3(localPos.x, localPos.y, localPos.z + collisionSensitivity);
            camPos.localPosition = localPos;
        }

        if (camPos.localPosition.z > -0.8f)
        {
            camPos.localPosition = new Vector3(camPos.localPosition.x, camPos.localPosition.y, -0.8f);
        }
    }
}