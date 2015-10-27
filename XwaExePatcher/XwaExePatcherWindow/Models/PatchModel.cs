using System.Collections.Generic;
using JeremyAnsel.Xwa.ExePatcher;

namespace XwaExePatcherWindow.Models
{
    public class PatchModel
    {
        public PatchModel()
        {
            this.Items = new List<PatchItemModel>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public IList<PatchItemModel> Items { get; private set; }

        public bool IsApplied { get; set; }

        public bool CanBeApplied { get; set; }

        public Patch ToPatch()
        {
            var patch = new Patch();

            patch.Name = this.Name;
            patch.Description = this.Description;

            foreach (var item in this.Items)
            {
                patch.Items.Add(new PatchItem
                {
                    Offset = item.Offset,
                    OldValues = item.OldValues,
                    NewValues = item.NewValues
                });
            }

            return patch;
        }
    }
}
