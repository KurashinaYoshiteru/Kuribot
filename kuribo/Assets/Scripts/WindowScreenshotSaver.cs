using UnityEngine;
using uWindowCapture;
using System.IO;

public class WindowScreenshotSaver : MonoBehaviour
{
    private UwcWindowTexture windowTexture;
    public Texture2D newTexture;

    void Start()
    {
        windowTexture = GetComponent<UwcWindowTexture>();
        if (windowTexture == null)
        {
            Debug.LogError("UwcWindowTexture ���A�^�b�`����Ă��܂���B");
        }

        // �ۑ��t�H���_���쐬
        string folderPath = GetFolderPath();
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12) && windowTexture != null && windowTexture.isValid)
        {
            SaveScreenshot();
        }
    }

    public void SaveScreenshot()
    {
        if (windowTexture == null || !windowTexture.isValid)
        {
            Debug.LogWarning("UwcWindowTexture�̓ǂݎ��Ɏ��s���܂����B");
            return;
        }

        // �ΏۃE�B���h�E�̃e�N�X�`�����擾
        Texture tex = windowTexture.window.texture;

        if (!(tex is Texture2D))
        {
            Debug.LogWarning("Texture2D �ł͂Ȃ��A�܂��͂܂��L���v�`������Ă��܂���B");
            return;
        }

        Texture2D tex2D = tex as Texture2D;

        // �`��p�̈ꎞ�I��RenderTexture���쐬
        RenderTexture rt = RenderTexture.GetTemporary(tex2D.width, tex2D.height, 0);
        Graphics.Blit(tex2D, rt); // Texture��RenderTexture�ɃR�s�[
        RenderTexture.active = rt;

        // �ǂݎ��\��Texture2D�Ƀs�N�Z���f�[�^���R�s�[
        Texture2D readableTex = new Texture2D(tex2D.width, tex2D.height, TextureFormat.RGBA32, false);
        readableTex.ReadPixels(new Rect(0, 0, tex2D.width, tex2D.height), 0, 0);
        readableTex.Apply();

        // RenderTexture�����
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // �㉺���]�����iReadPixels�͏㉺�t�ɂȂ邽�߁j
        readableTex = FlipTextureVertically(readableTex);

        // �ۑ���p�X���\�z
        string path = Path.Combine(GetFolderPath(), "ScreenShot.png");

        // PNG�Ƃ��ăG���R�[�h���ĕۑ�
        byte[] pngData = readableTex.EncodeToPNG();
        File.WriteAllBytes(path, pngData);

        Debug.Log($"�X�N���[���V���b�g��ۑ����܂���: {path}");

        newTexture = readableTex;
    }

    // Texture2D���㉺���]������֐�
    Texture2D FlipTextureVertically(Texture2D original)
    {
        int width = original.width;
        int height = original.height;

        // �����T�C�Y�ŐV����Texture2D���쐬
        Texture2D flipped = new Texture2D(width, height, original.format, false);

        // �㉺��1���C��������ւ��ăR�s�[
        for (int y = 0; y < height; y++)
        {
            flipped.SetPixels(0, y, width, 1, original.GetPixels(0, height - y - 1, width, 1));
        }

        flipped.Apply();
        return flipped;
    }

    //�ۑ���p�X�̎擾�֐�
    string GetFolderPath()
    {
        string folder = Path.Combine(Application.streamingAssetsPath, "ScreenShots");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder); // �t�H���_���Ȃ���΍쐬
        }
        return folder;
    }
}
