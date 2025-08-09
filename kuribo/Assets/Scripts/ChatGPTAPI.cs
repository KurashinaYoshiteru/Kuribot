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
    //要求パラメーター用変数
    public string apiKey = ""; //Enter your API KEY
    public string prompt = "次の対話履歴とスクリーンショットを見て、何か答えてください。なお、スクリーンショット内のデスクトップマスコットは無視してください。\\n\\n# 対話履歴\\n";
    private string model = "gpt-4o-mini";
    private string text = "";
    private string base64Data = "";
    private string endpoint = "https://api.openai.com/v1/chat/completions";

    //UI関連の変数
    private Text aiText;
    private Text userText;
    public AnimationController animationController;

    //音声認識用の変数
    public ASRManager asrManager;


    //シングルターンAPI送信処理
    public void SendSingleTurnAPI()
    {
        //音声認識を停止
        asrManager.StopASR();

        //テキストを取得
        aiText = asrManager.aiText;
        userText = asrManager.userText;

        //APIに投げる文字列
        text = prompt + asrManager.GetLatestUserText();

        //API送信開始
        Debug.Log("API呼び出しを開始します。");
        StartCoroutine("ChatCompletionRequest");
    }


    IEnumerator ChatCompletionRequest()
    {
        OpenAIChatCompletionAPI chatCompletionAPI = new OpenAIChatCompletionAPI(apiKey, endpoint);

        //スクリーンショット画像をBase64変換
        string path = Application.dataPath + "/StreamingAssets/ScreenShots/ScreenShot.png";
        byte[] screenShot = System.IO.File.ReadAllBytes(path);
        base64Data = System.Convert.ToBase64String(screenShot);

        //画像データも送信
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
            message = "正しく応答を得られませんでした。";
        }
        else if (request.Response != null)
        {
            message = request.Response.choices[0].message.content;

            //設定されているアニメーションをランダムで再生
            PlayRandomAnimation();
        }

        //応答メッセージをテキストに表示
        aiText.text += message;
        userText.text = "";

        asrManager.StartASR();
    }

    //設定されているアニメーションをランダムで再生する処理
    private void PlayRandomAnimation()
    {
        // animationClipsがnull、または要素数不足なら警告して終了
        if (animationController == null || animationController.animationClips == null || animationController.animationClips.Length < 7)
        {
            Debug.LogWarning("AnimationControllerまたはanimationClipsの設定が不正です");
            return;
        }

        // 使用可能な（nullでない）アニメーションクリップだけを対象にランダム選出
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
            Debug.LogWarning("有効なアニメーションクリップが見つかりません");
            return;
        }

        // ランダムなインデックスを取得し、該当番号を呼び出す
        int randomIndex = validIndices[UnityEngine.Random.Range(0, validIndices.Count)];
        string num = randomIndex.ToString();
        animationController.OnClickButton(num);
    }


    public class OpenAIChatCompletionAPI
    {
        //応答パラメーター用クラス定義
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
