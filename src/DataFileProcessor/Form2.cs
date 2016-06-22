using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace DataFileProcessor
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            var dir = new DirectoryInfo(@"..\..\..\SFA.DAS.ForecastingTool.Web\App_Data");
            txtDir.Text = dir.FullName;
        }

        private void btnJsonBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Json Files|*.json|All Files|*.*";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtJson.Text = dlg.FileName;
                }
            }
        }
        private void btnCsvBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Csv Files|*.csv|All Files|*.*";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtCsv.Text = dlg.FileName;
                }
            }
        }
        private void btnDirBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtDir.Text = dlg.SelectedPath;
                }
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(txtDir.Text))
                {
                    if (MessageBox.Show("Output directory does not exist. Create it?", "Create Directory?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                    Directory.CreateDirectory(txtDir.Text);
                }

                var standardsPath = Path.Combine(txtDir.Text, "Standards.json");
                if (File.Exists(standardsPath))
                {
                    if (MessageBox.Show($"{standardsPath} already exists. Overwrite it?", "Overwrite?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                var standards = JsonConvert.DeserializeObject<PocStandard[]>(File.ReadAllText(txtJson.Text))
                    .Select(s => new Standard
                    {
                        Code = s.Code,
                        Name = s.Name,
                        Duration = 12,
                        Price = s.Price
                    }).ToArray();
                using (var stream = new FileStream(txtCsv.Text, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream))
                {
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var data = reader.ReadLine().Split(',');
                        int code;
                        int duration;

                        int.TryParse(data[1], out code);
                        int.TryParse(data[2], out duration);

                        var standard = standards.SingleOrDefault(s => s.Code == code);
                        if (standard != null)
                        {
                            standard.Duration = duration;
                        }
                    }
                }

                var resultJson = JsonConvert.SerializeObject(standards.Where(s => s.Duration > 0).ToArray(), Formatting.Indented);
                using (var stream = new FileStream(standardsPath, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(resultJson);
                    writer.Flush();
                    writer.Close();
                }

                MessageBox.Show($"Saved to {standardsPath}", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
