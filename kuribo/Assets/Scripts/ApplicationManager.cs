using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Samples;


public class ApplicationManager : MonoBehaviour
{

    string path = Application.dataPath + "/StreamingAssets/SpeechMessages/SpeechMessage.txt";
    public GameObject DemoMic;

    void Start()
    {
        //認識音声出力用のファイル読み取り
        ReadFile(path);
    }

    //終了ボタンクリック時の処理
    public void OnClickCloseAppButton()
    {
        //認識音声をファイル出力
        string outputText = DemoMic.GetComponent<ASRManager>().userText.text;
        WriteFile(outputText, path);

        //アプリ終了
        Application.Quit();
    }



    //ファイル出力
    void WriteFile(string txt, string path)
    {
        FileInfo fi = new FileInfo(path);
        using (StreamWriter sw = fi.AppendText())
        {
            sw.WriteLine(txt);
        }
    }

    //ファイル読み取り
    void ReadFile(string path)
    {
        FileInfo fi = new FileInfo(path);
        try
        {
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                string readTxt = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
