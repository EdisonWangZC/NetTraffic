namespace NetTraffic
{
    partial class Form2
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
            this.components = new System.ComponentModel.Container();
            this.lvwFile = new System.Windows.Forms.ListView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.结束进程ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvwFile
            // 
            this.lvwFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwFile.Location = new System.Drawing.Point(0, 0);
            this.lvwFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lvwFile.Name = "lvwFile";
            this.lvwFile.Size = new System.Drawing.Size(644, 380);
            this.lvwFile.TabIndex = 0;
            this.lvwFile.UseCompatibleStateImageBehavior = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.结束进程ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(139, 28);
            // 
            // 结束进程ToolStripMenuItem
            // 
            this.结束进程ToolStripMenuItem.Name = "结束进程ToolStripMenuItem";
            this.结束进程ToolStripMenuItem.Size = new System.Drawing.Size(138, 24);
            this.结束进程ToolStripMenuItem.Text = "结束进程";
            this.结束进程ToolStripMenuItem.Click += new System.EventHandler(this.结束进程ToolStripMenuItem_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 380);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.lvwFile);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form2";
            this.Text = "进程";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvwFile;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 结束进程ToolStripMenuItem;
    }
}