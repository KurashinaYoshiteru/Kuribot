using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Whisper.Samples;

public class ChatGPTAPI : MonoBehaviour
{
    //�v���p�����[�^�[�p�ϐ�
    public string apiKey = ""; //Enter your API KEY
    public string prompt = "���̑Θb�����ƃX�N���[���V���b�g�����āA���������Ă��������B�Ȃ��A�X�N���[���V���b�g���̃f�X�N�g�b�v�}�X�R�b�g�͖������Ă��������B\\n\\n# �Θb����\\n";
    private string model = "gpt-4o-mini";
    private string text = "";
    private string base64Data = "";
    private string endpoint = "https://api.openai.com/v1/chat/completions";

    //UI�֘A�̕ϐ�
    private Text aiText;
    private Text userText;
    public AnimationController animationController;

    //�����F���p�̕ϐ�
    public ASRManager asrManager;


    //�V���O���^�[��API���M����
    public void SendSingleTurnAPI()
    {
        //�����F�����~
        asrManager.StopASR();

        //�e�L�X�g���擾
        aiText = asrManager.aiText;
        userText = asrManager.userText;

        //API�ɓ����镶����
        text = prompt + asrManager.GetLatestUserText();

        //API���M�J�n
        Debug.Log("API�Ăяo�����J�n���܂��B");
        StartCoroutine("ChatCompletionRequest");
    }


    IEnumerator ChatCompletionRequest()
    {
        OpenAIChatCompletionAPI chatCompletionAPI = new OpenAIChatCompletionAPI(apiKey, endpoint);

        //�X�N���[���V���b�g�摜��Base64�ϊ�
        string path = Application.dataPath + "/StreamingAssets/ScreenShots/ScreenShot.png";
        byte[] screenShot = System.IO.File.ReadAllBytes(path);
        base64Data = System.Convert.ToBase64String(screenShot);

        //�摜�f�[�^�����M
        string requestData =
            "{" +
            "   \"model\": \"" + model + "\", " +
            "   \"messages\": [" +
            "   {" +
            "       \"role\": \"user\"," +
            "       \"content\": [" +
            "       {" +
            "           \"type\": \"text\"," +
            "           \"text\": \"" + text + "\"" +
            "       }," +
            "       {" +
            "           \"type\": \"image_url\"," +
            "           \"image_url\": " +
            "           {" +
            "               \"url\": \"data:image/png;base64," + base64Data + "\"" +
            "           }" +
            "       }]" +
            "   }]," +
            "   \"max_tokens\": 300" +
            "}";

        OpenAIChatCompletionAPI.RequestHandler request = chatCompletionAPI.CreateCompletionRequest(requestData);

        yield return request.Send();

        asrManager.cloneAiRowObject.SetActive(true);
        asrManager.cloneUserRowObject.SetActive(true);

        string message = "";
        if (request.Error != null)
        {
            message = "�����������𓾂��܂���ł����B";
        }
        else if (request.Response != null)
        {
            message = request.Response.choices[0].message.content;

            //�ݒ肳��Ă���A�j���[�V�����������_���ōĐ�
            PlayRandomAnimation();
        }

        //�������b�Z�[�W���e�L�X�g�ɕ\��
        aiText.text += message;
        userText.text = "";

        asrManager.StartASR();
    }

    //�ݒ肳��Ă���A�j���[�V�����������_���ōĐ����鏈��
    private void PlayRandomAnimation()
    {
        // animationClips��null�A�܂��͗v�f���s���Ȃ�x�����ďI��
        if (animationController == null || animationController.animationClips == null || animationController.animationClips.Length < 7)
        {
            Debug.LogWarning("AnimationController�܂���animationClips�̐ݒ肪�s���ł�");
            return;
        }

        // �g�p�\�ȁinull�łȂ��j�A�j���[�V�����N���b�v������ΏۂɃ����_���I�o
        List<int> validIndices = new List<int>();
        for (int i = 1; i <= 6; i++)
        {
            if (animationController.animationClips.Length > i && animationController.animationClips[i] != null)
            {
                validIndices.Add(i);
            }
        }

        if (validIndices.Count == 0)
        {
            Debug.LogWarning("�L���ȃA�j���[�V�����N���b�v��������܂���");
            return;
        }

        // �����_���ȃC���f�b�N�X���擾���A�Y���ԍ����Ăяo��
        int randomIndex = validIndices[UnityEngine.Random.Range(0, validIndices.Count)];
        string num = randomIndex.ToString();
        animationController.OnClickButton(num);
    }


    public class OpenAIChatCompletionAPI
    {
        //�����p�����[�^�[�p�N���X��`
        [System.Serializable]
        public class ResponseData
        {
            public string id;
            public string @object;
            public int created;
            public string model;
            public Usage usage;
            public List<Choice> choices;
        }

        [System.Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }

        [System.Serializable]
        public class Choice
        {
            public Message message;
            public string finish_reason;
            public int index;
        }

        [System.Serializable]
        public class Message
        {
            public string role;
            public string content;
        }


        string _endpoint;
        string _apiKey;


        public OpenAIChatCompletionAPI(string apiKey, string endpoint)
        {
            this._apiKey = apiKey;
            this._endpoint = endpoint;
        }

        public RequestHandler CreateCompletionRequest(string requestData)
        {
            Debug.Log("requestData : " + requestData);

            byte[] data = System.Text.Encoding.UTF8.GetBytes(requestData);

            var request = new UnityWebRequest(_endpoint, "POST");
            request.SetRequestHeader("Authorization", $"Bearer {_apiKey}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new RequestHandler(request);
        }

        public class RequestHandler
        {
            public bool IsCompleted { get; private set; }
            public bool IsError => Error != null;
            public string Error { get; private set; }
            public ResponseData Response { get; private set; }

            UnityWebRequest request;

            public RequestHandler(UnityWebRequest request)
            {
                this.request = request;
            }

            public IEnumerator Send()
            {
                using (request)
                {
                    yield return request.SendWebRequest();

                    Debug.Log("request.responseCode : " + request.responseCode);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Error = "[OpenAIChatCompletionAPI] " + request.error + "\n\n" + request.downloadHandler.text;
                    }
                    else
                    {
                        Response = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
                    }
                }
            }
        }
    }
}
