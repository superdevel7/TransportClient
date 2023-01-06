using CompiledTechnologies.Network;
using System;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using static System.Net.WebRequestMethods;

namespace Transport
{
    public partial class Form1 : Form
    {
        public string path;

        public Form1()

        {
            InitializeComponent();
            path = string.Empty;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var dir = Directory.GetParent((Directory.GetCurrentDirectory()));
            path = dir.FullName;
            path = path.Replace(@"Transport\bin", @"Files\");
            lblPath.Text = path;
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            btnTransfer.Enabled = false;
            Local();
            btnTransfer.Enabled = true;
        }
        private void Local()
        {
           
            lb.Items.Clear();
            ////////////////////////////////////////////////////////////////////////////////////////
            var watch = System.Diagnostics.Stopwatch.StartNew();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(path + @"A.xml");
            using (MemoryStream ms = new MemoryStream())
            {
                xmldoc.Save(ms);
                Transporter.SendFile(@"c:\apps\test\AT.xml", ms);
            }
            watch.Stop();
            lb.Items.Add("File A: " + watch.Elapsed.ToString(@"m\:ss\.fff"));
            lb.Refresh();
            ////////////////////////////////////////////////////////////////////////////////////////
            watch = System.Diagnostics.Stopwatch.StartNew();
            xmldoc = new XmlDocument();
            xmldoc.Load(path + @"B.xml");
            using (MemoryStream ms = new MemoryStream())
            {
                xmldoc.Save(ms);
                Transporter.SendFile(@"c:\apps\test\BT.xml", ms);
            }
            watch.Stop();
            lb.Items.Add("File B: " + watch.Elapsed.ToString(@"m\:ss\.fff"));
            lb.Refresh();
            ////////////////////////////////////////////////////////////////////////////////////////
            watch = System.Diagnostics.Stopwatch.StartNew();
            xmldoc = new XmlDocument();
            xmldoc.Load(path + @"C.xml");
            using (MemoryStream ms = new MemoryStream())
            {
                xmldoc.Save(ms);
                Transporter.SendFile(@"c:\apps\test\CT.xml", ms);
            }
            watch.Stop();
            lb.Items.Add("File C: " + watch.Elapsed.ToString(@"m\:ss\.fff"));
            lb.Refresh();
            ////////////////////////////////////////////////////////////////////////////////////////
            watch = System.Diagnostics.Stopwatch.StartNew();
            xmldoc = new XmlDocument();
            xmldoc.Load(path + @"D.xml");
            using (MemoryStream ms = new MemoryStream())
            {
                xmldoc.Save(ms);
                Transporter.SendFile(@"c:\apps\test\DT.xml", ms);
            }
            watch.Stop();
            lb.Items.Add("File D: " + watch.Elapsed.ToString(@"m\:ss\.fff"));
            lb.Refresh();
            ////////////////////////////////////////////////////////////////////////////////////////
            watch = System.Diagnostics.Stopwatch.StartNew();
            xmldoc = new XmlDocument();
            xmldoc.Load(path + @"E.xml");
            using (MemoryStream ms = new MemoryStream())
            {
                xmldoc.Save(ms);
                Transporter.SendFile(@"c:\apps\test\ET.xml", ms);
            }
            watch.Stop();
            lb.Items.Add("File E: " + watch.Elapsed.ToString(@"m\:ss\.fff"));
            lb.Refresh();
        }
    }
}
