using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SmartController
{
    internal class Server
    {
        readonly IPAddress listenIP;
        readonly int listenPort;

        public Server(string listenIP, int listenPort)
        {
            this.listenIP = IPAddress.Parse(listenIP);
            this.listenPort = listenPort;
        }

        /// <summary>
        /// 何かしらの受信を待ちます。受信成功時trueを返します。
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> ConnectCheckAsync()
        {
            return await Task.Run(() =>
            {
                try
                {

                    TcpListener listener = new TcpListener(listenIP, listenPort);
                    listener.Start();
                    Console.WriteLine("ConnectCheck.....");
                    Console.WriteLine($"ip : {listenIP}  port {listenPort}");
                    //接続要求を受け入れる
                    using TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine($"Client ip : {((System.Net.IPEndPoint)(client.Client.RemoteEndPoint)).Address}  port {((System.Net.IPEndPoint)(client.Client.RemoteEndPoint)).Port} と接続.");

                    using NetworkStream ns = client.GetStream();
                    ns.ReadTimeout = 10000;
                    ns.WriteTimeout = 10000;

                    System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                    string resMsg = "";
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        byte[] bytes = new byte[256];
                        int resSize = 0;
                        do
                        {
                            resSize = ns.Read(bytes, 0, bytes.Length);
                            if (resSize == 0)
                            {
                                Console.WriteLine("Disconnected.");
                                break;
                            }
                            ms.Write(bytes, 0, resSize);
                        } while (ns.DataAvailable || bytes[resSize - 1] != '\n');
                        resMsg = encoding.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                    }
                    resMsg = resMsg.TrimEnd('\n');
                    Console.WriteLine($"Receive : {resMsg}");

                    Console.WriteLine("Communication end.");
                    listener.Stop();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        internal async Task<bool> StartAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    TcpListener listener = new TcpListener(listenIP, listenPort);
                    listener.Start();
                    Console.WriteLine("Listen.....");
                    Console.WriteLine($"ip : {listenIP}  port {listenPort}");
                    bool Continue = true;
                    while (Continue)
                    {
                        //接続要求を受け入れる
                        using TcpClient client = listener.AcceptTcpClient();
                        Console.WriteLine($"Client ip : {((System.Net.IPEndPoint)(client.Client.RemoteEndPoint)).Address}  port {((System.Net.IPEndPoint)(client.Client.RemoteEndPoint)).Port} と接続.");

                        using NetworkStream ns = client.GetStream();
                        ns.ReadTimeout = 10000;
                        ns.WriteTimeout = 10000;

                        System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                        string resMsg = "";
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                        {
                            byte[] bytes = new byte[256];
                            int resSize = 0;
                            do
                            {
                                resSize = ns.Read(bytes, 0, bytes.Length);
                                if (resSize == 0)
                                {
                                    Console.WriteLine("Disconnected.");
                                    Continue = false;
                                    break;
                                }
                                ms.Write(bytes, 0, resSize);
                            } while (ns.DataAvailable || bytes[resSize - 1] != '\n');
                            resMsg = encoding.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                        }
                        Console.WriteLine($"Receive : {resMsg.TrimEnd('\n')}");
                        var msgs = resMsg.TrimEnd('\n').Split(' ');

                        //入力処理部
                        //
                        //想定フォーマット
                        //mv {x} {y}
                        if (msgs[0] == "mv")
                        {
                            var x = (int)float.Parse(msgs[1]);
                            var y = (int)float.Parse(msgs[2]);
                            var nowpt = GetCursorPosition();
                            NativeMethods.SetCursorPos(x-nowpt.X,y-nowpt.Y);
                        }
                        else if (msgs[0] == "lc")
                        {
                            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                            Console.WriteLine("LeftClicked.");
                        }
                        else if (msgs[0] == "rc")
                        {
                            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                            Console.WriteLine("RightClicked.");
                        }
                        else if (msgs[0] == "e")
                        {
                            Continue = false;
                        }
                    }

                    Console.WriteLine("Communication end.");
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public static Point GetCursorPosition()
        {
            var pt = new NativeMethods.POINT();
            NativeMethods.GetCursorPos(out pt);
            return pt;
        }
    }
}
