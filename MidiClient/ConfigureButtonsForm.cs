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

    public partial class ConfigureButtonsForm : Form
    {
        public List<ButtonConfig> buttons;

        List<string> scripts;
        Button selected;
        bool selectedButtonBlinkState = false;
        Form1 owner;

        public ConfigureButtonsForm(List<string> buttons, List<string> scripts, Form1 owner)
        {
            InitializeComponent();
            this.scripts = scripts;
            this.buttons = buttons.Select(x => new ButtonConfig(x)).ToList();
            this.owner = owner;
        }

        private void ConfigureButtonsForm_Load(object sender, EventArgs e)
        {
            for (var i = 1; i <= 32; i++)
            {
                var control = this.Controls.Find(string.Format("button{0}", i), true).First() as Button;
                if (i <= scripts.Count)
                {
                    control.Enabled = true;
                    if (i <= buttons.Count)
                    {
                        control.Text = buttons[i - 1].GetLabel();
                    }
                    else
                    {
                        control.Text = "-/-";
                    }
                }
                else
                {
                    control.Enabled = false;
                    control.BackColor = Color.DimGray;
                    control.Text = "-/-";
                }
            }
            owner.GetClient().MessageReceived += ReceiveMessage;
        }

        private void ReceiveMessage(string message)
        {
            var command = message.Split(' ').First();
            var payload = String.Join(" ", message.Split(' ').Skip(1));

            switch (command)
            {
                case "debug-event":
                    DebugEvent(payload);
                    break;
            }
        }

        private void DebugEvent(string message)
        {
            if (selected != null)
            {
                var index = int.Parse(selected.Name.Substring(6));
                buttons[index - 1].Set(message);
                buttons[index - 1].fireStage = 2;
                selected.BackColor = Color.Tomato;
                SafeUpdateText(selected, buttons[index - 1].GetLabel());
                selected = null;
            }

            var temp = new ButtonConfig(message);
            for (var i=0; i<buttons.Count; i++)
            {
                var button = buttons[i];
                if (button.ToString() == temp.ToString())
                {
                    button.fireStage = 1;
                    (this.Controls.Find(string.Format("button{0}", (i + 1)), true).First() as Button).BackColor = Color.Tomato;
                }
            }
        }

        private void SafeUpdateText(Button control, string text)
        {
            if (control.InvokeRequired)
            {
                Action safeWrite = delegate { SafeUpdateText(control, text); };
                control.Invoke(safeWrite);
            }
            else
            {
                control.Text = text;
            }
        }

        private void ButtonHover(object sender, EventArgs e)
        {
            var control = sender as Button;
            var index = int.Parse(control.Name.Substring(6));
            toolTip1.Show(scripts[index - 1].Replace(">", ", ").Replace(".", " "), control);
        }

        private void buttonClick(object sender, EventArgs e)
        {
            var control = sender as Button;

            if (selected == control)
            {
                selected.BackColor = Color.RosyBrown;
                selected = null;
                return;
            }

            if (selected != null)
            {
                selected.BackColor = Color.RosyBrown;
            }

            control.BackColor = Color.LightGreen;
            selected = control;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            selectedButtonBlinkState = !selectedButtonBlinkState;
            if (selected != null)
            {
                selected.BackColor = selectedButtonBlinkState ? Color.LightGreen : Color.PaleGreen;
            }
            for (var i=0; i<buttons.Count; i++)
            {
                var button = buttons[i];
                if (button.fireStage > 0)
                {
                    button.fireStage -= 1;
                    (this.Controls.Find(string.Format("button{0}", (i + 1)), true).First() as Button).BackColor = button.fireStage > 0 ? Color.Salmon : Color.RosyBrown;
                }
            }
        }

        private void ConfigureButtonsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            owner.GetClient().Send("stop-debug-events");
            owner.GetClient().MessageReceived -= ReceiveMessage;
        }
    }
    public class ButtonConfig
    {
        public int fireStage = 0;

        string deviceName;
        int midiEvent;
        int note;

        public ButtonConfig(string source)
        {
            Set(source);
        }

        public void Set(string source)
        {
            var parts = source.Substring(source.IndexOf("event-")).Split(' ');
            deviceName = source.Substring(0, source.IndexOf("event-") - 1);
            midiEvent = int.Parse(parts[0].Substring(6));
            note = int.Parse(parts[1].Substring(5));
        }

        public string ToString()
        {
            return string.Format("{0} event-{1} note-{2}", deviceName, midiEvent, note);
        }

        public string GetLabel()
        {
            return string.Format("{0} [{1}] [{2}]", deviceName, midiEvent, note);
        }
    }
}
