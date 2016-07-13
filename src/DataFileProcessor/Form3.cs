using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace DataFileProcessor
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

            var dir = new DirectoryInfo(@"..\..\..\SFA.DAS.ForecastingTool.Web\App_Data");
            txtDir.Text = dir.FullName;
        }

        private void btnSrcBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Csv Files|*.csv|All Files|*.*";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtSrc.Text = dlg.FileName;
                }
            }
        }
        private void btnDstBrowse_Click(object sender, EventArgs e)
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
                var standardsPath = Path.Combine(txtDir.Text, "Standards.json");
                if (File.Exists(standardsPath))
                {
                    if (MessageBox.Show($"{standardsPath} already exists. Overwrite it?", "Overwrite?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                var standards = new List<Standard>();
                using (var stream = new FileStream(txtSrc.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var columns = ReadCsvLine(reader);
                        try
                        {
                            standards.Add(new Standard
                            {
                                Code = int.Parse(columns[0]),
                                Name = columns[1],
                                Duration = int.Parse(columns[2]),
                                Price = int.Parse(columns[3])
                            });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed on line {standards.Count + 1} - {ex.Message}", ex);
                        }
                    }
                }

                var resultJson = JsonConvert.SerializeObject(standards.Where(s => s.Duration > 0).OrderBy(s => s.Name).ToArray(), Formatting.Indented);
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


        private string[] ReadCsvLine(StreamReader reader)
        {
            var line = reader.ReadLine();
            var columns = new List<string>();
            var value = new StringBuilder();
            var inDelimitedCell = false;
            foreach (var c in line)
            {
                if (c == '"')
                {
                    inDelimitedCell = !inDelimitedCell;
                }
                else if (c == ',' && !inDelimitedCell)
                {
                    columns.Add(value.ToString());
                    value.Clear();
                }
                else
                {
                    value.Append(c);
                }
            }
            columns.Add(value.ToString());

            return columns.ToArray();
        }
    }
}
