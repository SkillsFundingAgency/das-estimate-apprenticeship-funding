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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var dir = new DirectoryInfo(@"..\..\..\SFA.DAS.ForecastingTool.Web\App_Data");
            txtOutputDir.Text = dir.FullName;
        }

        private void btnBrowseOutputDir_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(txtOutputDir.Text))
                {
                    dlg.SelectedPath = txtOutputDir.Text;
                }
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtOutputDir.Text = dlg.SelectedPath;
                }
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtOutputDir.Text))
            {
                if (MessageBox.Show("Output directory does not exist. Create it?", "Create Directory?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                Directory.CreateDirectory(txtOutputDir.Text);
            }

            var standardsPath = Path.Combine(txtOutputDir.Text, "Standards.json");
            if (File.Exists(standardsPath))
            {
                if (MessageBox.Show($"{standardsPath} already exists. Overwrite it?", "Overwrite?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }

            Process(txtInput.Text.Trim(), StandardsFormat.Poc, standardsPath);
        }


        private void Process(string input, StandardsFormat format, string standardsPath)
        {
            try
            {
                var standards = ProcessPocData(input);
                var data = JsonConvert.SerializeObject(standards, Formatting.Indented);

                using (var stream = new FileStream(standardsPath, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(data);
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
        private Standard[] ProcessPocData(string input)
        {
            input = ParseJsDeclarationToJson(input);

            var pocStandards = JsonConvert.DeserializeObject<PocStandard[]>(input);

            return pocStandards.Select(s => new Standard
            {
                Code = s.Code,
                Name = s.Name,
                Duration = 12,
                Price = s.Price
            }).ToArray();
        }
        private string ParseJsDeclarationToJson(string input)
        {
            if (input.Length < 1)
            {
                return input;
            }

            if (input.StartsWith("var"))
            {
                var arrayStartIndex = input.IndexOf('[');
                if (arrayStartIndex > 0)
                {
                    input = input.Substring(arrayStartIndex);
                }
            }
            if (input.EndsWith(";"))
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }

        private enum StandardsFormat
        {
            Poc
        }
    }
}
