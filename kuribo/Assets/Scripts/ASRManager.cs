using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;
using System.IO;
using System.Text;
using System;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using uWindowCapture;
using Unity.VisualScripting;


namespace Whisper.Samples
{
    public class ASRManager : MonoBehaviour
    {
        //音声認識に使用するWhisper関連クラス
        private WhisperManager whisper;
        private MicrophoneRecord microphoneRecord;
        private WhisperStream _stream;

        //UI関連の変数
        [Header("UI")] 
        public ScrollRect scrollRect;
        public GameObject userRowObject;
        public GameObject aiRowObject;
        [System.NonSerialized]
        public GameObject cloneAiRowObject;
        [System.NonSerialized]
        public UnityEngine.UI.Text aiText;
        [System.NonSerialized]
        public GameObject cloneUserRowObject;
        [System.NonSerialized]
        public UnityEngine.UI.Text userText;
        [System.NonSerialized]
        public UnityEngine.UI.Image userImage;

        //ユーザーの発話内容を一時保存しておくための変数
        private string currentUserText = "";

        //スクリーンショット用クラス
        public WindowScreenshotSaver windowScreenshotSaver;


        private async void Start()
        {
            //Whisper関連クラスの初期化
            whisper = transform.GetComponent<WhisperManager>();
            microphoneRecord = transform.GetComponent<MicrophoneRecord>();
            _stream = await whisper.CreateStream(microphoneRecord);
            _stream.OnResultUpdated += OnResult;

            //UIの初期化
            userText = userRowObject.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>();
            aiText = aiRowObject.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>();
            userImage = userRowObject.transform.GetChild(1).transform.GetChild(1).transform.GetComponent<UnityEngine.UI.Image>();

            //実行と同時に音声認識開始
            StartASR();

            //ユーザーのメッセージパネルを有効化
            userRowObject.SetActive(true);

            //スクロールの初期位置を下にする
            scrollRect.verticalNormalizedPosition = 0;
        }

        //認識音声をアップデート
        private void OnResult(string result)
        {
            userText.text = result;
            scrollRect.verticalNormalizedPosition = 0;
        }

        //音声認識停止時の処理
        public void StopASR()
        {
            //スクリーンショット保存
            windowScreenshotSaver.SaveScreenshot();

            //音声認識停止
            microphoneRecord.StopRecord();

            //入力音声を一時保存
            currentUserText = userText.text;


            //API送信するスクリーンショットをリサイズして表示
            Texture2D screenShotTexture = windowScreenshotSaver.newTexture;
            float resizeAspect = 245.0f / 1920.0f;
            int oldWidth = screenShotTexture.width;
            int newWidth = (int)(oldWidth * resizeAspect);
            int oldHeight = screenShotTexture.height;
            int newHeight = (int)(oldHeight * resizeAspect);
            var resizedTexture = new Texture2D(newWidth, newHeight);
            Graphics.ConvertTexture(screenShotTexture, resizedTexture);
            Sprite screenShotSprite = Sprite.Create(resizedTexture, new Rect(0, 0, newWidth, newHeight), Vector2.zero);
            userImage.preserveAspect = false;
            userImage.sprite = screenShotSprite;

            userRowObject.transform.position = userRowObject.transform.position - new Vector3(0, -20, 0);

            //AI用の列を複製
            cloneAiRowObject = Instantiate(aiRowObject, aiRowObject.transform.position, Quaternion.identity, aiRowObject.transform.parent);
            cloneAiRowObject.SetActive(false);
            aiText = cloneAiRowObject.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>();

            //ユーザー用の列を複製
            cloneUserRowObject = Instantiate(userRowObject, userRowObject.transform.position, Quaternion.identity, userRowObject.transform.parent);
            cloneUserRowObject.SetActive(false);
            userText = cloneUserRowObject.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>();
            userImage = cloneUserRowObject.transform.GetChild(1).transform.GetChild(1).transform.GetComponent<UnityEngine.UI.Image>();
            userImage.sprite = null;

            scrollRect.verticalNormalizedPosition = 0;

        }

        //音声認識開始処理
        public void StartASR()
        {
            _stream.StartStream();
            microphoneRecord.StartRecord();
        }

        //入力音声を返す処理
        public String GetLatestUserText()
        {
            return currentUserText;
        }
    }
}
