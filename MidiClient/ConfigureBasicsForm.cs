using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MidiClient
{
    public partial class ConfigureBasicsForm : Form
    {
        public int size;
        public int beats;

        public ConfigureBasicsForm(int size, int beats)
        {
            InitializeComponent();
            this.size = size;
            this.beats = beats;
        }

        private void ConfigureBasicsForm_Load(object sender, EventArgs e)
        {
            numericSize.Value = size;
            numericBeats.Value = beats;
        }

        private void numericSize_ValueChanged(object sender, EventArgs e)
        {
            size = ((int)numericSize.Value);
        }

        private void numericBeats_ValueChanged(object sender, EventArgs e)
        {
            beats = ((int)numericBeats.Value);
        }
    }
}
