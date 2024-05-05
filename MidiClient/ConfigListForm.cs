using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MidiClient
{
    public partial class ConfigListForm : Form
    {
        public string result = "";

        public ConfigListForm()
        {
            InitializeComponent();
        }

        private List<FileInfo> FindConfigFiles()
        {
            var fullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var path = fullPath.Substring(0, fullPath.Length - 14);
            var d = new DirectoryInfo(path);
            return d.GetFiles("*.midiconf").ToList();
        }

        private void RenderConfigOptions(List<FileInfo> list)
        {
            foreach (var file in list)
            {
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.ItemHeight = 24;
            listBox1.DrawItem += new DrawItemEventHandler(listBox_DrawItem);
        }

        private void ConfigListForm_Load(object sender, EventArgs e)
        {
            RenderConfigOptions(FindConfigFiles());
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox list = (ListBox)sender;
            if (e.Index > -1)
            {
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e = new DrawItemEventArgs(e.Graphics,
                                              e.Font,
                                              e.Bounds,
                                              e.Index,
                                              e.State ^ DrawItemState.Selected,
                                              Color.Black,
                                              Color.MistyRose); // Choose the color.

                object item = list.Items[e.Index];
                e.DrawBackground();
                e.DrawFocusRectangle();
                Brush brush = new SolidBrush(e.ForeColor);
                SizeF size = e.Graphics.MeasureString(item.ToString(), e.Font);
                e.Graphics.DrawString(item.ToString(), e.Font, brush, e.Bounds.Left + (e.Bounds.Width / 2 - size.Width / 2), e.Bounds.Top + (e.Bounds.Height / 2 - size.Height / 2));
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                result = listBox1.SelectedItem.ToString();
                this.Close();
            }
        }
    }
}
