using System;
using System.IO;
using UnityEngine;

public static class CmdUtil
{

    public static ByteArray GetPackage(int type, uint serverTime, byte[] body)
    {
        ByteArray data = new ByteArray();
        uint packBodyLen = 2;
        if(body != null)
            packBodyLen = (uint)body.Length + 2;
        uint len1 = (byte)packBodyLen;
        uint len2 = (byte)(packBodyLen >> 8);
        uint len3 = (byte)(packBodyLen >> 16);
        data.WriteUnsignedByte(len1);
        data.WriteUnsignedByte(len2);
        data.WriteUnsignedByte(len3);
        data.WriteUnsignedByte(0);
        //
        data.WriteUnsignedShort((uint)type);
        if(body != null)
            data.WriteBytes(body);

        return data;
    }

    public static T ParseCmd<T>(byte[] bytes)
    {
        ByteArray byteArr = new ByteArray();
        byteArr.WriteBytes(bytes);
        uint len1 = byteArr.ReadUnsignedByte();
        uint len2 = byteArr.ReadUnsignedByte();
        uint len3 = byteArr.ReadUnsignedByte();
        uint packBodyLen = len1 | len2 << 8 | len3 << 16;
        bool isCompress = byteArr.ReadUnsignedByte() == 1;
        //
        uint cmdID = byteArr.ReadUnsignedShort();
        byte[] packBody = new byte[packBodyLen - 2];
        byteArr.ReadBytes(packBody, packBody.Length);
        if (isCompress)
            packBody = GZipFileUtil.Uncompress(packBody);

        MemoryStream stream = new MemoryStream();
        stream.Write(packBody, 0, packBody.Length);
        stream.Position = 0;

        return ProtoBuf.Serializer.Deserialize<T>(stream);
    }

    public static ReceivePackage ParseCmd(ByteArray byteArr)
    {
        uint len1 = byteArr.ReadUnsignedByte();
        uint len2 = byteArr.ReadUnsignedByte();
        uint len3 = byteArr.ReadUnsignedByte();
        uint packBodyLen = len1 | len2 << 8 | len3 << 16;
        bool isCompress = byteArr.ReadUnsignedByte() == 1;
        //
         uint cmdID = byteArr.ReadUnsignedShort();
        if (packBodyLen == 2)
            return new ReceivePackage((int)cmdID);
        byte[] packBody = new byte[packBodyLen - 2];
        byteArr.ReadBytes(packBody, packBody.Length);
        if (isCompress)
            packBody = GZipFileUtil.Uncompress(packBody);
        MemoryStream stream = new MemoryStream();
        stream.Write(packBody, 0, packBody.Length);
        stream.Position = 0;
        return new ReceivePackage((int)cmdID, stream);
    }


}
