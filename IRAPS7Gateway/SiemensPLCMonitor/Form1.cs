﻿using HslCommunication;
using HslCommunication.Profinet.Siemens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiemensPLCMonitor
{
    public partial class Form1 : Form
    {
        private SiemensS7Net plc = null;
        private SiemensPLCS plcType = SiemensPLCS.S1500;
        private bool isConnected = false;

        public Form1()
        {
            InitializeComponent();

            edtIPAddress.Text = ReadParam("Address");
            edtPort.Text = ReadParam("Port") == "" ? "102" : ReadParam("Port");
            edtSlot.Text = ReadParam("Slot") == "" ? "0" : ReadParam("Slot");
            edtRack.Text = ReadParam("Rack") == "" ? "0" : ReadParam("Rack");
        }

        private string ReadParam(string key)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return ConfigurationManager.AppSettings[key];
            }
            else
            {
                return "";
            }
        }

        private void WriteParam(string key, string value)
        {
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            plc = new SiemensS7Net(plcType);
            try
            {
                plc.IpAddress = edtIPAddress.Text;
                if (int.TryParse(edtPort.Text, out int port))
                {
                    plc.Port = port;
                }
                else
                {
                    plc.Port = 102;
                    edtPort.Text = "102";
                }
                if (byte.TryParse(edtSlot.Text, out byte slot))
                {
                    plc.Slot = slot;
                }
                else
                {
                    plc.Slot = 0;
                    edtSlot.Text = "0";
                }
                if (byte.TryParse(edtRack.Text, out byte rack))
                {
                    plc.Rack = rack;
                }
                else
                {
                    plc.Rack = 0;
                    edtRack.Text = "0";
                }

                OperateResult connect = plc.ConnectServer();
                if (connect.IsSuccess)
                {
                    isConnected = true;
                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = true;

                    edtIPAddress.Enabled = false;
                    edtPort.Enabled = false;
                    edtRack.Enabled = false;
                    edtSlot.Enabled = false;
                }
                else
                {
                    MessageBox.Show($"({connect.ErrorCode}){connect.Message}");
                    isConnected = false;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                isConnected = false;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                plc.ConnectClose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            isConnected = false;

            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            edtIPAddress.Enabled = true;
            edtPort.Enabled = true;
            edtRack.Enabled = true;
            edtSlot.Enabled = true;
        }

        private void btnReadBool_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                OperateResult<bool> rlt = plc.ReadBool($"DB{edtDBNumber.Text}.{edtOffset.Text}");
                if (rlt.IsSuccess)
                {
                    edtText.Text = rlt.Content.ToString();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnWriteBool_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                OperateResult rlt =
                    plc.Write(
                        $"DB{edtDBNumber.Text}.{edtOffset.Text}",
                        edtText.Text.ToUpper() == "TRUE" || edtText.Text == "1");

                if (rlt.IsSuccess)
                {
                    btnReadBool.PerformClick();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnReadByte_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                OperateResult<byte> rlt = plc.ReadByte($"DB{edtDBNumber.Text}.{edtOffset.Text}");
                if (rlt.IsSuccess)
                {
                    edtText.Text = rlt.Content.ToString();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnWriteByte_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                byte.TryParse(edtText.Text, out byte data);
                OperateResult rlt =
                    plc.Write(
                        $"DB{edtDBNumber.Text}.{edtOffset.Text}",
                        data);

                if (rlt.IsSuccess)
                {
                    btnReadByte.PerformClick();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnReadWord_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                OperateResult<ushort> rlt = plc.ReadUInt16($"DB{edtDBNumber.Text}.{edtOffset.Text}");
                if (rlt.IsSuccess)
                {
                    edtText.Text = rlt.Content.ToString();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnWriteWord_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                ushort.TryParse(edtText.Text, out ushort data);
                OperateResult rlt =
                    plc.Write(
                        $"DB{edtDBNumber.Text}.{edtOffset.Text}",
                        data);

                if (rlt.IsSuccess)
                {
                    btnReadWord.PerformClick();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnReadint_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                OperateResult<short> rlt = plc.ReadInt16($"DB{edtDBNumber.Text}.{edtOffset.Text}");
                if (rlt.IsSuccess)
                {
                    edtText.Text = rlt.Content.ToString();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnWriteInt_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                short.TryParse(edtText.Text, out short data);
                OperateResult rlt =
                    plc.Write(
                        $"DB{edtDBNumber.Text}.{edtOffset.Text}",
                        data);

                if (rlt.IsSuccess)
                {
                    btnReadint.PerformClick();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnReadDWord_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                OperateResult<uint> rlt = plc.ReadUInt32($"DB{edtDBNumber.Text}.{edtOffset.Text}");
                if (rlt.IsSuccess)
                {
                    edtText.Text = rlt.Content.ToString();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnWriteDWord_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                uint.TryParse(edtText.Text, out uint data);
                OperateResult rlt =
                    plc.Write(
                        $"DB{edtDBNumber.Text}.{edtOffset.Text}",
                        data);

                if (rlt.IsSuccess)
                {
                    btnReadDWord.PerformClick();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnReadReal_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                OperateResult<float> rlt = plc.ReadFloat($"DB{edtDBNumber.Text}.{edtOffset.Text}");
                if (rlt.IsSuccess)
                {
                    edtText.Text = rlt.Content.ToString();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnWriteReal_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                float.TryParse(edtText.Text, out float data);
                OperateResult rlt =
                    plc.Write(
                        $"DB{edtDBNumber.Text}.{edtOffset.Text}",
                        data);

                if (rlt.IsSuccess)
                {
                    btnReadReal.PerformClick();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnReadChars_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                ushort.TryParse(edtLength.Text, out ushort length);
                OperateResult<byte[]> rlt =
                    plc.Read(
                        $"DB{edtDBNumber.Text}.{edtOffset.Text}",
                        length);
                if (rlt.IsSuccess)
                {
                    edtText.Text = Encoding.ASCII.GetString(rlt.Content);
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void btnWriteChars_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("还未连接 PLC");
                return;
            }

            try
            {
                int.TryParse(edtLength.Text, out int length);
                string data = edtText.Text.PadRight(length, (char)0);
                OperateResult rlt =
                    plc.Write(
                        $"DB{edtDBNumber.Text}.{edtOffset.Text}",
                        Encoding.ASCII.GetBytes(data));

                if (rlt.IsSuccess)
                {
                    btnReadChars.PerformClick();
                }
                else
                {
                    edtText.Text = $"{rlt.ToMessageShowString()}";
                }
            }
            catch { edtText.Text = ""; }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            WriteParam("Address", edtIPAddress.Text);
            WriteParam("Port", edtPort.Text);
            WriteParam("Slot", edtSlot.Text);
            WriteParam("Rack", edtRack.Text);
        }
    }
}
