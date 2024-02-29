using Google.Protobuf;
using SocketGameProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Message
{
    private byte[] buffer = new byte[1024];
    private int startIndex;
    public byte[] Buffer { get { return buffer; } }
    public int StartIndex { get { return startIndex; } }
    /// <summary>
    /// ʣ�೤��
    /// </summary>
    public int RemSize { get { return buffer.Length - startIndex; } }
    /// <summary>
    /// ����Buffer�е���Ϣ �����������Pack����ص���������
    /// </summary>
    /// <param name="len"></param>
    /// <param name="HandleRequest"></param>
    public void ReadBuffer(int len, Action<MainPack> HandleResponse)
    {
        // ��������Ϊ����buffer���� ���ں�������buffer
        startIndex += len;
        while (true)
        {
            // ���Ȳ����ͷ ��Ϣ��Ч
            if (startIndex <= 4)
                return;
            // buffer�� 0 ��ʼ��ȡ4���ֽ�(4 * 8 = 32����λ) ת��Ϊʮ���� ��body����
            int count = BitConverter.ToInt32(buffer, 0);

            // ���startIndex������һ����Ϣ�� ��ͷ+����
            // ����������Ч��Ϣ��buffer��
            if (startIndex >= (count + 4))
            {
                // ʹ��Protocol������Ϣ���� �Ӱ��忪ʼ���� ����Ϊcount
                MainPack mainPack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 4, count);
                HandleResponse(mainPack);
                // �ƶ�ʣ������ݵ��������Ŀ�ʼ��
                Array.Copy(buffer, count + 4, buffer, 0, startIndex - count - 4);
                // ������������ȥ�Ѿ����������Ϣ����
                startIndex -= (count + 4);
            }
            else// ����break
                break;
        }
    }
    /// <summary>
    /// ��packת��ΪTCPЭ�鷢������Ҫ������
    /// </summary>
    /// <param name="pack"></param>
    /// <returns></returns>
    public static byte[] PackToData(MainPack pack)
    {
        // ���� ��packת��Ϊ����
        byte[] body = pack.ToByteArray();
        // ��ͷ ��body�ĳ��ȴ����ͷ ռ4�ֽ�
        byte[] head = BitConverter.GetBytes(body.Length);
        // ƴ�Ӻ󷵻�
        return head.Concat(body).ToArray();
    }
    /// <summary>
    /// ��packת��ΪUDPЭ�鷢������Ҫ������
    /// </summary>
    /// <param name="pack"></param>
    /// <returns></returns>
    public static byte[] PackToDataUDP(MainPack pack)
    {
        // UDP�������Ҫ���ְ�ͷ���壬������ճ�������� ��ֱ��ת��
        return pack.ToByteArray();
    }
}
