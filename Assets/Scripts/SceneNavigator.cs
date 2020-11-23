//used to help move around a scene for designing levels
using UnityEngine;

public class SceneNavigator : MonoBehaviour
{
    public float moveSpeed = 10.0f; //can change in editor
    public float rotationSpeed = 100.0f; //can change in editor


    void Start()
    {
        //meant to be empty
    }

    void Update()
    {
        float translation = Input.GetAxis("Vertical") * moveSpeed; //up and down
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed; //left and right 

        //cap the inputs
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        //move along the z axis
        transform.Translate(0, 0, translation);

        //rotate around the y axis
        transform.Rotate(0, rotation, 0);
    }
}
