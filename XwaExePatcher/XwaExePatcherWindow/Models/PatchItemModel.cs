using System;
using System.Diagnostics.CodeAnalysis;

namespace XwaExePatcherWindow.Models
{
    public class PatchItemModel
    {
        public int Offset { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] OldValues { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] NewValues { get; set; }

        public string OldValuesString
        {
            get { return BitConverter.ToString(this.OldValues); }
        }

        public string NewValuesString
        {
            get { return BitConverter.ToString(this.NewValues); }
        }
    }
}
