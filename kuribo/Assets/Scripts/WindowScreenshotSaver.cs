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
            Debug.LogError("UwcWindowTexture がアタッチされていません。");
        }

        // 保存フォルダを作成
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
            Debug.LogWarning("UwcWindowTextureの読み取りに失敗しました。");
            return;
        }

        // 対象ウィンドウのテクスチャを取得
        Texture tex = windowTexture.window.texture;

        if (!(tex is Texture2D))
        {
            Debug.LogWarning("Texture2D ではない、またはまだキャプチャされていません。");
            return;
        }

        Texture2D tex2D = tex as Texture2D;

        // 描画用の一時的なRenderTextureを作成
        RenderTexture rt = RenderTexture.GetTemporary(tex2D.width, tex2D.height, 0);
        Graphics.Blit(tex2D, rt); // TextureをRenderTextureにコピー
        RenderTexture.active = rt;

        // 読み取り可能なTexture2Dにピクセルデータをコピー
        Texture2D readableTex = new Texture2D(tex2D.width, tex2D.height, TextureFormat.RGBA32, false);
        readableTex.ReadPixels(new Rect(0, 0, tex2D.width, tex2D.height), 0, 0);
        readableTex.Apply();

        // RenderTextureを解放
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // 上下反転処理（ReadPixelsは上下逆になるため）
        readableTex = FlipTextureVertically(readableTex);

        // 保存先パスを構築
        string path = Path.Combine(GetFolderPath(), "ScreenShot.png");

        // PNGとしてエンコードして保存
        byte[] pngData = readableTex.EncodeToPNG();
        File.WriteAllBytes(path, pngData);

        Debug.Log($"スクリーンショットを保存しました: {path}");

        newTexture = readableTex;
    }

    // Texture2Dを上下反転させる関数
    Texture2D FlipTextureVertically(Texture2D original)
    {
        int width = original.width;
        int height = original.height;

        // 同じサイズで新しいTexture2Dを作成
        Texture2D flipped = new Texture2D(width, height, original.format, false);

        // 上下を1ラインずつ入れ替えてコピー
        for (int y = 0; y < height; y++)
        {
            flipped.SetPixels(0, y, width, 1, original.GetPixels(0, height - y - 1, width, 1));
        }

        flipped.Apply();
        return flipped;
    }

    //保存先パスの取得関数
    string GetFolderPath()
    {
        string folder = Path.Combine(Application.streamingAssetsPath, "ScreenShots");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder); // フォルダがなければ作成
        }
        return folder;
    }
}
