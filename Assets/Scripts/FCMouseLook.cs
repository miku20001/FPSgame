using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCMouseLook : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField]private Transform charcaterTransform;  //角色正前方，跟随相机视角
    private Vector3 camraRotation;

    public float MouseSensitivity;    //鼠标灵敏度
    public Vector2 MaxminAngle;       //限制视角旋转角度

    public AnimationCurve RecoilCurve;
    public Vector2 RecoilRange;
    public float RecoilFadeOutTime = 0.3f;

    private float currentRecoilTime;
    private Vector2 currentRecoil;
    private CameraSpring cameraSpring;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = transform;
        cameraSpring = GetComponentInChildren<CameraSpring>();
    }

    // Update is called once per frame
    void Update()
    {
        float tmp_MouseX = Input.GetAxis("Mouse X");
        float tmp_MouseY = Input.GetAxis("Mouse Y");

        camraRotation.x -= tmp_MouseY * MouseSensitivity;
        camraRotation.y += tmp_MouseX * MouseSensitivity;

        CalculateRecoilOffset();
        //Debug.Log(currentRecoil);

        camraRotation.x -= currentRecoil.x;
        camraRotation.y += currentRecoil.y;

        camraRotation.x = Mathf.Clamp(camraRotation.x, MaxminAngle.x, MaxminAngle.y);

        cameraTransform.rotation = Quaternion.Euler(camraRotation.x, camraRotation.y, 0);

        charcaterTransform.rotation = Quaternion.Euler(0, camraRotation.y, 0);
    }

    private void CalculateRecoilOffset()  //计算后坐力的偏移
    {
        currentRecoilTime += Time.deltaTime;
        float tmp_RecoilFraction = currentRecoilTime / RecoilFadeOutTime;
        float tmp_RecoilValue =  RecoilCurve.Evaluate(tmp_RecoilFraction);
        currentRecoil = Vector2.Lerp(Vector2.zero, currentRecoil, tmp_RecoilValue);
    }

    public void FiringForTest()
    {
        currentRecoil += RecoilRange;
        cameraSpring.StartCameraSpring();
        currentRecoilTime = 0;
    }
}
