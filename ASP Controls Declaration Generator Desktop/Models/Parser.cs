using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASP_Controls_Declaration_Generator_Desktop.Models
{
    public class Parser
    {

        private bool OutputToFile
        {
            get
            {
                return (OutputFilePath == null || OutputFilePath == "") ? false : true;
            }
        }

        private string FilePath { get; set; }
        private string OutputFilePath { get; set; }

        private string FileText { get; set; }
        private string CleanedText { get; set; }
        private string AdjustedNamespacesText { get; set; }

        private List<ControlObject> _controls = new List<ControlObject>();
        private MatchCollection ControlTagMatches;

        private const string ASPTag = @"<%(.|\.)*%>";
        private const string NamespacedTag = @"</?([a-zA-Z0-9_])*:([a-zA-Z0-9_])*";

        public Parser(string filepath)
        {
            bool FileExists = File.Exists(filepath);

            if (!FileExists)
            {
                throw new FileNotFoundException();
            }

            FilePath = filepath;
            FileText = File.ReadAllText(filepath);

        }

        public Parser(string filepath, string outputfilepath)
        {
            bool FileExists = File.Exists(filepath);
            //bool OutputFilePathExists = File.Exists(outputfilepath);

            if (!FileExists)
            {
                throw new FileNotFoundException();
            }

            FilePath = filepath;
            FileText = File.ReadAllText(filepath);

            OutputFilePath = outputfilepath;
        }

        public void SaveToFile()
        {
            if (OutputToFile)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var line in _controls)
                {
                    sb.AppendLine(line.ToString());
                }

                File.WriteAllText(OutputFilePath, sb.ToString());
            }

        }

        private void AdjustNamespaces()
        {

            ControlTagMatches = Regex.Matches(CleanedText, NamespacedTag);

            foreach (Match m in ControlTagMatches)
            {
                CleanedText = CleanedText.Replace(m.Value, m.Value.Replace(":", "."));
            }

            AdjustedNamespacesText = CleanedText;
        }


        public void ParseFile()
        {

            //Clean the FileText, Remove <% %> tags
            CleanedText = Regex.Replace(FileText, ASPTag, "");

            //replace colons with periods so that HAP Works
            AdjustNamespaces();

            //Initialize HAP
            var Document = new HtmlDocument();
            Document.LoadHtml(AdjustedNamespacesText);

            foreach (Match m in ControlTagMatches)
            {
                string fulltag = m.Value.Replace("<", "").Replace("/", "").Replace(":", ".").ToLower();

                string selector = $"//{fulltag}[@runat=\"server\"][@id]";
                var elements = Document.DocumentNode.SelectNodes(selector);

                if (elements != null) { 

                    foreach (HtmlNode e in elements)
                    {
                        string Namespace = e.Name.Split('.')[0];
                        string TagName = e.Name.Split('.')[1];
                        string ID = e.Id;

                        // check if there is already an entry for this control
                        if (!ControlExists(Namespace, TagName, ID))
                        {
                            //add the control
                            _controls.Add(new ControlObject(Namespace, TagName, ID));
                        }

                    }

                }



            }

        }

        private bool ControlExists(string Namespace, string Tag, string ID)
        {
            foreach (var c in _controls)
            {
                if (c.ControlNamespace == Namespace && c.ControlTag == Tag && c.ControlID == ID)
                {
                    return true;
                }

            }

            return false;
        }

        public List<ControlObject> GetControls()
        {
            return _controls;
        }
    }
}
