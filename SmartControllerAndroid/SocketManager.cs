using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java.Net;
using Java.IO;

namespace SmartControllerAndroid
{
    public class SocketManager
    {
        string IpAddress;
        readonly int Port = 2001;

        public SocketManager(string ipAddress)
        {
            IpAddress = ipAddress;
        }
        /// <summary>
        /// Ping をIpAddressのPort番に飛ばす
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PingAsync()
        {
            return await SocketSendAsync("Ping");
        }

        /// <summary>
        /// 左クリックを指す lc をIpAddressのPort番に飛ばす
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LeftClickAsync()
        {
            return await SocketSendAsync("lc");
        }

        /// <summary>
        /// 右クリックを指す rc をIpAddressのPort番に飛ばす
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RightClickAsync()
        {
            return await SocketSendAsync("rc");
        }

        /// <summary>
        /// 移動を指す mv {x} {y} をIpAddressのPort番に飛ばす
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MoveCursorAsync(float x, float y,int ratio)
        {
            //0除算対策
            if (ratio <= 0) ratio = 1;
            return await SocketSendAsync($"mv {x/ratio} {y/ratio}");
        }

        /// <summary>
        /// Socket通信を行う関数
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task<bool> SocketSendAsync(string msg)
        {
            return await Task.Run(() =>
            {
                try
                {
                    InetSocketAddress address = new InetSocketAddress(IpAddress, Port);
                    Socket socket = new Socket();
                    socket.Connect(address, 3000);
                    using PrintWriter pw = new PrintWriter(socket.OutputStream, true);
                    pw.Println(msg);
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            });
        }
    }
}