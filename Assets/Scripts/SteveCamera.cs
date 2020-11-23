//camera that follows the main character (Steve)
using UnityEngine;

public class SteveCamera : MonoBehaviour
{
    //variables 
    public float cameraSensitivity = 1.5f; //default cameraSensitivity value, can be changed in the editor
    public float cameraSmoothing = 1.5f; //default cameraSmoothing value, can be changed in the editor

    private Vector2 mouseLook;
    private Vector2 smoothCam;

    private GameObject Steve; //the main character


    void Start()
    {
        Steve = transform.parent.gameObject; //make Steve the parent of the camera
    }

    void Update()
    {
        //meant to be empty
    }

    void LateUpdate()
    {
        //camera movement
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        md = Vector2.Scale(md, new Vector2(cameraSensitivity * cameraSmoothing, cameraSensitivity * cameraSmoothing));
        smoothCam.x = Mathf.Lerp(smoothCam.x, md.x, 1f / cameraSmoothing);
        smoothCam.y = Mathf.Lerp(smoothCam.y, md.y, 1f / cameraSmoothing);
        mouseLook += smoothCam;
        //stop camera at Steves feet when looking down and at Steves head when looking up
        mouseLook.y = Mathf.Clamp(mouseLook.y, -45f, 45f);

        //camera rotation
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        Steve.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, Steve.transform.up);
    }

    //this is a first person camera so collision code is not needed
}
