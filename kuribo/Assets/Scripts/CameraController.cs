using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{

    private Vector3 defaultPosition = new Vector3(0, 0, 0);  //初期位置座標
    private Vector3 defaultRotation = new Vector3(0, 0, 0);  //初期回転座標
    private float defaultZoom = 30.0f;                       //初期ズーム（FOV）

    public float moveSensitivity = 0.05f;    //左クリックでカメラの位置を動かす際の感度
    public float rotateSensitivity = 10.0f;  //右クリックでカメラを回転させるときの感度
    public float zoomSensitivity = 1.0f;     //マウスホイールでカメラをズームさせるときの感度

    public float positionLimitX = 0.5f;      //カメラの位置座標の移動制限 X軸
    public float positionLimitY = 0.5f;      //カメラの位置座標の移動制限 Y軸
    public float positionLimitZ = 0.5f;      //カメラの位置座標の移動制限 Z軸
    public float zoomLimitMax = 60.0f;       //カメラのズームの最大値（ズームアウトの限界）
    public float zoomLimitMin = 30.0f;       //カメラのズームの最小値（ズームインの限界）

    Camera mainCamera;  //カメラコンポーネント

    public GameObject buttonsUI;
    public GameObject messageWindowUI;

    private bool isActiveButtonsUI = true;


    //ゲーム実行時に一度だけ呼ばれる関数
    void Start()
    {
        //子オブジェクトからカメラを取得
        mainCamera = GetComponentInChildren<Camera>();
    }

    //毎フレーム呼ばれる関数
    void Update()
    {
        //右クリック押しながらドラッグ：カメラを回転させる
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(Input.GetAxis("Mouse Y") * rotateSensitivity, Input.GetAxis("Mouse X") * rotateSensitivity, 0);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }

        //左Ctrlキー押しながらマウスホイール：カメラのズーム（手前に回すとアウト、奥にするとイン）
        //if (Input.GetMouseButton(2))
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            //カメラの視野角変更
            mainCamera.fieldOfView -= Input.mouseScrollDelta.y * zoomSensitivity;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, zoomLimitMax, zoomLimitMin);

            //ボタンUIのサイズ変更
            buttonsUI.GetComponent<RectTransform>().localScale = new Vector2(30 / mainCamera.fieldOfView, 30 / mainCamera.fieldOfView);
            buttonsUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300 + (mainCamera.fieldOfView - 30) * 5, 135 - (mainCamera.fieldOfView - 30) * 3);

            //メッセージウィンドウUIのサイズ変更
            messageWindowUI.GetComponent<RectTransform>().localScale = new Vector2(30 / mainCamera.fieldOfView, 30 / mainCamera.fieldOfView);
            messageWindowUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(475 - (mainCamera.fieldOfView - 30) * 7.75f, 50 - (mainCamera.fieldOfView - 30) * 1.5f);
        }


        //F1：ボタンUIの表示/非表示切り替え
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
