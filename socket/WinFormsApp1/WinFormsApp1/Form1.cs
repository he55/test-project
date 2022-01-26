using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        Socket socket;
        IPEndPoint iPEndPoint;
        IPEndPoint iPEndPoint2;

        public Form1()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            iPEndPoint = new IPEndPoint(IPAddress.Any, 12345);
            iPEndPoint2 = new IPEndPoint(IPAddress.Broadcast, 12345);
            socket.Bind(iPEndPoint);

        }

        private void button1_Click(object sender, EventArgs e)
        {

            Task.Run(() =>
            {
                while (true)
                {
                    byte[] buffer = new byte[100];
                    EndPoint endPoint = iPEndPoint;
                    socket.ReceiveFrom(buffer, ref endPoint);

                    string v = Encoding.UTF8.GetString(buffer);
                    this.Invoke((Action)(() => {
                        textBox1.Text += $"{v}\n";
                    }));

                    Debug.WriteLine($"ReceiveFrom: {v}");
                }
            });

        }

        private void button2_Click(object sender, EventArgs e)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            byte[] vs = Encoding.UTF8.GetBytes("dffffd");
            socket.SendTo(vs, iPEndPoint2);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            socket.Close();
        }
    }
}
