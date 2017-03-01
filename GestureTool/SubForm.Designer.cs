namespace GestureTool
{
    partial class SubForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubForm));
            this.buttonStart_Next_Final = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lb_process = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonStart_Next_Final
            // 
            this.buttonStart_Next_Final.Location = new System.Drawing.Point(40, 258);
            this.buttonStart_Next_Final.Name = "buttonStart_Next_Final";
            this.buttonStart_Next_Final.Size = new System.Drawing.Size(86, 25);
            this.buttonStart_Next_Final.TabIndex = 0;
            this.buttonStart_Next_Final.Text = "Start";
            this.buttonStart_Next_Final.UseVisualStyleBackColor = true;
            this.buttonStart_Next_Final.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Notice: The order is gx->gy->gz\r\n->ax-> ay->az->mx->my->mz";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Now gather data of:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 198);
            this.progressBar.Maximum = 90;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(152, 25);
            this.progressBar.TabIndex = 3;
            this.progressBar.Visible = false;
            // 
            // lb_process
            // 
            this.lb_process.AutoSize = true;
            this.lb_process.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_process.Location = new System.Drawing.Point(63, 115);
            this.lb_process.Name = "lb_process";
            this.lb_process.Size = new System.Drawing.Size(63, 17);
            this.lb_process.TabIndex = 4;
            this.lb_process.Text = "waiting...";
            // 
            // SubForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(174, 302);
            this.Controls.Add(this.lb_process);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonStart_Next_Final);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Auto Monitor";
            this.Load += new System.EventHandler(this.SubForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart_Next_Final;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lb_process;
    }
}