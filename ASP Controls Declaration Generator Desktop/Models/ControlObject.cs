using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP_Controls_Declaration_Generator_Desktop.Models
{
    public class ControlObject
    {
        public string ControlTag { get; set; }
        public string ControlID { get; set; }
        public string ControlNamespace { get; set; }

        private const string AccessModifiers = "Protected WithEvents";
        private const string PreType = "As";

        private string GetControlTypeName()
        {
            switch (ControlNamespace)
            {
                case "asp":
                    return ControlTag;
                default:
                    return $"{ControlNamespace}.{ControlTag}";
            }
        }

        public override string ToString()
        {
            return $"{AccessModifiers} {ControlID} {PreType} {GetControlTypeName()}";
        }

        public ControlObject()
        {

        }

        public ControlObject(string Namespace, string Tag, string ID)
        {
            ControlNamespace = Namespace;
            ControlTag = Tag;
            ControlID = ID;
        }

    }

}
