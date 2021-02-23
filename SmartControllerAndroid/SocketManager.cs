﻿using Android.App;
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
        public async Task<bool> MoveCursorAsync(float x, float y, uint moveSpeed, uint maxMoveSpeed)
        {
            // 0除算対策
            // 1 <= moveSpeed <= 10
            if (moveSpeed == 0) moveSpeed = 1;
            // 0.1 <= ratio <= 1.0 
            double ratio = moveSpeed * 0.1;

            return await SocketSendAsync($"mv {RoundingRange(x * ratio, maxMoveSpeed * 10)} {RoundingRange(y * ratio, maxMoveSpeed * 10)}");
        }

        /// <summary>
        /// サーバー停止を指す e をIpAddressのPort番に飛ばす
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ServerStopAsync()
        {
            return await SocketSendAsync("e");
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

        /// <summary>
        /// -maxMoveSpeed <= value <= maxMoveSpeed になるように value を丸める
        /// </summary>
        /// <param name="value">丸める値</param>
        /// <param name="maxMoveSpeed">上限下限の範囲</param>
        /// <returns></returns>
        private double RoundingRange(double value, uint maxMoveSpeed)
        {
            return Math.Min(maxMoveSpeed, Math.Max(value, -maxMoveSpeed));
        }
    }
}