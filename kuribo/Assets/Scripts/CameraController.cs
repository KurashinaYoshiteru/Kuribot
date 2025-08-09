using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{

    private Vector3 defaultPosition = new Vector3(0, 0, 0);  //�����ʒu���W
    private Vector3 defaultRotation = new Vector3(0, 0, 0);  //������]���W
    private float defaultZoom = 30.0f;                       //�����Y�[���iFOV�j

    public float moveSensitivity = 0.05f;    //���N���b�N�ŃJ�����̈ʒu�𓮂����ۂ̊��x
    public float rotateSensitivity = 10.0f;  //�E�N���b�N�ŃJ��������]������Ƃ��̊��x
    public float zoomSensitivity = 1.0f;     //�}�E�X�z�C�[���ŃJ�������Y�[��������Ƃ��̊��x

    public float positionLimitX = 0.5f;      //�J�����̈ʒu���W�̈ړ����� X��
    public float positionLimitY = 0.5f;      //�J�����̈ʒu���W�̈ړ����� Y��
    public float positionLimitZ = 0.5f;      //�J�����̈ʒu���W�̈ړ����� Z��
    public float zoomLimitMax = 60.0f;       //�J�����̃Y�[���̍ő�l�i�Y�[���A�E�g�̌��E�j
    public float zoomLimitMin = 30.0f;       //�J�����̃Y�[���̍ŏ��l�i�Y�[���C���̌��E�j

    Camera mainCamera;  //�J�����R���|�[�l���g

    public GameObject buttonsUI;
    public GameObject messageWindowUI;

    private bool isActiveButtonsUI = true;


    //�Q�[�����s���Ɉ�x�����Ă΂��֐�
    void Start()
    {
        //�q�I�u�W�F�N�g����J�������擾
        mainCamera = GetComponentInChildren<Camera>();
    }

    //���t���[���Ă΂��֐�
    void Update()
    {
        //�E�N���b�N�����Ȃ���h���b�O�F�J��������]������
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(Input.GetAxis("Mouse Y") * rotateSensitivity, Input.GetAxis("Mouse X") * rotateSensitivity, 0);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }

        //��Ctrl�L�[�����Ȃ���}�E�X�z�C�[���F�J�����̃Y�[���i��O�ɉ񂷂ƃA�E�g�A���ɂ���ƃC���j
        //if (Input.GetMouseButton(2))
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            //�J�����̎���p�ύX
            mainCamera.fieldOfView -= Input.mouseScrollDelta.y * zoomSensitivity;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, zoomLimitMax, zoomLimitMin);

            //�{�^��UI�̃T�C�Y�ύX
            buttonsUI.GetComponent<RectTransform>().localScale = new Vector2(30 / mainCamera.fieldOfView, 30 / mainCamera.fieldOfView);
            buttonsUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300 + (mainCamera.fieldOfView - 30) * 5, 135 - (mainCamera.fieldOfView - 30) * 3);

            //���b�Z�[�W�E�B���h�EUI�̃T�C�Y�ύX
            messageWindowUI.GetComponent<RectTransform>().localScale = new Vector2(30 / mainCamera.fieldOfView, 30 / mainCamera.fieldOfView);
            messageWindowUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(475 - (mainCamera.fieldOfView - 30) * 7.75f, 50 - (mainCamera.fieldOfView - 30) * 1.5f);
        }


        //F1�F�{�^��UI�̕\��/��\���؂�ւ�
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (isActiveButtonsUI)
            {
                isActiveButtonsUI = false;
            }
            else
            {
                isActiveButtonsUI = true;
            }
            buttonsUI.SetActive(isActiveButtonsUI);
        }
    }
}
