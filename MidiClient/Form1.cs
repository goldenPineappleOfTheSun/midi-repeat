using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Media;

namespace MidiClient
{
    /*
    0. запускает питон сервер
    1. при включении считывает *.conf файлы из текущей папки
    2. показывает на экране список считанных файлов
    3. пользователь выбирает
    4. из файла читаются: длина трека, список устройств, скрипты, кнопки
    5. если чего-то не хватает, что-то не парсится, то пусть падает - тут не страшно
    6. происходит подключение к серверу с просьбой уточнить, подключены ли физически эти устройства
    7. инфа об этом сохраняется
    8. отрисовываются все нужные вещи: дорожка с указателем, цветные кассеты, у каждой кассеты указано входное устройство (с кнопкой монитора), кнопки
    9. рядом с устройствами и рядом с кнопками есть кнопка перенастройки
    10. для перенастройки устройств открывается доп окно, клиент спрашивает сервер о доступных устройствах и выдаёт их в виде списка
    11. для перенастройки кнопок открывается доп окно и в нём кнопки. там можно переназначить
    12. потом ктото жмет кнопку подключится и вся инфа отсылается в сервер (кнопка пропадает)
    13. сервер иногда присылает обратную связь и от этого обновляется ui в винформс
    14. дорожка сама считает время без сервера
    15. когда сервер останавливается он присылает на клиент инфу об остановке (кнопка подкл появляется)
    16. если клиент сам нажимает кнопку подключения (или программа закрыв), то он шлёт сигнал об отключении
     */

    public partial class Form1 : Form
    {
        string configFile;
        /* list of input ids "MPK mini 3|MPK mini 3|MPK mini 18" */

        List<string> names;

        string wiring;
        /* list of scripts "monitor.bass/record.bass>record.drums/play.drums>play.bass" */
        string script;
        /* list of buttons "MPK mini 3 event-153 note-36|MPK mini 3 event-153 note-37|MPK mini 3 event-153 note-38" */
        /* {name} event-{midievent} note-{midipayload1}*/
        string buttons;
        int loopSize;
        int beats;
        bool testMonitor;
        bool started;
        bool metronome = true;
        bool backingTrack = false;
        bool backingTrackAwaits = false;
        string backingTrackFile;
        SoundPlayer backingTrackWav; 
        /* result of the check-wiring */
        string wiresCheckErrors = "check has not been done yet";
        List<TapeInfo> tapes = new List<TapeInfo>();
        Dictionary<string, List<TapeInfo>> nameToTapeMapping = new Dictionary<string, List<TapeInfo>>();

        SocketClient client;
        ConfigListForm configListForm;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var fullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var path = fullPath.Substring(0, fullPath.Length - 14);
            configListForm = new ConfigListForm();
            configListForm.ShowDialog();
            configFile = path + configListForm.result + ".midiconf";
            backingTrackFile = path + configListForm.result + ".wav";
            ReadFile();

            var devices = wiring.Split('|');
            for (var i = 0; i < 12 && i < devices.Length; i++)
            {
                var control = this.Controls.Find(string.Format("pictureBox{0}", (i + 1)), true).First() as PictureBox;
                var tape = new TapeInfo(devices[i], control);
                tapes.Add(tape);

                if (!nameToTapeMapping.ContainsKey(devices[i]))
                {
                    nameToTapeMapping.Add(devices[i], new List<TapeInfo>());
                }
                nameToTapeMapping[devices[i]].Add(tape);

                var match = new Regex(@"(.+) \[[ABCDEFGH]\]$").Match(devices[i]);
                if (match.Groups.Count > 1)
                {
                    if (!nameToTapeMapping.ContainsKey(match.Groups[1].Value))
                    {
                        nameToTapeMapping.Add(match.Groups[1].Value, new List<TapeInfo>());
                    }
                    nameToTapeMapping[match.Groups[1].Value].Add(tape);
                }
            }

            InitialTapesColors();

            client = new SocketClient();
            client.MessageReceived += ReceiveMessage;
            Connect();

            SendWiringCheck();
        }

        private void InitialTapesColors()
        {
            var devices = wiring.Split('|');
            for (var i = 0; i < 12 && i < devices.Length; i++)
            {
                var control = this.Controls.Find(string.Format("pictureBox{0}", (i + 1)), true).First() as PictureBox;
                control.BackColor = Color.MistyRose;
            }

            for (var i = devices.Length; i < 12; i++)
            {
                var control = this.Controls.Find(string.Format("pictureBox{0}", (i + 1)), true).First() as PictureBox;
                control.BackColor = Color.DimGray;
            }
        }

        public SocketClient GetClient()
        {
            return client;
        }

        public List<TapeInfo> GetTapeInfo(string name)
        {
            return nameToTapeMapping[name];
        }

        private void ReadFile() 
        {
            var state = "devices";
            var names = new List<string>();
            var devices = new List<string>();
            var scripts = new List<string>();
            var buts = "";
            var portIndex = 1;
            var size = 0;
            var ticks = 0;

            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(configFile))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (state == "devices")
                    {
                        if (new Regex(@"^\s*$").Match(line).Success)
                        {
                            state = "size";
                            continue;
                        }

                        var match = new Regex(@"([\w-]+): (.+)").Match(line);
                        if (match.Success)
                        {
                            names.Add(match.Groups[1].Value);
                            devices.Add(match.Groups[2].Value);
                        }
                        else
                        {
                            match = new Regex(@"([\w-]+):").Match(line);
                            names.Add(match.Groups[1].Value);
                            devices.Add("");
                        }
                        portIndex += 1;
                        continue;
                    }

                    if (state == "size") 
                    {
                        var match = new Regex(@"size: (\d+)ms").Match(line);
                        size = int.Parse(match.Groups[1].Value);
                        state = "beats";
                        continue;
                    }

                    if (state == "beats")
                    {
                        if (new Regex(@"^\s*$").Match(line).Success)
                        {
                            state = "scripts";
                            continue;
                        }

                        var match = new Regex(String.Format(@"beats: (\d+)", portIndex)).Match(line);
                        ticks = int.Parse(match.Groups[1].Value);

                        continue;
                    }

                    if (state == "scripts")
                    {
                        if (new Regex(@"^\s*$").Match(line).Success)
                        {
                            state = "buttons";
                            continue;
                        }

                        scripts.Add(line.Replace(", ", ">").Replace(",", ">").Replace(' ', '.'));
                        portIndex += 1;
                        continue;
                    }

                    if (state == "buttons")
                    {
                        buts = line;
                    }
                }
            }

            if (File.Exists(backingTrackFile))
            {
                backingTrackWav = new SoundPlayer(backingTrackFile);
                checkBox1.Enabled = true;
            }

            this.names = names;
            wiring = String.Join("|", devices.ToArray());
            script = String.Join("/", scripts.ToArray());
            buttons = buts;
            loopSize = size;
            beats = ticks; 
        }

        private void SaveFile()
        {
            var text = new StringBuilder();
            var wireArray = wiring.Split('|');
            for (var i=0; i<wireArray.Length; i++)
            {
                var wire = wireArray[i];
                text.AppendFormat("{0}: {1}{2}", names[i], wireArray[i], Environment.NewLine);
            }
            for (var i=wireArray.Length; i<names.Count; i++)
            {
                text.AppendFormat("{0}:{1}", names[i], Environment.NewLine);
            }
            text.Append(Environment.NewLine);

            text.AppendFormat("size: {0}ms{1}", loopSize, Environment.NewLine);
            text.AppendFormat("beats: {0}{1}", beats, Environment.NewLine);
            text.Append(Environment.NewLine);

            var scriptsArray = script.Replace(">", ", ").Replace(".", " ").Split('/');
            for (var i = 0; i < scriptsArray.Length; i++)
            {
                var script = scriptsArray[i];
                text.AppendFormat("{0}{1}", scriptsArray[i], Environment.NewLine);
            }
            text.Append(Environment.NewLine);

            text.AppendFormat("{0}{1}", buttons, Environment.NewLine);

            File.WriteAllText(configFile, text.ToString());
        }

        private void SendWiringCheck()
        {
            client.Send("check-wiring " + wiring.Replace(" ", "%20"));
        }

        private void Connect()
        {
            client.Connect("localhost", 12345);
        }

        private void Disonnect()
        {
            if (client != null)
            {
                client.Send("exit");
                client.Close();
            }
        }

        private void ReceiveMessage(string message)
        {
            var command = message.Split(' ').First();
            var payload = String.Join(" ", message.Split(' ').Skip(1));

            Console.WriteLine(string.Format("Received: {0}", message));

            switch (command) 
            {
                case "callback-check-wiring":
                    WiringCheckCallback(payload);
                    break;
                case "callback-request-io":
                    RequestIoCallback(payload);
                    break;
                case "debug-event":
                    DebugEvent(payload);
                    break;
                case "tape-monitor":
                    MonitorEvent(payload);
                    break;
                case "tape-play":
                    PlayEvent(payload);
                    break;
                case "tape-record":
                    RecordEvent(payload);
                    break;
                case "tape-mute":
                    MuteEvent(payload);
                    break;
                case "metronome-on-event":
                    MetronomeSwitchEvent(true);
                    break;
                case "metronome-off-event":
                    MetronomeSwitchEvent(false);
                    break;
                case "metronome-tick":
                    MetronomeTickEvent(payload);
                    break;
                case "loop-event":
                    LoopEvent();
                    break;
            }
        }

        private void WiringCheckCallback(string message)
        {
            if (message.ToLower() == "ok")
            {
                wiresCheckErrors = "";
            } 
            else
            {
                wiresCheckErrors = message.Replace("\n", Environment.NewLine);
                WiringErrorHandling();
            }
            InitialTapesColors();
        }

        private void RequestIoCallback(string message)
        {
            var parts = message.Split(';');
            var outputs = int.Parse(parts[0].Split(':')[1]);
            var inputs = parts[1].Substring(7).Split('|').Where(x => !x.Contains("loopMIDI")).ToList();
            var current = wiring.Split('|').ToList();
            var form = new ConfigureWiringForm(outputs, inputs, current, names);
            form.ShowDialog();
            wiring = string.Join("|", form.current);
            while (wiring.EndsWith("|"))
            {
                wiring = wiring.Substring(0, wiring.Length - 1);
            }
            SaveFile();
            SendWiringCheck();
            InitialTapesColors();
        }

        private void DebugEvent(string message)
        {
            var name = message.Substring(0, message.IndexOf("event-") - 1);
            var tapeInfo = GetTapeInfo(name);
            tapeInfo.ForEach(x => x.DebugFire());
        }

        private void MonitorEvent(string message)
        {
            var index = int.Parse(message);
            var control = this.Controls.Find(string.Format("pictureBox{0}", (index)), true).First() as PictureBox;
            control.BackColor = Color.Turquoise;
        }

        private void RecordEvent(string message)
        {
            var index = int.Parse(message);
            var control = this.Controls.Find(string.Format("pictureBox{0}", (index)), true).First() as PictureBox;
            control.BackColor = Color.Salmon;
        }

        private void PlayEvent(string message)
        {
            var index = int.Parse(message);
            var control = this.Controls.Find(string.Format("pictureBox{0}", (index)), true).First() as PictureBox;
            control.BackColor = Color.LightGreen;
        }

        private void MuteEvent(string message)
        {
            var index = int.Parse(message);
            var control = this.Controls.Find(string.Format("pictureBox{0}", (index)), true).First() as PictureBox;
            control.BackColor = Color.MistyRose;
        }

        private void MetronomeSwitchEvent(bool value)
        {
            metronome = value;
            metronomeButton.BackColor = metronome ? Color.PaleVioletRed : Color.MistyRose;
        }

        private void MetronomeTickEvent(string message)
        {
            var n = int.Parse(message);
            SafeUpdateProgress(n);
        }

        private void LoopEvent()
        {
            if (backingTrackAwaits)
            {
                backingTrackAwaits = false;
                PlayBackingTrack();
            }
        }

        private bool WiringErrorHandling()
        {
            if (MessageBox.Show(wiresCheckErrors, "Попробовать снова?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                client.Send("hard-reset-midi");
                Thread.Sleep(200);
                SendWiringCheck();
                return true;
            }
            else
            {
                // неисправленные ошибки подключения!
                return false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disonnect();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            configPanelGroupBox.Visible = !configPanelGroupBox.Visible;
        }

        private void basicConfigButton_Click(object sender, EventArgs e)
        {
            configPanelGroupBox.Visible = false;
            Connect();
            var form = new ConfigureBasicsForm(loopSize, beats);
            form.ShowDialog();
            loopSize = form.size;
            beats = form.beats;
            SaveFile();
        }

        private void wiringConfigButton_Click(object sender, EventArgs e)
        {
            configPanelGroupBox.Visible = false;
            Connect();
            client.Send("request-io");
        }

        private void buttonsConfigButton_Click(object sender, EventArgs e)
        {
            configPanelGroupBox.Visible = false;
            Connect();
            client.Send("start-debug-events " + wiring.Replace(" ", "%20"));
            var form = new ConfigureButtonsForm(buttons.Split('|').ToList(), script.Split('/').ToList(), this);
            form.ShowDialog();
            buttons = string.Join("|", form.buttons.Select(x => x.ToString()));
            SaveFile();
        }

        private void testDevicesButton_Click(object sender, EventArgs e)
        {
            var control = sender as Button;
            if (!testMonitor)
            {
                Connect();
                testMonitor = true;
                client.Send("start-debug-events " + wiring.Replace(" ", "%20"));
                startButton.Enabled = false;
                control.BackColor = Color.LightGreen;
                startButton.BackColor = Color.DimGray;
            } 
            else
            {
                Connect();
                testMonitor = false;
                client.Send("stop-debug-events");
                startButton.Enabled = true;
                control.BackColor = Color.MistyRose;
                startButton.BackColor = Color.MistyRose;
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (wiresCheckErrors != "")
            {
                if (!WiringErrorHandling())
                {
                    return;
                }
            }

            var control = sender as Button;
            if (!started)
            {
                Connect();
                started = true;
                client.Send(string.Format("set-basics {0} {1}", loopSize, beats));
                Thread.Sleep(300);
                client.Send(string.Format("set-wiring {0}", wiring.Replace(" ", "%20")));
                Thread.Sleep(300);
                client.Send(string.Format("set-scripts {0}", script));
                Thread.Sleep(300);
                client.Send(string.Format("set-buttons {0}", buttons.Replace(" ", "%20")));
                Thread.Sleep(300);
                client.Send("start");
                testDevicesButton.Enabled = false;
                control.BackColor = Color.LightGreen;
                testDevicesButton.BackColor = Color.DimGray;
                metronome = true;
                metronomeButton.Enabled = true;
                metronomeButton.BackColor = metronome ? Color.PaleVioletRed : Color.MistyRose;
                if (backingTrack)
                {
                    backingTrackAwaits = true;
                }
            }
            else
            {
                Connect();
                started = false;
                client.Send("stop");
                testDevicesButton.Enabled = true;
                control.BackColor = Color.MistyRose;
                testDevicesButton.BackColor = Color.MistyRose;
                metronomeButton.Enabled = false;
                metronomeButton.BackColor = Color.DimGray;
                backingTrackWav.Stop();
                InitialTapesColors();
            }
        }

        private void PlayBackingTrack()
        {
            if (backingTrackWav != null)
            {
                backingTrackWav.Play();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var tape in tapes)
            {
                tape.Timer();
            }
        }

        private void metronomeButton_Click(object sender, EventArgs e)
        {
             SwitchMetronome();
        }

        private void SwitchMetronome()
        {
            if (!started)
            {
                return;
            }
            if (!metronome)
            {
                metronome = true;
                client.Send("enable-metronome");
                metronomeButton.BackColor = Color.PaleVioletRed;
            }
            else
            {
                metronome = false;
                client.Send("disable-metronome");
                metronomeButton.BackColor = Color.MistyRose;
            }
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {

        }

        private void SafeUpdateProgress(int n)
        {
            if (panel1.InvokeRequired)
            {
                Action safeWrite = delegate { SafeUpdateProgress(n); };
                panel1.Invoke(safeWrite);
            }
            else
            {
                if (beats - n < 9)
                {
                    countdown.Text = (beats - n).ToString();
                    if (n % 4 == 0)
                    {
                        countdown.Text = countdown.Text + "!";
                    }
                    if (beats - n < 5)
                    {
                        countdown.BackColor = Color.Salmon;
                    }
                    else
                    {
                        countdown.BackColor = Color.DimGray;
                    }
                }
                else
                {
                    if (n % 4 == 0)
                    {
                        countdown.BackColor = Color.DimGray;
                        countdown.Text = "!";
                    }
                    else
                    {
                        countdown.BackColor = Color.DimGray;
                        countdown.Text = ".";
                    }
                }
                panel2.Width = (int)((float)(panel1.Width) / (float)beats) * (n + 1);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.ShiftKey)
            {
                SwitchMetronome();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            backingTrack = !backingTrack;
        }
    }

    public class TapeInfo
    {
        int fireStage;
        PictureBox control;
        string name;

        public TapeInfo(string name, PictureBox control)
        {
            this.fireStage = 0;
            this.name = name;
            this.control = control;
        }

        internal void DebugFire()
        {
            fireStage = 10;
        }

        internal void Timer()
        {
            if (fireStage == 1)
            {
                control.BackColor = Color.MistyRose;
                fireStage = 0;
                return;
            }
            if (fireStage > 0)
            {
                var progress = fireStage / 10f;
                var r = (int)(255 + (64 - 255) * progress);
                var g = (int)(228 + (224 - 228) * progress);
                var b = (int)(225 + (208 - 225) * progress);
                control.BackColor = Color.FromArgb(255, r, g, b);
                fireStage -= 1;
            }
        }
    }
}
