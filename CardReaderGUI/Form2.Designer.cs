using System.Reflection.Emit;
using System.Windows.Forms;

namespace CardReaderGUI
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public static System.Windows.Forms.Label[] statusLabelArray = new System.Windows.Forms.Label[1];
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
            this.chooseDirectory = new System.Windows.Forms.Button();
            this.uploadButton = new System.Windows.Forms.Button();
            this.chosenDirectoryText = new System.Windows.Forms.Label();
            this.directoryPathText = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxFahrer = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nfcStatusLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxNFC = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // chooseDirectory
            // 
            this.chooseDirectory.Location = new System.Drawing.Point(12, 75);
            this.chooseDirectory.Name = "chooseDirectory";
            this.chooseDirectory.Size = new System.Drawing.Size(104, 23);
            this.chooseDirectory.TabIndex = 0;
            this.chooseDirectory.Text = "choose directory";
            this.chooseDirectory.UseVisualStyleBackColor = true;
            this.chooseDirectory.Click += new System.EventHandler(this.button1_Click);
            // 
            // uploadButton
            // 
            this.uploadButton.Location = new System.Drawing.Point(128, 75);
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(105, 23);
            this.uploadButton.TabIndex = 1;
            this.uploadButton.Text = "upload";
            this.uploadButton.UseVisualStyleBackColor = true;
            this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // chosenDirectoryText
            // 
            this.chosenDirectoryText.AutoSize = true;
            this.chosenDirectoryText.Location = new System.Drawing.Point(26, 43);
            this.chosenDirectoryText.Name = "chosenDirectoryText";
            this.chosenDirectoryText.Size = new System.Drawing.Size(90, 13);
            this.chosenDirectoryText.TabIndex = 2;
            this.chosenDirectoryText.Text = "chosen Directory:";
            this.chosenDirectoryText.Click += new System.EventHandler(this.chosenDirectoryText_Click);
            // 
            // directoryPathText
            // 
            this.directoryPathText.AutoSize = true;
            this.directoryPathText.Location = new System.Drawing.Point(116, 43);
            this.directoryPathText.Name = "directoryPathText";
            this.directoryPathText.Size = new System.Drawing.Size(0, 13);
            this.directoryPathText.TabIndex = 3;
            this.directoryPathText.Click += new System.EventHandler(this.label1_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(26, 59);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(87, 13);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "select a directory";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.Location = new System.Drawing.Point(63, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 18);
            this.label1.TabIndex = 5;
            this.label1.Text = "Fahrerkarten-leser";
            this.label1.Click += new System.EventHandler(this.label1_Click_2);
            // 
            // comboBoxFahrer
            // 
            this.comboBoxFahrer.FormattingEnabled = true;
            this.comboBoxFahrer.Location = new System.Drawing.Point(81, 104);
            this.comboBoxFahrer.Name = "comboBoxFahrer";
            this.comboBoxFahrer.Size = new System.Drawing.Size(121, 21);
            this.comboBoxFahrer.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Kartenleser:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(453, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "NFC-Leser";
            // 
            // nfcStatusLabel
            // 
            this.nfcStatusLabel.AutoSize = true;
            this.nfcStatusLabel.Location = new System.Drawing.Point(386, 43);
            this.nfcStatusLabel.Name = "nfcStatusLabel";
            this.nfcStatusLabel.Size = new System.Drawing.Size(170, 13);
            this.nfcStatusLabel.TabIndex = 9;
            this.nfcStatusLabel.Text = "Bitte einen Kartenleser auswählen!";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(367, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Kartenleser:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // comboBoxNFC
            // 
            this.comboBoxNFC.FormattingEnabled = true;
            this.comboBoxNFC.Location = new System.Drawing.Point(433, 102);
            this.comboBoxNFC.Name = "comboBoxNFC";
            this.comboBoxNFC.Size = new System.Drawing.Size(121, 21);
            this.comboBoxNFC.TabIndex = 11;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 135);
            this.Controls.Add(this.comboBoxNFC);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nfcStatusLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxFahrer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.directoryPathText);
            this.Controls.Add(this.chosenDirectoryText);
            this.Controls.Add(this.uploadButton);
            this.Controls.Add(this.chooseDirectory);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button chooseDirectory;
        private System.Windows.Forms.Button uploadButton;
        private System.Windows.Forms.Label chosenDirectoryText;
        private System.Windows.Forms.Label directoryPathText;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label1;
        private ComboBox comboBoxFahrer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label nfcStatusLabel;
        private System.Windows.Forms.Label label4;
        private ComboBox comboBoxNFC;
    }
}