using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ASP_Controls_Declaration_Generator_Desktop.Models;

namespace ASP_Controls_Declaration_Generator_Desktop
{
    public partial class Generator : Form
    {
        Parser AspxFileParser;


        public Generator()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog1.FileName;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            AspxFileParser = new Parser(txtFilePath.Text);

            AspxFileParser.ParseFile();

            StringBuilder sb = new StringBuilder();

            foreach (var item in AspxFileParser.GetControls())
            {
                sb.AppendLine(item.ToString());
            }

            txtDeclarations.Text = sb.ToString();
            
        }
    }
}
