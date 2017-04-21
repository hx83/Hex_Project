using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using usercmd;
/// <summary>
/// ACE
/// HTTP请求
/// </summary>
public class HttpManager
{
    public const string FORM_CMD_KEY = "c";
    public static string ip;
    public static string port;
    public static Dictionary<string, string> cookies;
    private static Dictionary<WWWForm, int> _cmdFormDic;

    public static void Request(WWWForm form, EnumHTTPHead head = EnumHTTPHead.Message)
    {
        MainEntry.Instance.StartCoroutine(Send(form, head));
    }

    public static WWWForm CreateWWWForm(MsgType cmd)
    {
        WWWForm form = new WWWForm();
        form.AddField(FORM_CMD_KEY, (int)cmd);
        if (_cmdFormDic == null)
            _cmdFormDic = new Dictionary<WWWForm, int>();
        _cmdFormDic.Add(form, (int)cmd);
        return form;
    }

    private static IEnumerator Send(WWWForm form, EnumHTTPHead head)
    {
        string url = "http://" + ip + ":" + port + "/" + GetHeadString(head);
        WWW www = new WWW(url, form.data, cookies);
        //Debug.Log("request http url " + url + " cmdID, " + _cmdFormDic[form]);
        yield return www;
        int cmdID = _cmdFormDic[form];
        _cmdFormDic.Remove(form);
        if (www.error != null)
        {
            Debug.Log("serch fail...");
        }
        else if (www.isDone)
        {
            if (www.bytes.Length == 0)
            {
                HttpEvent evt = new HttpEvent(cmdID.ToString());
                _dispatcher.DispatchEventInstance(evt);
                yield break;
            }
            ByteArray receiveBytes = new ByteArray();
            receiveBytes.WriteBytes(www.bytes);

            ReceivePackage pack = CmdUtil.ParseCmd(receiveBytes);
            if (pack.CmdID == (int)MsgType.ErrorMsg)
            {
                RetErrorMsg errorMsg = ProtoBuf.Serializer.Deserialize<RetErrorMsg>(pack.BodyStream);
                if (errorMsg.RetCode == 0)
                {
                    Debug.Log("response http url success" + url);
                    HttpEvent evt = new HttpEvent(cmdID.ToString());
                    _dispatcher.DispatchEventInstance(evt);
                }
                else
                {
                    if (LoginServerErrorCode.ErrorCodeName == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("用户名最多为4个汉字或8个英文字符");
                    }
                    else if (LoginServerErrorCode.ErrorCodeAuthErr == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("验证码错误，还可以输入" + errorMsg.Params + "次");
                    }
                    else if (LoginServerErrorCode.ErrorCodeDb == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("设置密码错误");
                    }
                    else if (LoginServerErrorCode.ErrorCodePass == errorMsg.RetCode)
                    {
                        if (cmdID == (int)MsgType.CheckOldPassword)
                        {
                            PopManager.ShowSimpleItem("密码错误");
                        }
                        else
                        {
                            PopManager.ShowSimpleItem("账号或密码错误");
                        }
                    }
                    else if (LoginServerErrorCode.ErrorCodeParam == errorMsg.RetCode)
                    {
                        if (cmdID == (int)MsgType.ChangePasswd || cmdID == (int)MsgType.SetPasswd || cmdID == (int)MsgType.CheckOldPassword)
                        {
                            PopManager.ShowSimpleItem("密码复杂度不够");
                        }
                        else if (cmdID == (int)MsgType.AuthCode)
                        {
                            PopManager.ShowSimpleItem("获取验证码过于频繁");
                            HttpEvent evt = new HttpEvent(MsgType.ErrorMsg.ToString());
                            _dispatcher.DispatchEventInstance(evt);
                        }
                        else
                        {
                            PopManager.ShowSimpleItem("参数错误");
                        }
                    }
                    else if (LoginServerErrorCode.ErrorCodeIsBind == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("手机号已被绑定");
                    }
                    else if (LoginServerErrorCode.ErrorCodeMsgErr == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("短信服务器出错");
                    }
                    else if (LoginServerErrorCode.ErrorCodeTelErr == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("手机号错误");
                    }
                    else if (LoginServerErrorCode.ErrorCodeMaxMsg == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("今天短信次数已用完");
                    }
                    else if (LoginServerErrorCode.ErrorCodeMaxErr == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("密码错误已超过最大限制");
                    }
                    else if (LoginServerErrorCode.ErrorCodeExist == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("用户名已存在");
                    }
                    else if (LoginServerErrorCode.ErrorCodeUnBind == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("未绑定手机");
                    }
                    else if (LoginServerErrorCode.ErrorCodeSetAccountTm == errorMsg.RetCode)
                    {
                        ulong leftDays = errorMsg.Params / (24 * 60 * 60);
                        if (leftDays >= 1)
                        {
                            PopManager.ShowSimpleItem("还需 " + (uint)leftDays + "天才可以修改用户名");
                        }
                        else
                        {
                            ulong leftHours = errorMsg.Params / (60 * 60);
                            if (leftHours >= 1)
                            {
                                PopManager.ShowSimpleItem("还需 " + (uint)leftHours + "小时才可以修改用户名");
                            }
                            else
                            {
                                PopManager.ShowSimpleItem("还需 " + (uint)(errorMsg.Params / 60) + "分钟才可以修改用户名");
                            }
                        }
                    }
                    else if (LoginServerErrorCode.ErrorCodeSignReward == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("签到奖励领取失败");
                    }
                    else if (LoginServerErrorCode.ErrorCodeSignIned == errorMsg.RetCode)
                    {
                        PopManager.ShowSimpleItem("已签到");
                    }
					else if(LoginServerErrorCode.ErrorCodeRoom == errorMsg.RetCode)
					{
						PopManager.ShowSimpleItem("未找到房间");
					}
					else if(LoginServerErrorCode.ErrorCodeIsFull == errorMsg.RetCode)
					{
						PopManager.ShowSimpleItem("目标房间已满");
					}
					else if(LoginServerErrorCode.ErrorCodeDb == errorMsg.RetCode)
					{
						PopManager.ShowSimpleItem("数据库错误");
					}

                    Log("http cmd error code:" + errorMsg.RetCode + " para:" + errorMsg.Params);
                }
            }
            else
            {
                Log("response http url success " + url + " CmdID:" + (MsgType)pack.CmdID);
                HttpEvent evt = new HttpEvent(pack.CmdID.ToString(), pack.BodyStream);
                _dispatcher.DispatchEventInstance(evt);
            }
        }
    }

    private static void Log(string str)
    {
        if(Global.IsDebug)
        {
            Debug.Log(str);
        }
    }

    private static string GetHeadString(EnumHTTPHead head)
    {
        string str;
        switch (head)
        {
            case EnumHTTPHead.Message:
                str = "msg";
                break;
            case EnumHTTPHead.AddFriend:
                str = "findfriend";
                break;
            case EnumHTTPHead.Share:
                str = "share";
                break;
            default:
                str = "msg";
                break;
        }
        return str;
    }

    //----------------------------------------
    //cmd Listener
    //----------------------------------------
    private static EventDispatcher _dispatcher = new EventDispatcher();

    public static bool HasCmdListener(object cmdID)
    {
        return _dispatcher.HasEventListener(((int)cmdID).ToString().ToString());
    }

    public static void AddCmdListener(object cmdID, Action<BaseEvent> listener)
    {
        _dispatcher.AddEventListener(((int)cmdID).ToString(), listener);
    }

    public static void RemoveCmdListener(object cmdID, Action<BaseEvent> listener)
    {
        _dispatcher.RemoveEventListener(((int)cmdID).ToString(), listener);
    }

    public static EventDispatcher Dispatcher
    {
        get
        {
            return _dispatcher;
        }
    }
}
public enum EnumHTTPHead
{
    Message,
    AddFriend,
    Share,
}
class LoginServerErrorCode
{
    public static uint ErrorCodeOkay = 0;  // 成功
    public static uint ErrorCodeName = 1;  // 名字非法
    public static uint ErrorCodeVersion = 2;  // 版本号错误
    public static uint ErrorCodeRoom = 3;  // 未找到房间
    public static uint ErrorCodeDb = 4;  // 数据库错误
    public static uint ErrorCodePass = 5;  // 账号或密码错误
    public static uint ErrorCodeParam = 6; // 参数错误
    public static uint ErrorCodeSession = 7;  // session错误
    public static uint ErrorCodeDecode = 8;  // 解码错误
    public static uint ErrorCodeCmd = 9; // 未知指令
    public static uint ErrorCodeExist = 10;// 账号名已存在
    public static uint ErrorCodeNotExist = 11; // 账号不存在
    public static uint ErrorCodeOffline = 12; // 玩家离线
    public static uint ErrorCodeIsFull = 13; // 目标房间已满
    public static uint ErrorCodeUnPlay = 14; // 玩家不在游戏
    public static uint ErrorCodeFollow = 15;// 关注人数满
    public static uint ErrorCodeSelf = 16; // 不能关注自己
    public static uint ErrorCodeVerify = 17; // 验证失败
    public static uint ErrorCodeTooQuick = 18; // 操作太快
    public static uint ErrorCodeBeBinded = 19; // 邮箱已绑定
    public static uint ErrorCodeInValid = 20; // 邮箱不合法
    public static uint ErrorCodeReLogin = 21; // 其它地方登录
    public static uint ErrorCodeMoney = 22; // 彩豆不足
    public static uint ErrorCodeMicBusy = 23; // 麦上有人
    public static uint ErrorCodeVoice = 24; // 语音服务器出错
    public static uint ErrorCodeTeam = 25; // 未找到组队服务器
    public static uint ErrorCodeJoinTeam = 26; // 组队模式暂不能加入
    public static uint ErrorCodeInTeam = 27; // 已在组队游戏中
    public static uint ErrorCodeTeamFull = 28; // 队伍已满
    public static uint ErrorCodeInvFail = 29; // 组队邀请失效
    public static uint ErrorCodeLeader = 30; // 只有队长才能点开始游戏
    public static uint ErrorCodeOutTeam = 31;// 不在组队状态
    public static uint ErrorCodeTeamRoom = 32; // 未找到目标房间
    public static uint ErrorCodeNoTeam = 33; // 未找到目标队伍
    public static uint ErrorCodeUnBuy = 34; // 未购买物品
    public static uint ErrorCodeAuthErr = 35; // 验证码错误
    public static uint ErrorCodeMsgErr = 36;// 短信服务器出错
    public static uint ErrorCodeUnBind = 37; // 未绑定手机
    public static uint ErrorCodeTelErr = 38;// 手机号错误
    public static uint ErrorCodeEAdded = 39; // 已经别人加过了
    public static uint ErrorCodeEPlayNum = 40; // 游戏次数不够
    public static uint ErrorCodeIsBind = 41;// 手机号已被绑定
    public static uint ErrorCodeFindSelf = 42; // 不能找回自己
    public static uint ErrorCodeMaxMsg = 43; // 今天短信次数已用完
    public static uint ErrorCodeMaxErr = 44; // 密码错误已超过最大限制
    public static uint ErrorCodeNoWidget = 45; // 数量不足
    public static uint ErrorCodeInQRoom = 46; // 闪电战房间不能观战
    public static uint ErrorCodeNoTicket = 47; // 门票不足
    public static uint ErrorCodeNoLife = 48; // 生命数不足
    public static uint ErrorCodeEMoney = 49; // 棒棒糖不足
    public static uint ErrorCodeBuyNum = 50; // 购买次数已用完
    public static uint ErrorCodeObjNum = 51; // 材料不足
    public static uint ErrorCodeJoinRoom = 52; // 闪电战房间不能加入
    public static uint ErrorCodeBanAcc = 53; // 账号被封
    public static uint ErrorCodeLevel = 54; // 等级不够
    public static uint ErrorCodeInQuick = 55; // 闪电战暂不能加入
    public static uint ErrorCodeUseNum = 56; // 使用次数已用完
    public static uint ErrorCodeMaxFile = 57; // 文件太大
    public static uint ErrorCodeUploadFile = 58;// 文件上传失败
    public static uint ErrorCodeLauding = 59; // 已经点赞
    public static uint ErrorCodeNotLaud = 60; // 没有点赞
    public static uint ErrorCodeAudienceIng = 61; // 语音已经听过
    public static uint ErrorCodePhotoNum = 62; // 照片太多
    public static uint ErrorCodeRMsgTQuick = 63; // 留言间隔太短
    public static uint ErrorCodeSelfStar = 64; // 不能给自己点赞
    public static uint ErrorCodeLoveMe = 65; // 不能喜欢自己
    public static uint ErrorCodeFast = 66;// 操作太快
    public static uint ErrorCodeNEHero = 67;// 卡牌不存在
    public static uint ErrorCodeEWater = 68;// 圣水不足
    public static uint ErrorCodeNxValidPos = 69;// npc摆放非法位置
    public static uint ErrorCodeExistUnlock = 70; // 另一个宝箱正在解锁
    public static uint ErrorCodeNoDiamond = 71; // 宝石不足
    public static uint ErrorCodeNoGold = 72; // 金币不足
    public static uint ErrorCodeSetAccountTm = 78; // 修改用户名
    public static uint ErrorCodeSignReward = 127; // 签到奖励领取失败
    public static uint ErrorCodeSignIned = 130; // 已签到
    public static uint ErrorCodeWhiteList = 215; // 检查白名单
}