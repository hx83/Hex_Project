using UnityEngine;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Collections;
using com.google.zxing;
using com.google.zxing.qrcode;
using com.google.zxing.common;
using com.google.zxing.qrcode.decoder;

public class GameQRInfo
{
    public ulong id = 0;
    public string Account = string.Empty;
    public string Context = string.Empty;
    public int Type = 0;//0账号1组队2自建房间
    public string QRCode = string.Empty;
}

public class ShareTool : MonoBehaviour
{
    private static String OFFICIAL_URL = "http://www.51gods.com/"; // 官方网址
    public static ShareTool Instance = null;
    public delegate void CallBackFunc(GameQRInfo info);

    /// <summary>
    /// 二维码
    /// </summary>
    private string QRShareUrl = null;
    private string shortUrl = "";
    private Action shortAction;

    private void Awake()
    {
        Instance = this;
    }

    public Sprite CreateQRCode(string context, Texture2D gameLogo = null, int offsetX = 0, int offsetY = 0)
    {
        QRCodeWriter qrwriter = new QRCodeWriter();
        Hashtable table = new Hashtable();
        table[EncodeHintType.ERROR_CORRECTION] = ErrorCorrectionLevel.Q;
        ByteMatrix matrix = qrwriter.encode(context, com.google.zxing.BarcodeFormat.QR_CODE, 440, 440, table);

        Texture2D pic = new Texture2D(440, 440, TextureFormat.RGBA32, false);
        Color whitecolor = new Color(238 / 255f, 245 / 255f, 212 / 255f);
        for (int x = 0; x < 440; x++)
        {
            for (int y = 0; y < 440; y++)
            {
                pic.SetPixel(x, y, matrix.get_Renamed(y, x) == 0 ? Color.black : whitecolor);
            }
        }

        if (gameLogo != null)
        {
            for (int x = 0; x < gameLogo.width; x++)
            {
                for (int y = 0; y < gameLogo.height; y++)
                {
                    pic.SetPixel(x + offsetX, y + offsetY, gameLogo.GetPixel(x, y));
                }
            }
        }

        pic.Apply(false);

        Sprite sp = Sprite.Create(pic, new Rect(0, 0, pic.width, pic.height), new Vector2(0.5f, 0.5f));

        return sp;
    }

    //public string CheckShareUrl()
    //{
    //    if (QRShareUrl == null)
    //    {
    //        QRShareUrl = OFFICIAL_URL + ShareParam;
    //    }
    //    return QRShareUrl;
    //}

//    public string PersonalShareUrl
//    {
//        get
//        {
//#if UNITY_EDITOR
//            return OFFICIAL_URL + "index_mobile_share.html" + ShareParam;
//#elif UNITY_ANDROID
//            return OFFICIAL_URL + "cl/index_ms_az.html" + ShareParam;
//#elif UNITY_IPHONE
//            return OFFICIAL_URL + "cl/index_ms_ios.html" + ShareParam;
//#endif
//        }
//    }
    //public string PlayerAccount
    //{
    //        get{ return LoginManager.Account; }
    //}
    //private string ShareParam
    //{
    //    get
    //    {
    //        return "?id=" + LoginManager.playerId.ToString() + "&Account=" + LoginManager.Account;
    //    }
    //}

    public string ShortUrl
    {
        get { return shortUrl; }
    }

    //public void LongUrlToShort(Action action)
    //{
    //    shortAction = action;
    //    if (shortUrl == "")
    //    {
    //        WebClient client = new WebClient();
    //        client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
    //        client.UploadStringCompleted += UpLodStringComplete;
    //        client.UploadStringAsync(new Uri("http://api.t.sina.com.cn/short_url/shorten.json?source=1681459862&url_long=" + Uri.EscapeDataString(PersonalShareUrl)), "GET");
    //    }
    //    else
    //    {
    //        shortAction();
    //    }
    //}

    private void UpLodStringComplete(object sender, UploadStringCompletedEventArgs e)
    {
        string result = e.Result.Replace("\"", "").Replace("\\", "");
        int index = result.IndexOf("http:");
        shortUrl = result.Substring(index, result.IndexOf(",", index) - index);
        shortAction();
    }

    public bool HandleQRText(string text, CallBackFunc callBackFunc)
    {
        //CanScaner = false;
        //FindingGO.SetActive(false);

        Debug.Log("HandleQRText=>" + text);

        bool finded = false;

        try
        {
            //http://sapi.ztgame.com/s/axp?id=5556;Account=H&A500005556
            if (!string.IsNullOrEmpty(text) && text.Contains(";"))
            {
                string[] list = text.Split(';'); // http://sapi.ztgame.com/s/axp?id=5556    Account=H&A500005556
                GameQRInfo info = new GameQRInfo();
                if (list.Length > 1)
                {
                    string[] templist = list[0].Split('='); // http://sapi.ztgame.com/s/axp?id  5556
                    if (templist.Length == 2)
                    {
                        if (templist[0].Contains("id"))
                        {
                            info.id = ulong.Parse(templist[1]); // 这个正常
                        }
                    }

                    templist = list[1].Split('=');// Account    H&A500005556
                    if (templist.Length == 2)
                    {
                        if (templist[0].Equals("Account"))
                        {
                            info.Account = Uri.UnescapeDataString(templist[1]);
                            finded = true;
                        }
                    }

                    if (list.Length > 2)
                    {
                        templist = list[2].Split('=');
                        if (templist.Length == 2)
                        {
                            info.Context = templist[1];
                            info.Type = int.Parse(templist[0]);
                        }

                        if (list.Length > 3)
                        {
                            templist = list[3].Split('=');
                            if (templist.Length == 2)
                            {
                                if (templist[0].Equals("QRCode"))
                                    info.QRCode = templist[1];
                            }
                        }

                    }
                    if (finded)
                    {
                        callBackFunc(info);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        return finded;
    }

    public void ReadQRCode(Color[] clist, int width, int height, Action<string> OnOk = null)
    {
        byte[] bcolor = new byte[width * height * 3];

        for (int y = 0; y < height; y++)
        {
            int offset = y * width;
            for (int x = 0; x < width; x++)
            {
                bcolor[offset * 3 + x * 3] = (byte)(clist[x * height + y].r * 255);
                bcolor[offset * 3 + x * 3 + 1] = (byte)(clist[x * height + y].g * 255);
                bcolor[offset * 3 + x * 3 + 2] = (byte)(clist[x * height + y].b * 255);
            }
        }

        Loom.RunAsync(() =>
        {
            try
            {
                QRCodeReader qrreader = new QRCodeReader();
                RGBLuminanceSource scource = new RGBLuminanceSource(bcolor, width, height);
                HybridBinarizer binarizer = new HybridBinarizer(scource);
                BinaryBitmap bitmap = new BinaryBitmap(binarizer);
                Result result = qrreader.decode(bitmap);
                Loom.QueueOnMainThread(() => { OnOk(result.Text); });
            }
            catch (Exception ex)
            {

            }
        });
    }
}