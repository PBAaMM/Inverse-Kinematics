namespace InverseKinematics
{
    partial class MainForm
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.skeleton_creation = new System.Windows.Forms.Button();
            this.forward_kinematics = new System.Windows.Forms.Button();
            this.inverse_kinematics = new System.Windows.Forms.Button();
            this.status_bar = new System.Windows.Forms.StatusBar();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // skeleton_creation
            // 
            this.skeleton_creation.Location = new System.Drawing.Point(34, 310);
            this.skeleton_creation.Name = "skeleton_creation";
            this.skeleton_creation.Size = new System.Drawing.Size(108, 23);
            this.skeleton_creation.TabIndex = 0;
            this.skeleton_creation.Text = "Skeleton Creation";
            this.skeleton_creation.UseVisualStyleBackColor = true;
            this.skeleton_creation.Click += new System.EventHandler(this.skeleton_creation_Click);
            // 
            // forward_kinematics
            // 
            this.forward_kinematics.Location = new System.Drawing.Point(238, 310);
            this.forward_kinematics.Name = "forward_kinematics";
            this.forward_kinematics.Size = new System.Drawing.Size(111, 23);
            this.forward_kinematics.TabIndex = 1;
            this.forward_kinematics.Text = "Forward Kinematics";
            this.forward_kinematics.UseVisualStyleBackColor = true;
            this.forward_kinematics.Click += new System.EventHandler(this.forward_kinematics_Click);
            // 
            // inverse_kinematics
            // 
            this.inverse_kinematics.Location = new System.Drawing.Point(438, 310);
            this.inverse_kinematics.Name = "inverse_kinematics";
            this.inverse_kinematics.Size = new System.Drawing.Size(106, 23);
            this.inverse_kinematics.TabIndex = 2;
            this.inverse_kinematics.Text = "Inverse Kinematics";
            this.inverse_kinematics.UseVisualStyleBackColor = true;
            this.inverse_kinematics.Click += new System.EventHandler(this.inverse_kinematics_Click);
            // 
            // status_bar
            // 
            this.status_bar.Location = new System.Drawing.Point(0, 339);
            this.status_bar.Name = "status_bar";
            this.status_bar.Size = new System.Drawing.Size(584, 22);
            this.status_bar.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.status_bar);
            this.Controls.Add(this.inverse_kinematics);
            this.Controls.Add(this.forward_kinematics);
            this.Controls.Add(this.skeleton_creation);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button skeleton_creation;
        private System.Windows.Forms.Button forward_kinematics;
        private System.Windows.Forms.Button inverse_kinematics;
        private System.Windows.Forms.StatusBar status_bar;
    }
}

