using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraRotation : MonoBehaviour
{
    public Transform CameraTransform;
    private bool flag; 
    // Start is called before the first frame update
    void Start()
    {
        CameraTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            flag = !flag;
        }

        if (flag)
        {
            CameraTransform.localRotation = Quaternion.Slerp(CameraTransform.localRotation,
                Quaternion.Euler(new Vector3(5, 5, 0)), Time.deltaTime * 10);
        }
        else
        {
            CameraTransform.localRotation = Quaternion.Slerp(CameraTransform.localRotation,
                Quaternion.Euler(new Vector3(0, 0, 0)), Time.deltaTime * 10);
        }
    }
}
