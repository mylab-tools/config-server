using System.Text;
using System.Xml.Linq;

namespace MyLab.ConfigServer.Tools
{
    static class XElementPathProvider
    {
        public static string Provide(XElement xElement)
        {
            var sb = new StringBuilder(xElement.Name.LocalName);
            ExtractXElementBasePath(xElement, sb);
            return sb.ToString();
        }

        static void ExtractXElementBasePath(XElement xElement, StringBuilder sb)
        {
            var parent = xElement.Parent;

            if (parent == null)
            {
                sb.Insert(0, "/");
                return;
            }

            sb.Insert(0, parent.Name.LocalName + "/");
            ExtractXElementBasePath(parent, sb);
        }
    }
}