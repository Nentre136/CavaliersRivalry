using Google.Protobuf;
using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Tool
{
    class Message
    {
        private byte[] buffer = new byte[1024];
        private int startIndex;
        public byte[] Buffer { get { return buffer; } }
        public int StartIndex { get { return startIndex; } }
        /// <summary>
        /// 剩余长度
        /// </summary>
        public int RemSize { get { return buffer.Length - startIndex; } }

        /// <summary>
        /// 解析Buffer中的消息 并将解析后的Pack传入回调函数调用
        /// </summary>
        /// <param name="len"></param>
        /// <param name="HandleRequest"></param>
        public void ReadBuffer(int len,Action<MainPack> HandleRequest)
        {
            // 索引更新为整个buffer长度 用于涵盖整个buffer
            startIndex += len;
            while (true)
            {
                // 长度不足包头 消息无效
                if (startIndex <= 4)
                    return;
                // buffer从 0 开始读取4个字节(4 * 8 = 32比特位) 转化为十进制 即body长度
                int count = BitConverter.ToInt32(buffer, 0);

                // 如果startIndex还大于一条消息的 包头+包体
                // 代表还存在有效信息于buffer中
                if (startIndex >= (count + 4))
                {
                    // 使用Protocol解析消息包体 从包体开始解析 长度为count
                    MainPack mainPack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 4, count);
                    HandleRequest(mainPack);
                    // 移动剩余的数据到缓冲区的开始处
                    Array.Copy(buffer, count + 4, buffer, 0, startIndex - count - 4);
                    // 更新索引，减去已经处理过的消息长度
                    startIndex -= (count + 4);
                }
                else// 否则break
                    break;
            }
        }
        /// <summary>
        /// 将pack转化为TCP协议发送所需要的数据
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public static byte[] PackToData(MainPack pack)
        {
            // 包体 将pack转化为包体
            byte[] body = pack.ToByteArray();
            // 包头 将body的长度存入包头 占4字节
            byte[] head = BitConverter.GetBytes(body.Length);
            // 拼接后返回
            return head.Concat(body).ToArray();
        }
        /// <summary>
        /// 将pack转化为UDP协议发送所需要的数据
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public static byte[] PackToDataUDP(MainPack pack)
        {
            // UDP打包不需要区分包头包体，不存在粘包等问题 可直接转换
            return pack.ToByteArray();
        }
    }
}
