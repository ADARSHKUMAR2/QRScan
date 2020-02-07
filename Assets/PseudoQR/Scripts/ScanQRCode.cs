using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using ZXing;

public class ScanQRCode : MonoBehaviour
{
    public Text result_value;

    WebCamTexture webCamTexture;
    Material quadMat;
    Texture origTexture;
    string resultText;
    const float SCAN_FREQUENCY = .5f;

    void Start()
    {
        webCamTexture = new WebCamTexture(4096, 4096);
        var devices = WebCamTexture.devices;
        webCamTexture.deviceName = devices[0].name;
        quadMat = GetComponent<Renderer>().material;
        origTexture = quadMat.mainTexture;
        quadMat.mainTexture = webCamTexture;
        webCamTexture.Play();
        StartCoroutine(ScanCoroutine());
    }
    void Update()

    {
        transform.rotation = Quaternion.AngleAxis(webCamTexture.videoRotationAngle, -Vector3.forward);

        var screenAspect = Screen.width / (float)Screen.height;
        var webCamAspect = webCamTexture.width / (float)webCamTexture.height;

        var rot90 = (webCamTexture.videoRotationAngle / 90) % 2 != 0;

        if (rot90)
            webCamAspect = 1.0f / webCamAspect;

        float sx, sy;
        if (webCamAspect < screenAspect)
        {
            sx = webCamAspect;
            sy = 1.0f;
        }
        else
        {
            sx = screenAspect;
            sy = screenAspect / webCamAspect;
        }

        transform.localScale = rot90 ? new Vector3(sy, sx, 1) : new Vector3(sx, sy, 1);

        var mirror = webCamTexture.videoVerticallyMirrored;

        quadMat.mainTextureOffset = new Vector2(0, mirror ? 1 : 0);
        quadMat.mainTextureScale = new Vector2(1, mirror ? -1 : 1);
    }

    IEnumerator ScanCoroutine()
    {
        while (string.IsNullOrEmpty(resultText))
        {
            yield return new WaitForSeconds(SCAN_FREQUENCY);
            Scan();
        }
        quadMat.mainTexture = origTexture;
        Debug.LogError("Scan Result: " + resultText);
    }

    private void Scan()
    {
        if (webCamTexture != null && webCamTexture.width > 100)
        {
            resultText = Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
            Debug.Log(resultText);
            result_value.text = resultText;
        }
    }

    public string Decode(Color32[] colors, int width, int height)
    {
        BarcodeReader reader = new BarcodeReader();
        var result = reader.Decode(colors, width, height);

        result_value.text = resultText;
        if (result != null)
            return result.Text;
        return null;
    }

    public void GoToCreateQRScene()
    {
        SceneManager.LoadScene("CreateQRCode");
    }
}
