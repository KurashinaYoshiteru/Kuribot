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
        //�����F���Ɏg�p����Whisper�֘A�N���X
        private WhisperManager whisper;
        private MicrophoneRecord microphoneRecord;
        private WhisperStream _stream;

        //UI�֘A�̕ϐ�
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

        //���[�U�[�̔��b���e���ꎞ�ۑ����Ă������߂̕ϐ�
        private string currentUserText = "";

        //�X�N���[���V���b�g�p�N���X
        public WindowScreenshotSaver windowScreenshotSaver;


        private async void Start()
        {
            //Whisper�֘A�N���X�̏�����
            whisper = transform.GetComponent<WhisperManager>();
            microphoneRecord = transform.GetComponent<MicrophoneRecord>();
            _stream = await whisper.CreateStream(microphoneRecord);
            _stream.OnResultUpdated += OnResult;

            //UI�̏�����
            userText = userRowObject.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>();
            aiText = aiRowObject.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>();
            userImage = userRowObject.transform.GetChild(1).transform.GetChild(1).transform.GetComponent<UnityEngine.UI.Image>();

            //���s�Ɠ����ɉ����F���J�n
            StartASR();

            //���[�U�[�̃��b�Z�[�W�p�l����L����
            userRowObject.SetActive(true);

            //�X�N���[���̏����ʒu�����ɂ���
            scrollRect.verticalNormalizedPosition = 0;
        }

        //�F���������A�b�v�f�[�g
        private void OnResult(string result)
        {
            userText.text = result;
            scrollRect.verticalNormalizedPosition = 0;
        }

        //�����F����~���̏���
        public void StopASR()
        {
            //�X�N���[���V���b�g�ۑ�
            windowScreenshotSaver.SaveScreenshot();

            //�����F����~
            microphoneRecord.StopRecord();

            //���͉������ꎞ�ۑ�
            currentUserText = userText.text;


            //API���M����X�N���[���V���b�g�����T�C�Y���ĕ\��
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

            //AI�p�̗�𕡐�
            cloneAiRowObject = Instantiate(aiRowObject, aiRowObject.transform.position, Quaternion.identity, aiRowObject.transform.parent);
            cloneAiRowObject.SetActive(false);
            aiText = cloneAiRowObject.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>();

            //���[�U�[�p�̗�𕡐�
            cloneUserRowObject = Instantiate(userRowObject, userRowObject.transform.position, Quaternion.identity, userRowObject.transform.parent);
            cloneUserRowObject.SetActive(false);
            userText = cloneUserRowObject.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>();
            userImage = cloneUserRowObject.transform.GetChild(1).transform.GetChild(1).transform.GetComponent<UnityEngine.UI.Image>();
            userImage.sprite = null;

            scrollRect.verticalNormalizedPosition = 0;

        }

        //�����F���J�n����
        public void StartASR()
        {
            _stream.StartStream();
            microphoneRecord.StartRecord();
        }

        //���͉�����Ԃ�����
        public String GetLatestUserText()
        {
            return currentUserText;
        }
    }
}
