
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace MidiClient
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.configPanelGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonsConfigButton = new System.Windows.Forms.Button();
            this.wiringConfigButton = new System.Windows.Forms.Button();
            this.basicConfigButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.testDevicesButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.startButton = new System.Windows.Forms.Button();
            this.metronomeButton = new System.Windows.Forms.Button();
            this.configButton = new System.Windows.Forms.Button();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.configPanelGroupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // configPanelGroupBox
            // 
            this.configPanelGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.configPanelGroupBox.Controls.Add(this.checkBox1);
            this.configPanelGroupBox.Controls.Add(this.buttonsConfigButton);
            this.configPanelGroupBox.Controls.Add(this.wiringConfigButton);
            this.configPanelGroupBox.Controls.Add(this.basicConfigButton);
            this.configPanelGroupBox.Location = new System.Drawing.Point(229, -3);
            this.configPanelGroupBox.Name = "configPanelGroupBox";
            this.configPanelGroupBox.Size = new System.Drawing.Size(182, 132);
            this.configPanelGroupBox.TabIndex = 3;
            this.configPanelGroupBox.TabStop = false;
            this.configPanelGroupBox.Visible = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.ForeColor = System.Drawing.Color.MistyRose;
            this.checkBox1.Location = new System.Drawing.Point(49, 103);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(88, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Бэкинг трек";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // buttonsConfigButton
            // 
            this.buttonsConfigButton.BackColor = System.Drawing.Color.MistyRose;
            this.buttonsConfigButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonsConfigButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonsConfigButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.buttonsConfigButton.Location = new System.Drawing.Point(6, 71);
            this.buttonsConfigButton.Name = "buttonsConfigButton";
            this.buttonsConfigButton.Size = new System.Drawing.Size(169, 23);
            this.buttonsConfigButton.TabIndex = 1;
            this.buttonsConfigButton.Text = "Управляющие кнопки";
            this.buttonsConfigButton.UseVisualStyleBackColor = false;
            this.buttonsConfigButton.Click += new System.EventHandler(this.buttonsConfigButton_Click);
            // 
            // wiringConfigButton
            // 
            this.wiringConfigButton.BackColor = System.Drawing.Color.MistyRose;
            this.wiringConfigButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.wiringConfigButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.wiringConfigButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.wiringConfigButton.Location = new System.Drawing.Point(7, 42);
            this.wiringConfigButton.Name = "wiringConfigButton";
            this.wiringConfigButton.Size = new System.Drawing.Size(169, 23);
            this.wiringConfigButton.TabIndex = 0;
            this.wiringConfigButton.Text = "MIDI-устройства";
            this.wiringConfigButton.UseVisualStyleBackColor = false;
            this.wiringConfigButton.Click += new System.EventHandler(this.wiringConfigButton_Click);
            // 
            // basicConfigButton
            // 
            this.basicConfigButton.BackColor = System.Drawing.Color.MistyRose;
            this.basicConfigButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.basicConfigButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.basicConfigButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.basicConfigButton.Location = new System.Drawing.Point(7, 13);
            this.basicConfigButton.Name = "basicConfigButton";
            this.basicConfigButton.Size = new System.Drawing.Size(169, 23);
            this.basicConfigButton.TabIndex = 0;
            this.basicConfigButton.Text = "Базовые параметры";
            this.basicConfigButton.UseVisualStyleBackColor = false;
            this.basicConfigButton.Click += new System.EventHandler(this.basicConfigButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DimGray;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(13, 186);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(429, 7);
            this.panel1.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.MistyRose;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(5, 7);
            this.panel2.TabIndex = 0;
            // 
            // testDevicesButton
            // 
            this.testDevicesButton.BackColor = System.Drawing.Color.MistyRose;
            this.testDevicesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.testDevicesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.testDevicesButton.ForeColor = System.Drawing.Color.Black;
            this.testDevicesButton.Location = new System.Drawing.Point(117, 10);
            this.testDevicesButton.Name = "testDevicesButton";
            this.testDevicesButton.Size = new System.Drawing.Size(75, 23);
            this.testDevicesButton.TabIndex = 6;
            this.testDevicesButton.Text = "Проверка";
            this.testDevicesButton.UseVisualStyleBackColor = false;
            this.testDevicesButton.Click += new System.EventHandler(this.testDevicesButton_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // startButton
            // 
            this.startButton.BackColor = System.Drawing.Color.MistyRose;
            this.startButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.startButton.ForeColor = System.Drawing.Color.Black;
            this.startButton.Location = new System.Drawing.Point(191, 10);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 6;
            this.startButton.Text = "Старт";
            this.startButton.UseVisualStyleBackColor = false;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // metronomeButton
            // 
            this.metronomeButton.BackColor = System.Drawing.Color.DimGray;
            this.metronomeButton.Enabled = false;
            this.metronomeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.metronomeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.metronomeButton.ForeColor = System.Drawing.Color.Black;
            this.metronomeButton.Location = new System.Drawing.Point(265, 10);
            this.metronomeButton.Name = "metronomeButton";
            this.metronomeButton.Size = new System.Drawing.Size(75, 23);
            this.metronomeButton.TabIndex = 6;
            this.metronomeButton.Text = "Метроном";
            this.metronomeButton.UseVisualStyleBackColor = false;
            this.metronomeButton.Click += new System.EventHandler(this.metronomeButton_Click);
            // 
            // configButton
            // 
            this.configButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.configButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.configButton.FlatAppearance.BorderSize = 0;
            this.configButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.configButton.Image = global::MidiClient.Properties.Resources.cogw;
            this.configButton.Location = new System.Drawing.Point(413, 8);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(32, 32);
            this.configButton.TabIndex = 2;
            this.configButton.UseVisualStyleBackColor = false;
            this.configButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBox12
            // 
            this.pictureBox12.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox12.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox12.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox12.Location = new System.Drawing.Point(378, 115);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Size = new System.Drawing.Size(64, 64);
            this.pictureBox12.TabIndex = 4;
            this.pictureBox12.TabStop = false;
            // 
            // pictureBox11
            // 
            this.pictureBox11.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox11.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox11.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox11.Location = new System.Drawing.Point(308, 115);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(64, 64);
            this.pictureBox11.TabIndex = 4;
            this.pictureBox11.TabStop = false;
            // 
            // pictureBox10
            // 
            this.pictureBox10.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox10.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox10.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox10.Location = new System.Drawing.Point(238, 115);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(64, 64);
            this.pictureBox10.TabIndex = 4;
            this.pictureBox10.TabStop = false;
            // 
            // pictureBox9
            // 
            this.pictureBox9.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox9.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox9.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox9.Location = new System.Drawing.Point(152, 115);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(64, 64);
            this.pictureBox9.TabIndex = 4;
            this.pictureBox9.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox8.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox8.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox8.Location = new System.Drawing.Point(82, 115);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(64, 64);
            this.pictureBox8.TabIndex = 4;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox7.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox7.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox7.Location = new System.Drawing.Point(12, 115);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(64, 64);
            this.pictureBox7.TabIndex = 4;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox6.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox6.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox6.Location = new System.Drawing.Point(378, 45);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(64, 64);
            this.pictureBox6.TabIndex = 4;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox5.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox5.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox5.Location = new System.Drawing.Point(308, 45);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(64, 64);
            this.pictureBox5.TabIndex = 4;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox4.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox4.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox4.Location = new System.Drawing.Point(238, 45);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(64, 64);
            this.pictureBox4.TabIndex = 4;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox3.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox3.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox3.Location = new System.Drawing.Point(152, 45);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(64, 64);
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox2.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox2.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox2.Location = new System.Drawing.Point(82, 45);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(64, 64);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.MistyRose;
            this.pictureBox1.Image = global::MidiClient.Properties.Resources.frame;
            this.pictureBox1.InitialImage = global::MidiClient.Properties.Resources.frame;
            this.pictureBox1.Location = new System.Drawing.Point(12, 45);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(454, 209);
            this.Controls.Add(this.configPanelGroupBox);
            this.Controls.Add(this.configButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox12);
            this.Controls.Add(this.pictureBox11);
            this.Controls.Add(this.pictureBox10);
            this.Controls.Add(this.pictureBox9);
            this.Controls.Add(this.pictureBox8);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.metronomeButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.testDevicesButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.configPanelGroupBox.ResumeLayout(false);
            this.configPanelGroupBox.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button configButton;
        private System.Windows.Forms.GroupBox configPanelGroupBox;
        private System.Windows.Forms.Button buttonsConfigButton;
        private System.Windows.Forms.Button wiringConfigButton;
        private System.Windows.Forms.Button basicConfigButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.PictureBox pictureBox12;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button testDevicesButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button metronomeButton;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

