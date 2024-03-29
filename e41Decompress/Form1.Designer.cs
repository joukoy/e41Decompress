﻿namespace e41Decompress
{
    partial class Form1
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
            this.btnExtract = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.btnCompress = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboCompType = new System.Windows.Forms.ComboBox();
            this.btnMultiCompress = new System.Windows.Forms.Button();
            this.btnMultiExtract = new System.Windows.Forms.Button();
            this.txtExtension = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnTestIntegrity = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnReadFile = new System.Windows.Forms.Button();
            this.groupExtract = new System.Windows.Forms.GroupBox();
            this.chkWriteTxt = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupExtract.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(134, 14);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(77, 23);
            this.btnExtract.TabIndex = 0;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(1, 100);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(721, 349);
            this.txtResult.TabIndex = 2;
            this.txtResult.Text = "";
            // 
            // btnCompress
            // 
            this.btnCompress.Location = new System.Drawing.Point(93, 21);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(72, 23);
            this.btnCompress.TabIndex = 3;
            this.btnCompress.Text = "Compress";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboCompType);
            this.groupBox1.Controls.Add(this.btnCompress);
            this.groupBox1.Location = new System.Drawing.Point(332, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(175, 54);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compress as";
            // 
            // comboCompType
            // 
            this.comboCompType.FormattingEnabled = true;
            this.comboCompType.Location = new System.Drawing.Point(6, 23);
            this.comboCompType.Name = "comboCompType";
            this.comboCompType.Size = new System.Drawing.Size(81, 21);
            this.comboCompType.TabIndex = 4;
            // 
            // btnMultiCompress
            // 
            this.btnMultiCompress.Location = new System.Drawing.Point(394, 11);
            this.btnMultiCompress.Name = "btnMultiCompress";
            this.btnMultiCompress.Size = new System.Drawing.Size(119, 23);
            this.btnMultiCompress.TabIndex = 6;
            this.btnMultiCompress.Text = "MultiCompress";
            this.btnMultiCompress.UseVisualStyleBackColor = true;
            this.btnMultiCompress.Click += new System.EventHandler(this.btnMultiCompress_Click);
            // 
            // btnMultiExtract
            // 
            this.btnMultiExtract.Location = new System.Drawing.Point(255, 12);
            this.btnMultiExtract.Name = "btnMultiExtract";
            this.btnMultiExtract.Size = new System.Drawing.Size(133, 23);
            this.btnMultiExtract.TabIndex = 7;
            this.btnMultiExtract.Text = "MultiExtract";
            this.btnMultiExtract.UseVisualStyleBackColor = true;
            this.btnMultiExtract.Click += new System.EventHandler(this.btnMultiExtract_Click);
            // 
            // txtExtension
            // 
            this.txtExtension.Location = new System.Drawing.Point(153, 14);
            this.txtExtension.Name = "txtExtension";
            this.txtExtension.Size = new System.Drawing.Size(80, 20);
            this.txtExtension.TabIndex = 8;
            this.txtExtension.Text = "-new.bin";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnTestIntegrity);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnMultiCompress);
            this.groupBox2.Controls.Add(this.btnMultiExtract);
            this.groupBox2.Controls.Add(this.txtExtension);
            this.groupBox2.Location = new System.Drawing.Point(7, 54);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(702, 43);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Multiple files";
            // 
            // btnTestIntegrity
            // 
            this.btnTestIntegrity.Location = new System.Drawing.Point(519, 11);
            this.btnTestIntegrity.Name = "btnTestIntegrity";
            this.btnTestIntegrity.Size = new System.Drawing.Size(119, 23);
            this.btnTestIntegrity.TabIndex = 10;
            this.btnTestIntegrity.Text = "Test integrity...";
            this.btnTestIntegrity.UseVisualStyleBackColor = true;
            this.btnTestIntegrity.Click += new System.EventHandler(this.btnTestIntegrity_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Rename files when saving:";
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(7, 31);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(75, 23);
            this.btnAbout.TabIndex = 10;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnReadFile
            // 
            this.btnReadFile.Location = new System.Drawing.Point(7, 6);
            this.btnReadFile.Name = "btnReadFile";
            this.btnReadFile.Size = new System.Drawing.Size(75, 23);
            this.btnReadFile.TabIndex = 12;
            this.btnReadFile.Text = "Read file";
            this.btnReadFile.UseVisualStyleBackColor = true;
            this.btnReadFile.Click += new System.EventHandler(this.btnReadFile_Click);
            // 
            // groupExtract
            // 
            this.groupExtract.Controls.Add(this.chkWriteTxt);
            this.groupExtract.Controls.Add(this.btnExtract);
            this.groupExtract.Location = new System.Drawing.Point(98, 5);
            this.groupExtract.Name = "groupExtract";
            this.groupExtract.Size = new System.Drawing.Size(228, 48);
            this.groupExtract.TabIndex = 13;
            this.groupExtract.TabStop = false;
            this.groupExtract.Text = "Extract";
            // 
            // chkWriteTxt
            // 
            this.chkWriteTxt.AutoSize = true;
            this.chkWriteTxt.Checked = true;
            this.chkWriteTxt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWriteTxt.Location = new System.Drawing.Point(12, 22);
            this.chkWriteTxt.Name = "chkWriteTxt";
            this.chkWriteTxt.Size = new System.Drawing.Size(103, 17);
            this.chkWriteTxt.TabIndex = 1;
            this.chkWriteTxt.Text = "Write type to .txt";
            this.chkWriteTxt.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 450);
            this.Controls.Add(this.groupExtract);
            this.Controls.Add(this.btnReadFile);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtResult);
            this.Name = "Form1";
            this.Text = "(De)Compress";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupExtract.ResumeLayout(false);
            this.groupExtract.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnMultiCompress;
        private System.Windows.Forms.Button btnMultiExtract;
        private System.Windows.Forms.TextBox txtExtension;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnReadFile;
        private System.Windows.Forms.GroupBox groupExtract;
        private System.Windows.Forms.CheckBox chkWriteTxt;
        private System.Windows.Forms.Button btnTestIntegrity;
        private System.Windows.Forms.ComboBox comboCompType;
    }
}

