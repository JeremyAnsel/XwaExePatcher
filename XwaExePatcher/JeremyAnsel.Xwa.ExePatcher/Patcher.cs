using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace JeremyAnsel.Xwa.ExePatcher
{
    public class Patcher
    {
        private const string xwaExeVersion = @"X-Wing Alliance\V2.0";

        private const int xwaExeVersionOffset = 0x200E19;

        private static readonly byte[] xwaExeVersionBytes = Encoding.ASCII.GetBytes(Patcher.xwaExeVersion);

        public Patcher()
        {
            this.Patches = new List<Patch>();
        }

        public IList<Patch> Patches { get; private set; }

        public static bool IsApplied(string exeFileName, Patch patch)
        {
            if (exeFileName == null)
            {
                throw new ArgumentNullException("exeFileName");
            }

            if (patch == null)
            {
                throw new ArgumentNullException("patch");
            }

            using (FileStream file = File.Open(exeFileName, FileMode.Open, FileAccess.Read))
            {
                Patcher.FileVerifyExeVersion(file);

                foreach (var item in patch.Items)
                {
                    if (!Patcher.FileCheckBytes(file, item.Offset, item.NewValues))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool CanBeApplied(string exeFileName, Patch patch)
        {
            if (exeFileName == null)
            {
                throw new ArgumentNullException("exeFileName");
            }

            if (patch == null)
            {
                throw new ArgumentNullException("patch");
            }

            using (FileStream file = File.Open(exeFileName, FileMode.Open, FileAccess.Read))
            {
                Patcher.FileVerifyExeVersion(file);

                foreach (var item in patch.Items)
                {
                    if (!Patcher.FileCheckBytes(file, item.Offset, item.OldValues))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static Patcher Read(string fileName)
        {
            Patcher patcher = new Patcher();

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            foreach (XmlNode patchNode in document.SelectNodes("/ArrayOfPatch/Patch"))
            {
                XmlNode node;

                Patch patch = new Patch();

                patch.Name = patchNode.Attributes.GetNamedItem("Name").Value;

                node = patchNode.Attributes.GetNamedItem("Description");
                patch.Description = node != null ? node.Value : null;

                foreach (XmlNode itemNode in patchNode.SelectNodes("Item"))
                {
                    patch.Items.Add(new PatchItem
                    {
                        OffsetString = itemNode.Attributes.GetNamedItem("Offset").Value,
                        OldValuesString = itemNode.Attributes.GetNamedItem("From").Value,
                        NewValuesString = itemNode.Attributes.GetNamedItem("To").Value
                    });
                }

                patcher.Patches.Add(patch);
            }

            return patcher;
        }

        public void Write(string fileName)
        {
            XmlDocument document = new XmlDocument();
            document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", null));

            var rootNode = document.CreateElement("ArrayOfPatch");

            foreach (Patch patch in this.Patches)
            {
                var patchNode = document.CreateElement("Patch");

                patchNode.SetAttribute("Name", patch.Name);

                if (!string.IsNullOrEmpty(patch.Description))
                {
                    patchNode.SetAttribute("Description", patch.Description);
                }

                foreach (PatchItem item in patch.Items)
                {
                    var itemNode = document.CreateElement("Item");

                    itemNode.SetAttribute("Offset", item.OffsetString);
                    itemNode.SetAttribute("From", item.OldValuesString);
                    itemNode.SetAttribute("To", item.NewValuesString);

                    patchNode.AppendChild(itemNode);
                }

                rootNode.AppendChild(patchNode);
            }

            document.AppendChild(rootNode);
            document.Save(fileName);
        }

        public void Apply(string exeFileName, bool writeNewValues)
        {
            if (exeFileName == null)
            {
                throw new ArgumentNullException("exeFileName");
            }

            using (FileStream file = File.Open(exeFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                Patcher.FileVerifyExeVersion(file);

                foreach (var patch in this.Patches)
                {
                    foreach (var item in patch.Items)
                    {
                        if (writeNewValues)
                        {
                            if (!Patcher.FileCheckBytes(file, item.Offset, item.OldValues))
                            {
                                throw new InvalidDataException();
                            }
                        }
                        else
                        {
                            if (!Patcher.FileCheckBytes(file, item.Offset, item.NewValues))
                            {
                                throw new InvalidDataException();
                            }
                        }
                    }

                    foreach (var item in patch.Items)
                    {
                        if (writeNewValues)
                        {
                            file.Seek(item.Offset, SeekOrigin.Begin);
                            file.Write(item.NewValues, 0, item.NewValues.Length);
                        }
                        else
                        {
                            file.Seek(item.Offset, SeekOrigin.Begin);
                            file.Write(item.OldValues, 0, item.OldValues.Length);
                        }
                    }
                }
            }
        }

        private static void FileVerifyExeVersion(FileStream file)
        {
            if (!Patcher.FileCheckBytes(file, Patcher.xwaExeVersionOffset, Patcher.xwaExeVersionBytes))
            {
                throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "{0} not found", Patcher.xwaExeVersion));
            }
        }

        private static bool FileCheckBytes(FileStream file, int offset, byte[] bytes)
        {
            byte[] buffer = new byte[bytes.Length];

            file.Seek(offset, SeekOrigin.Begin);
            file.Read(buffer, 0, bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
            {
                if (buffer[i] != bytes[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
