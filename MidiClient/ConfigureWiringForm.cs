using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MidiClient
{
    public partial class ConfigureWiringForm : Form
    {
        public List<string> current;

        int outputs;
        List<string> inputs;
        List<string> names;

        public ConfigureWiringForm(int outputs, List<string> inputs, List<string> current, List<string> names)
        {
            InitializeComponent();
            this.inputs = inputs;
            this.outputs = outputs;
            this.current = current;
            this.names = names;
        }

        private void ConfigureWiringForm_Load(object sender, EventArgs e)
        {
            var options = new List<string>();
            var sameDevicesCount = new Dictionary<string, int>();

            foreach (var item in inputs)
            {
                if (!sameDevicesCount.ContainsKey(item))
                {
                    sameDevicesCount.Add(item, 0);
                    options.Add(item);
                }
                else
                {
                    if (sameDevicesCount[item] == 0)
                    {
                        options.Remove(item);
                        options.Add(string.Format("{0} [{1}]", item, "A"));
                    }
                    sameDevicesCount[item] += 1;
                    var letter = (new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" })[sameDevicesCount[item]];
                    options.Add(string.Format("{0} [{1}]", item, letter));
                }

            }

            for (var i=1; i<=12; i++)
            {
                var group = this.Controls.Find(string.Format("groupBox{0}", i), true).First() as GroupBox;
                var control = this.Controls.Find(string.Format("comboBox{0}", i), true).First() as ComboBox;
                group.Text = i <= names.Count ? string.Format("loopMIDI Port {0} ({1})", i, names[i-1]) : string.Format("loopMIDI Port {0}", i);
                group.Enabled = i <= outputs;

                control.Items.Clear();
                control.Items.Add("");
                foreach (var item in options)
                {
                    control.Items.Add(item);
                }

                if (current.Count >= i)
                {
                    var index = control.FindString(current[i-1]);
                    if (index == -1)
                    {
                        var alternative = Regex.Replace(current[i - 1], @"(.*)( \[[A-H]\])$", m => m.Groups[1].Value);
                        current[i - 1] = alternative;
                        index = control.FindString(current[i - 1]);
                    }

                    if (index != -1)
                    {
                        control.SelectedIndex = index;
                    }
                    else
                    {
                        current[i - 1] = "";
                    }
                }
            }

            if (current.Count > outputs)
            {
                current = current.Take(outputs).ToList();
            }

            while (current.Count < outputs)
            {
                current.Add("");
            }
        }

        private void ItemSelected(object sender, EventArgs e)
        {
            var control = (sender as ComboBox);
            current[int.Parse(control.Name.Substring(8)) - 1] = control.SelectedItem as string;
        }
    }
}
