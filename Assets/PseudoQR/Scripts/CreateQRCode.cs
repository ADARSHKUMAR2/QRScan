using UnityEngine;
using UnityEngine.SceneManagement;
using ZXing;
using ZXing.QrCode;

public class CreateQRCode : MonoBehaviour
{
    Texture2D encoded;
    public string lastresult;

    void Start()
    {
        encoded = new Texture2D(256, 256);
        if (string.IsNullOrEmpty(lastresult))
            lastresult = "PSID: 30651";

        var textForEncoding = lastresult;
        if (textForEncoding != null)
        {
            var color32 = Encode(textForEncoding, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();
        }
    }

    Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(100, 100, 256, 256), encoded, ScaleMode.StretchToFill);
    }

    public void GoToScanQRScene()
    {
        SceneManager.LoadScene("ScanQRCode");
    }
}