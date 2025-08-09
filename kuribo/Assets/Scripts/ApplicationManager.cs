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
        //�F�������o�͗p�̃t�@�C���ǂݎ��
        ReadFile(path);
    }

    //�I���{�^���N���b�N���̏���
    public void OnClickCloseAppButton()
    {
        //�F���������t�@�C���o��
        string outputText = DemoMic.GetComponent<ASRManager>().userText.text;
        WriteFile(outputText, path);

        //�A�v���I��
        Application.Quit();
    }



    //�t�@�C���o��
    void WriteFile(string txt, string path)
    {
        FileInfo fi = new FileInfo(path);
        using (StreamWriter sw = fi.AppendText())
        {
            sw.WriteLine(txt);
        }
    }

    //�t�@�C���ǂݎ��
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
