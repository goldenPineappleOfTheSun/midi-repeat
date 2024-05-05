
namespace MidiClient
{
    partial class ConfigureBasicsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.numericSize = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericBeats = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBeats)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.SeaShell;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Длина кассеты (ms)";
            // 
            // numericSize
            // 
            this.numericSize.BackColor = System.Drawing.Color.MistyRose;
            this.numericSize.ForeColor = System.Drawing.SystemColors.InfoText;
            this.numericSize.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericSize.Location = new System.Drawing.Point(16, 31);
            this.numericSize.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericSize.Name = "numericSize";
            this.numericSize.Size = new System.Drawing.Size(97, 20);
            this.numericSize.TabIndex = 1;
            this.numericSize.ValueChanged += new System.EventHandler(this.numericSize_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.SeaShell;
            this.label2.Location = new System.Drawing.Point(15, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ударов метронома";
            // 
            // numericBeats
            // 
            this.numericBeats.BackColor = System.Drawing.Color.MistyRose;
            this.numericBeats.ForeColor = System.Drawing.SystemColors.InfoText;
            this.numericBeats.Location = new System.Drawing.Point(15, 88);
            this.numericBeats.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericBeats.Name = "numericBeats";
            this.numericBeats.Size = new System.Drawing.Size(97, 20);
            this.numericBeats.TabIndex = 3;
            this.numericBeats.ValueChanged += new System.EventHandler(this.numericBeats_ValueChanged);
            // 
            // ConfigureBasicsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(124, 127);
            this.Controls.Add(this.numericBeats);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericSize);
            this.Controls.Add(this.label1);
            this.Name = "ConfigureBasicsForm";
            this.Text = "ConfigureBasicsForm";
            this.Load += new System.EventHandler(this.ConfigureBasicsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBeats)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericBeats;
    }
}