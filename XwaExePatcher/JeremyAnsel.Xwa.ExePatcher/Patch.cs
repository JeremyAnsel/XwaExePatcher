using System.Collections.Generic;

namespace JeremyAnsel.Xwa.ExePatcher
{
    public class Patch
    {
        public Patch()
        {
            this.Items = new List<PatchItem>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public IList<PatchItem> Items { get; private set; }
    }
}
