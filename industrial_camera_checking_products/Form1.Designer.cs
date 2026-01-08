namespace industrial_camera_checking_products
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private PictureBox pictureBox;
        private Button btnStart;
        private Button btnStop;
        private Button btnLoadModel;
        private ComboBox cboCameras;
        private Label lblStatus;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBox = new PictureBox();
            btnStart = new Button();
            btnStop = new Button();
            btnLoadModel = new Button();
            cboCameras = new ComboBox();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox.Location = new Point(12, 12);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(776, 368);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            // 
            // btnStart
            // 
            btnStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStart.Location = new Point(12, 400);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(94, 29);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStop.Location = new Point(112, 400);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(94, 29);
            btnStop.TabIndex = 2;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnLoadModel
            // 
            btnLoadModel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLoadModel.Location = new Point(212, 400);
            btnLoadModel.Name = "btnLoadModel";
            btnLoadModel.Size = new Size(120, 29);
            btnLoadModel.TabIndex = 3;
            btnLoadModel.Text = "Load YOLO";
            btnLoadModel.UseVisualStyleBackColor = true;
            btnLoadModel.Click += btnLoadModel_Click;
            // 
            // cboCameras
            // 
            cboCameras.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cboCameras.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCameras.FormattingEnabled = true;
            cboCameras.Location = new Point(338, 401);
            cboCameras.Name = "cboCameras";
            cboCameras.Size = new Size(188, 28);
            cboCameras.TabIndex = 4;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.AutoEllipsis = true;
            lblStatus.Location = new Point(532, 401);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(256, 28);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Ready";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblStatus);
            Controls.Add(cboCameras);
            Controls.Add(btnLoadModel);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(pictureBox);
            Name = "Form1";
            Text = "Camera + YOLO";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}
