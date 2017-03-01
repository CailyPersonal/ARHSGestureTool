using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestureTool
{
    public partial class SubForm : Form
    {
        public delegate void InformEndEventHandler();
        public InformEndEventHandler informEnd;

        public delegate void DataGatherEventHanlder(int index);
        public DataGatherEventHanlder datagather;

        public delegate void EndThisEventHandler(int index);
        public EndThisEventHandler endthis;

        private int index = 0;
        private bool doubled = false;
        string[] slbshow = new string[]{"gx","gy","gz","ax","ay","az","mx","my","mz","waiting..."};

        public SubForm()
        {
            InitializeComponent();
        }

        private void SubForm_Load(object sender, EventArgs e)
        {

           
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            informEnd.Invoke();
 	        base.OnClosing(e);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.lb_process.Text = slbshow[index];
            if (0 == index)
            {
                //通知父窗口接收数据
                datagather.Invoke(index);
                this.buttonStart_Next_Final.Text = "Next>>";
                this.progressBar.Visible = true;
                this.progressBar.Value = 0;
                this.lb_process.Text = "gx";
                index = 1;
            }
            else 
            {
                if (4 == index || 5 == index || 6 == index)
                {
                    if (!doubled)
                    {
                        endthis.Invoke(index);
                        MessageBox.Show("Second time now!", "Cation!");
                        datagather.Invoke(index - 1);
                        doubled = true;
                        return;
                    }
                    else
                        doubled = false;
                }

                if (9 == index)
                {
                    this.buttonStart_Next_Final.Text = "Start";
                    this.lb_process.Text = slbshow[index];
                    this.progressBar.Visible = false;
                    this.progressBar.Value = 0;
                    endthis.Invoke(index);
                    datagather.Invoke(index);
                    MessageBox.Show("All done!", "Info");
                    index = 0;
                    this.Close();
                    return;
                }


                //通知父窗口结束前次数据接收过程
                endthis.Invoke(index);
                MessageBox.Show("Click this when ready!", "Caution");
                //通知父窗口接收数据
                datagather.Invoke(index);

                if (8 == index)
                {
                    this.buttonStart_Next_Final.Text = "Finish";
                }
                
                this.progressBar.Value += this.progressBar.Step;
                index++;
            }
        }

    }



}
