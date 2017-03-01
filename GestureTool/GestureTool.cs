using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.IO;

namespace GestureTool
{
    public partial class GestureTool : Form
    {
        private SerialPort comm = new SerialPort();
        private StringBuilder builder = new StringBuilder();
        private StringBuilder unUsed = new StringBuilder();
        private StringBuilder outdata = new StringBuilder();
        private delegate void ChangeUIEventHandler(byte[] buf);
        Data data = new Data();
        private byte[] singleString = new byte[26];

        StreamWriter sw;
        UInt64 datacount = 1;
        int icount = 1;
        double ax = 0;
        double ax_out = 0;
        bool isStreamOpened = false;
        private bool Listening = false;
        private bool Closing = false;
        private int show_index = -1;
        private int receive_index = 0;
        private long receive_count = 0;
        int pre = 0;
        int itemp = 0;
        public GestureTool()
        {
            InitializeComponent();
        }
        
        //解决关闭程序时出现卡死现象
        protected override void OnClosing(CancelEventArgs e)
        {
            Closing = true;
            while (Listening) Application.DoEvents();
            if (isStreamOpened)
                sw.Close();
        }
        
        //初始化配置
        private void Form_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);

            comboPortName.Items.AddRange(ports);
            comboPortName.SelectedIndex = comboPortName.Items.Count > 0 ? 0 : -1;
            comboBaudrate.SelectedIndex = comboBaudrate.Items.IndexOf("115200");

            comm.NewLine = "\r\n";
            comm.RtsEnable = true;

            comm.DataReceived += comm_DataReceived;
            this.buttonMonitor.Enabled = false;
        }

        //更新串口源数据的显示，调用OriginalDataShow_sub1函数
        private void OriginalDataShow(byte[] buf)
        {
            bool insave = false;
            foreach (byte b in buf) {
                if (!insave){
                    if (0x5F == b){
                        insave = true;
                        receive_index = 0;
                        singleString[receive_index++] = b;
                    }
                }
                else {
                    singleString[receive_index++] = b;
                    if (26 == receive_index)
                    {
                        ChangeUIEventHandler handler = new ChangeUIEventHandler(OriginalDataShow_sub1);
                        this.Invoke(handler, new object[] { singleString });
                        insave = false;
                    }
                }
            }
        }
 
        //服务于OriginalDataShow函数
        public void OriginalDataShow_sub1(byte[] buf) 
        {
            builder.Clear();
            foreach(byte b in buf){
                builder.Append(b.ToString("X2") + " ");
            }
            this.textGet.Clear();
            this.textGet.AppendText(builder.ToString());
            data.UpdateData(buf);
            RefreshShowData();
        }
 
        //事件响应，存储处理串口接收到的数据
        void comm_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            if (Closing) return;
            try
            {
                Listening = true;
                int n = comm.BytesToRead;
                byte[] buf = new byte[n];
                receive_count += n;
                comm.Read(buf, 0, n);
                OriginalDataShow(buf);
            }
            finally
            {
                Listening = false;
            }
        }
 
        //串口开启关闭
        private void buttonOpenClose_Click_Click(object sender, EventArgs e)
        {
            if (comm.IsOpen)
            {
                Closing = true;
                while (Listening) Application.DoEvents();
                comm.Close();
                Closing = false;
                this.buttonMonitor.Enabled = false;
            }
            else
            {
                if (isStreamOpened == false)
                {
                    if (DialogResult.Yes == MessageBox.Show("Write data to txt file?", "Warning", MessageBoxButtons.YesNo))
                    {
                        MessageBox.Show("Now will create a file named out", "Result");
                        System.DateTime currentTime = new System.DateTime();
                        currentTime = System.DateTime.Now;

                        string out1 = currentTime.Month.ToString() + "." +
                            currentTime.Day.ToString() + "." +
                            currentTime.Hour.ToString() + "." +
                            currentTime.Minute.ToString();
                        sw = new StreamWriter(out1 + ".txt");
                        isStreamOpened = true;
                    }
                }
               
                try
                {
                    if (comboPortName.Items.Count == 0)
                    {
                        MessageBox.Show("No port detected!", "Error");
                        return;
                    }

                    comm.PortName = comboPortName.Text;
                    comm.BaudRate = int.Parse(comboBaudrate.Text);

                    comm.Open();
                    this.buttonMonitor.Enabled = true;
                }
                catch(Exception ex)
                {
                    comm = new SerialPort();
                    MessageBox.Show(ex.Message);
                }
            }
            buttonOpenClose_Click.Text = comm.IsOpen ? "Close" : "Open";
        }
 
        //子窗口显示回调函数，使AutoMonitor按钮可用
        private void ShowButton()
        {
            this.buttonMonitor.Enabled = true;
        }
 
        //子窗口显示调用
        private void buttonMonitor_Click(object sender, EventArgs e)
        {
            //清除所有选择框按钮
            this.checkBox_ax.Checked = false;
            this.checkBox_ay.Checked = false;
            this.checkBox_az.Checked = false;
            this.checkBox_mx.Checked = false;
            this.checkBox_my.Checked = false;
            this.checkBox_mz.Checked = false;
            this.radioButton_gx.Checked = false;
            this.radioButton_gy.Checked = false;
            this.radioButton_gz.Checked = false;
            this.radioButton_ax.Checked = false;
            this.radioButton_ay.Checked = false;
            this.radioButton_az.Checked = false;
            this.radioButton_mx.Checked = false;
            this.radioButton_my.Checked = false;
            this.radioButton_mz.Checked = false;

            //清除已保存数据
            data.ClearAllData();
            for (int i = 0; i < 9; i++)
            {
                show_index = i;
                RefreshShowData();
            }
            show_index = -1;

            //开启子窗口，完成相关配置
            int x, y;
            x = this.Right+10;
            y = this.Top;
            SubForm form = new SubForm();
            form.Location = new Point(x, y);

            //委托注册，用以通知：子窗口即将关闭，可以使AutoMonitor按钮可用
            form.informEnd = this.ShowButton;
            //委托注册，用以通知：子窗口正在统计的数据索引
            form.datagather = this.gatherdata;
            //委托注册，用以通知：结束当前数值的收集
            form.endthis = this.closelastdata;
            form.Show();
            buttonMonitor.Enabled = false;
        }

        //用以更新界面上显示的数据
        private void RefreshShowData()
        {
            this.lb_gx.Text = this.data.datas[0].data.ToString();
            this.lb_gy.Text = this.data.datas[1].data.ToString();
            this.lb_gz.Text = this.data.datas[2].data.ToString();
            this.lb_ax.Text = this.data.datas[3].data.ToString();
            this.lb_ay.Text = this.data.datas[4].data.ToString();
            this.lb_az.Text = this.data.datas[5].data.ToString();
            this.lb_mx.Text = this.data.datas[6].data.ToString();
            this.lb_my.Text = this.data.datas[7].data.ToString();
            this.lb_mz.Text = this.data.datas[8].data.ToString();
            if (show_index > -1 && show_index < 9)
            {
                if (!data.datas[show_index].bGathering)
                    return;
                data.RefreshMaxAndMin(show_index);
                data.makeaverage(show_index);
            }
            this.lb_yaw.Text = this.data.yaw.ToString();


            switch (show_index)
            {
                case 0:
                    this.lb_gx_max.Text = this.data.datas[show_index].max.ToString();
                    this.lb_gx_min.Text = this.data.datas[show_index].min.ToString();
                    this.lb_gx_ave.Text = this.data.datas[show_index].average.ToString();
                    break;
                case 1:
                    this.lb_gy_max.Text = this.data.datas[show_index].max.ToString();
                    this.lb_gy_min.Text = this.data.datas[show_index].min.ToString();
                    this.lb_gy_ave.Text = this.data.datas[show_index].average.ToString();
                    break;
                case 2:
                    this.lb_gz_max.Text = this.data.datas[show_index].max.ToString();
                    this.lb_gz_min.Text = this.data.datas[show_index].min.ToString();
                    this.lb_gz_ave.Text = this.data.datas[show_index].average.ToString();
                    break;
                case 3:
                    this.lb_ax_max.Text = this.data.datas[show_index].max.ToString();
                    this.lb_ax_min.Text = this.data.datas[show_index].min.ToString();
                    this.lb_ax_ave.Text = this.data.datas[show_index].average.ToString();
                    break;
                case 4:
                    this.lb_ay_max.Text = this.data.datas[show_index].max.ToString();
                    this.lb_ay_min.Text = this.data.datas[show_index].min.ToString();
                    this.lb_ay_ave.Text = this.data.datas[show_index].average.ToString();
                    break;
                case 5:
                    this.lb_az_max.Text = this.data.datas[show_index].max.ToString();
                    this.lb_az_min.Text = this.data.datas[show_index].min.ToString();
                    this.lb_az_ave.Text = this.data.datas[show_index].average.ToString();
                    break;
                case 6:
                    this.lb_mx_max.Text = this.data.datas[show_index].max.ToString();
                    this.lb_mx_min.Text = this.data.datas[show_index].min.ToString();
                    this.lb_mx_ave.Text = this.data.datas[show_index].average.ToString();
                    break;
                case 7:
                    this.lb_my_max.Text = this.data.datas[show_index].max.ToString();
                    this.lb_my_min.Text = this.data.datas[show_index].min.ToString();
                    this.lb_my_ave.Text = this.data.datas[show_index].average.ToString();
                    break;
                case 8:
                    this.lb_mz_max.Text = this.data.datas[show_index].max.ToString();
                    this.lb_mz_min.Text = this.data.datas[show_index].min.ToString();
                    this.lb_mz_ave.Text = this.data.datas[show_index].average.ToString();
                    break;
            }

            if (isStreamOpened)
            {
                outdata.Clear();
                outdata.Append(datacount.ToString() + " "
                    + this.data.datas[0].data.ToString() + " "
                    + this.data.datas[1].data.ToString() + " "
                    + this.data.datas[2].data.ToString() + " "
                    + this.data.datas[3].data.ToString() + " "
                    + this.data.datas[4].data.ToString() + " "
                    + this.data.datas[5].data.ToString() + " "
                    + this.data.datas[6].data.ToString() + " "
                    + this.data.datas[7].data.ToString() + " "
                    + this.data.datas[8].data.ToString() + " "
                    + this.data.yaw.ToString() + " ");
                datacount++;
                ax += this.data.datas[0].data;
                if (initcount < 4)
                {
                    buffer[initcount++] = this.data.datas[0].data;
                }
                else 
                {
                    outdata.Append(SortAndOut(this.data.datas[0].data).ToString());
                }

                sw.WriteLine(outdata);
            }
        }

        int initcount = 0;
        int[] buffer = new int[5];
        int bufferIndex = 0;
        int prePos = -1;
        int k, j, temp;
        int SortAndOut(int newdata)
        {
            if (-1 == prePos)
            {
                buffer[4] = newdata;
            }
            else
                buffer[prePos] = newdata;
            for (int i = 0; i < 3; i++)
            {
                k = i;
                for (j = i + 1; j < 5; j++)
                    if (buffer[j] < buffer[k])
                        k = j;
                if (k != i)
                {
                    temp = buffer[i];
                    buffer[i] = buffer[k];
                    buffer[k] = temp;
                }
            }
            k = 0;
            for (int i = 0; i < 5; i++)
            {
                if (buffer[i] == newdata)
                {
                    prePos = i;
                    break;
                }
            }
            return buffer[2];
        }

        //更新界面勾选按钮
        private void RefreshCheckAndRadio(int index, bool Check, bool radio)
        {
            switch (index)
            {
                case 0:
                    this.checkBox_gx.Checked = Check;
                    this.radioButton_gx.Checked = radio;
                    break;
                case 1:
                    this.checkBox_gy.Checked = Check;
                    this.radioButton_gy.Checked = radio;
                    break;
                case 2:
                    this.checkBox_gz.Checked = Check;
                    this.radioButton_gz.Checked = radio;
                    break;
                case 3:
                    this.checkBox_ax.Checked = Check;
                    this.radioButton_ax.Checked = radio;
                    break;
                case 4:
                    this.checkBox_ay.Checked = Check;
                    this.radioButton_ay.Checked = radio;
                    break;
                case 5:
                    this.checkBox_az.Checked = Check;
                    this.radioButton_az.Checked = radio;
                    break;
                case 6:
                    this.checkBox_mx.Checked = Check;
                    this.radioButton_mx.Checked = radio;
                    break;
                case 7:
                    this.checkBox_my.Checked = Check;
                    this.radioButton_my.Checked = radio;
                    break;
                case 8:
                    this.checkBox_mz.Checked = Check;
                    this.radioButton_mz.Checked = radio;
                    break;
            }
        }

        //接收数据通知1
        private void gatherdata(int index)
        {
            //用以通知正在接收处理数据
            if (index < 9)
            {
                show_index = index;
                data.datas[index].bGathering = true;
                RefreshCheckAndRadio(index, true, false);
            }
            if (9 == index)
            {
                data.datas[8].bGathering = false;
                data.datas[8].bGathered = false;
                RefreshCheckAndRadio(8, false, true);
            }

        }

        //接收数据通知2
        private void closelastdata(int index)
        {
            if (index > 0)
            {
                data.datas[index - 1].bGathered = true;
                data.datas[index - 1].bGathering = false;
                RefreshCheckAndRadio(index - 1, false, true);
            }
        }
    }


    class SingleData
    {
        public bool bGathering, bGathered;
        public Int16 data, max, min, average;

        public SingleData()
        {
            bGathered = bGathering = false;
            data = max = min = average = 0;
        }

    }



    class Data
    {
        private delegate void CalculateEventHanlder(object sender, EventArgs e);

        public SingleData[] datas = new SingleData[9];//The order is gx,gy,gz,ax,ay,az,mx,my,mz
        public Int16 temp = 0;
        public float pitch, roll, yaw;

        public Data()
        {
            for (int i = 0; i < 9; i++)
            {
                datas[i] = new SingleData();
                datas[i].data = 0;
                datas[i].max = 0;
                datas[i].min = 0;
                datas[i].average = 0;
            }
        }

        public void ClearAllData()
        {
            getoncedata();
            for (int i = 3; i < 9; i++)
            {
                datas[i].max = 0;
                datas[i].min = 0;
                datas[i].average = 0;
            }
        }

        public void makeaverage(int index) 
        {
            datas[index].average = (Int16)((datas[index].min + datas[index].max) / 2);
        }

        public void UpdateData(byte[] buf)
        {
            datas[0].data = (Int16)((int)buf[2] << 8);
            datas[0].data += buf[3];
            datas[1].data = (Int16)((int)buf[4] << 8);
            datas[1].data += buf[5];
            datas[2].data = (Int16)((int)buf[6] << 8);
            datas[2].data += buf[7];

            datas[3].data = (Int16)((int)buf[8] << 8);
            datas[3].data += buf[9];
            datas[4].data = (Int16)((int)buf[10] << 8);
            datas[4].data += buf[11];
            datas[5].data = (Int16)((int)buf[12] << 8);
            datas[5].data += buf[13];

            datas[6].data = (Int16)((int)buf[14] << 8);
            datas[6].data += buf[15];
            datas[7].data = (Int16)((int)buf[16] << 8);
            datas[7].data += buf[17];
            datas[8].data = (Int16)((int)buf[18] << 8);
            datas[8].data += buf[19];

            temp = (Int16)((int)buf[20] << 8);
            temp += buf[21];
            pitch = (float)temp / 100;

            temp = (Int16)((int)buf[22] << 8);
            temp += buf[23];
            roll = (float)temp / 100;

            temp = (Int16)((int)buf[24] << 8);
            temp += buf[25];
            yaw = (float)temp / 100;
        }

        public void getoncedata()
        {
            datas[0].min = datas[0].max = datas[0].data;
            datas[1].min = datas[1].max = datas[1].data;
            datas[2].min = datas[2].max = datas[2].data;
        }

        public void DealData(byte buf) 
        {
            for (int i = 0; i < 9; i++)
            {
                if (datas[i].bGathering)
                    RefreshMaxAndMin(i);
            }
        }

        #region This part calculate the max and min data of each item
        public void RefreshMaxAndMin(int index)
        {
            datas[index].max = datas[index].data > datas[index].max ? datas[index].data : datas[index].max;
            datas[index].min = datas[index].data < datas[index].min ? datas[index].data : datas[index].min;
        }
        #endregion
      
    }
}
