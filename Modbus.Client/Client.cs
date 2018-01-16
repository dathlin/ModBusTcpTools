using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Modbus.Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }

        private void Client_Load(object sender, EventArgs e)
        {

            groupBox2.Enabled = false;
            groupBox3.Enabled = false;

            userButton2.Enabled = false;

            LogNet = new HslCommunication.LogNet.LogNetSingle(Application.StartupPath + @"\log.txt");
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            try
            {
                modBusTcpClient = new HslCommunication.ModBus.ModBusTcpClient(textBox1.Text, int.Parse(textBox2.Text));
                modBusTcpClient.LogNet = LogNet;
                
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                userButton1.Enabled = false;
                userButton2.Enabled = true;

                groupBox2.Enabled = true;
                groupBox3.Enabled = true;

                if(!modBusTcpClient.ConnectServer().IsSuccess)
                {
                    MessageBox.Show("连接服务器失败！");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("失败：" + ex.Message);
            }
        }

        private void userButton2_Click(object sender, EventArgs e)
        {
            // 重新配置
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;

            userButton2.Enabled = false;
            userButton1.Enabled = true;

            textBox1.Enabled = true;
            textBox2.Enabled = true;
        }


        private void userButton3_Click(object sender, EventArgs e)
        {
            // 发送数据
            try
            {
                HslCommunication.OperateResult<byte[]> operate = modBusTcpClient.ReadFromServerCore(
                    HslCommunication.BasicFramework.SoftBasic.HexStringToBytes(textBox3.Text));

                // 展示结果
                MessageResultShow(operate);

                textBox3.Focus();
                if(textBox3.Text.EndsWith(Environment.NewLine))
                {
                    textBox3.Text = textBox3.Text.Remove(textBox3.Text.Length - 2);
                }
                textBox3.SelectionStart = textBox3.Text.Length;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private HslCommunication.OperateResult<ushort,ushort> GetAddressAndLength()
        {
            HslCommunication.OperateResult<ushort, ushort> result = new HslCommunication.OperateResult<ushort, ushort>();
            try
            {
                result.Content1 = ushort.Parse(textBox5.Text);
                result.Content2 = ushort.Parse(textBox6.Text);
                result.IsSuccess = true;
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }


        private void userButton4_Click(object sender, EventArgs e)
        {
            // 读线圈，功能码0x01
            HslCommunication.OperateResult<ushort, ushort> result = GetAddressAndLength();
            if(result.IsSuccess)
            {
                MessageResultShowBool(modBusTcpClient.ReadCoil(result.Content1, result.Content2),result.Content2);
            }
            else
            {
                MessageBox.Show(result.ToMessageShowString());
            }
        }

        private void userButton5_Click(object sender, EventArgs e)
        {
            // 读离散
            HslCommunication.OperateResult<ushort, ushort> result = GetAddressAndLength();
            if(result.IsSuccess)
            {
                MessageResultShowBool(modBusTcpClient.ReadDiscrete(result.Content1, result.Content2),result.Content2);
            }
            else
            {
                MessageBox.Show(result.ToMessageShowString());
            }
        }

        private void userButton6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                // 读寄存器
                HslCommunication.OperateResult<ushort, ushort> result = GetAddressAndLength();
                if (result.IsSuccess)
                {
                    MessageResultShow(modBusTcpClient.ReadRegister(result.Content1, result.Content2));
                }
                else
                {
                    MessageBox.Show(result.ToMessageShowString());
                }
            }
        }

        private void userButton7_Click(object sender, EventArgs e)
        {
            // 写单个寄存器测试
            if(!ushort.TryParse(textBox8.Text,out ushort address))
            {
                MessageBox.Show("地址输入格式错误或范围超出预期！");
                textBox8.Focus();
                return;
            }

            if(!short.TryParse(textBox7.Text,out short value))
            {
                MessageBox.Show("写入值输入格式错误或范围超出预期！");
                textBox7.Focus();
                return;
            }

            for (int i = 0; i < 1; i++)
            {
                HslCommunication.OperateResult write = modBusTcpClient.WriteOneRegister(address, value);
                if (write.IsSuccess)
                {
                    MessageInfoShow("写入地址" + address + "成功");
                }
                else
                {
                    MessageBox.Show(write.ToMessageShowString());
                }
            }
        }
        private void userButton9_Click(object sender, EventArgs e)
        {
            textBox4.Clear();
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            // 支持按Enter键发送
            if(e.KeyCode == Keys.Enter)
            {
                userButton3.PerformClick();
                e.Handled = true;
            }
        }

        private void userButton8_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(textBox9.Text))
            {
                if (!short.TryParse(textBox9.Text, out short value))
                {
                    MessageBox.Show("写入值输入格式错误或范围超出预期！");
                    textBox9.Focus();
                    return;
                }
                // 特殊用途的测试
                userButton8.Enabled = false;
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(
                    ThreadBackground));
                thread.IsBackground = true;
                thread.Start(value);
            }
        
        }
        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            // 支持按Enter键发送
            if(e.KeyCode == Keys.Enter)
            {
                userButton8.PerformClick();
                textBox9.Focus();
                textBox9.SelectionStart = textBox9.Text.Length;
            }
        }
        private void ThreadBackground(object obj)
        {
            if (obj is short value)
            {
                MessageInfoShow("开始写值");
                HslCommunication.OperateResult result = modBusTcpClient.WriteOneRegister(0, value);


                MessageInfoShow("开始读取值并进行对比");
                HslCommunication.OperateResult<byte[]> read = modBusTcpClient.ReadRegister(30, 1);


                if ((read.Content[0] * 256 + read.Content[1]) == value)
                {
                    MessageInfoShow("开始清空原数据");
                    modBusTcpClient.WriteOneRegister(0, 0);
                    MessageInfoShow("操作完成");
                }

                Invoke(new Action(() =>
                {
                    userButton8.Enabled = true;
                }));
            }
        }


        /// <summary>
        /// 线程安全的显示消息文本
        /// </summary>
        /// <param name="message"></param>
        private void MessageInfoShow(string message)
        {
            if (textBox4.IsHandleCreated && textBox4.InvokeRequired)
            {
                textBox4.Invoke(new Action<string>(MessageInfoShow), message);
                return;
            }

            textBox4.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + message + Environment.NewLine);

        }

        private void MessageResultShow(HslCommunication.OperateResult<byte[]> result)
        {
            if(result.IsSuccess)
            {
                MessageInfoShow(HslCommunication.BasicFramework.SoftBasic.ByteToHexString(result.Content, ' '));
            }
            else
            {
                MessageBox.Show(result.ToMessageShowString());
            }
        }
        private void MessageResultShowBool(HslCommunication.OperateResult<byte[]> result,int length)
        {
            if(result.IsSuccess)
            {
                MessageInfoShow(GetStringFromBoolArray(HslCommunication.BasicFramework.SoftBasic.ByteToBoolArray(result.Content,length)));
            }
            else
            {
                MessageBox.Show(result.ToMessageShowString());
            }
        }

        private void MessageResultShow(HslCommunication.OperateResult result)
        {
            if(result.IsSuccess)
            {
                MessageInfoShow("写入成功");
            }
            else
            {
                MessageBox.Show(result.ToMessageShowString());
            }
        }


        private HslCommunication.ModBus.ModBusTcpClient modBusTcpClient;

        private string GetStringFromBoolArray(bool[] array)
        {
            StringBuilder sb = new StringBuilder("[");
            if (array != null)
            {
                foreach (var m in array)
                {
                    sb.Append(m);
                    sb.Append(",");
                }
            }

            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");

            return sb.ToString();
        }

        private void userButton10_Click(object sender, EventArgs e)
        {
            userButton10.Enabled = false;
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadBackground));
                thread.IsBackground = true;
                thread.Start();

        }

        private void ThreadBackground()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);

                HslCommunication.OperateResult<byte[]> result = modBusTcpClient.ReadRegister(20, 1);

                if (result.IsSuccess)
                {
                    ushort value = BitConverter.ToUInt16(result.Content, 0);
                    if (value != 0)
                    {
                        if (modBusTcpClient.WriteOneRegister(10, value).IsSuccess)
                        {
                            LogNet?.WriteDebug("地址10写入(" + value + ")成功！");
                            if (!modBusTcpClient.WriteOneRegister(10, 0).IsSuccess)
                                LogNet?.WriteDebug("地址10写入0失败！");
                            if (!modBusTcpClient.WriteOneRegister(12, 1).IsSuccess)
                                LogNet?.WriteDebug("地址12写入1失败！");
                        }
                        else
                        {
                            LogNet?.WriteDebug("地址10写入(" + value + ")失败！");
                        }
                    }
                }
                else
                {
                    LogNet?.WriteDebug("地址0读取失败！");
                }
            }
        }


        private HslCommunication.LogNet.ILogNet LogNet;

        private void userButton11_Click(object sender, EventArgs e)
        {
            modBusTcpClient.ConnectServer();
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (modBusTcpClient != null) modBusTcpClient.ConnectClose( );
        }

        private void userButton12_Click(object sender, EventArgs e)
        {
            // 压力测试
            new System.Threading.Thread(ThreadRead) { IsBackground = true }.Start();
            new System.Threading.Thread(ThreadRead) { IsBackground = true }.Start();
            new System.Threading.Thread(ThreadRead) { IsBackground = true }.Start();
            new System.Threading.Thread(ThreadRead) { IsBackground = true }.Start();
        }
        private void ThreadRead()
        {
            for (int i = 0; i < 10000; i++)
            {
                modBusTcpClient.ReadRegister(0, 5);
            }
        }
    }
}
