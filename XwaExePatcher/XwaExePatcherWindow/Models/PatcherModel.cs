using System.Collections.Generic;
using JeremyAnsel.Xwa.ExePatcher;

namespace XwaExePatcherWindow.Models
{
    public class PatcherModel
    {
        public PatcherModel(string fileName, string exeFileName)
        {
            this.FileName = fileName;
            this.ExeFileName = exeFileName;

            this.Patches = new List<PatchModel>();

            var patcher = Patcher.Read(fileName);

            foreach (var patch in patcher.Patches)
            {
                var patchModel = new PatchModel();

                patchModel.Name = patch.Name;
                patchModel.Description = patch.Description;

                foreach (var item in patch.Items)
                {
                    patchModel.Items.Add(new PatchItemModel
                    {
                        Offset = item.Offset,
                        OldValues = item.OldValues,
                        NewValues = item.NewValues
                    });
                }

                patchModel.IsApplied = Patcher.IsApplied(exeFileName, patch);
                patchModel.CanBeApplied = Patcher.CanBeApplied(exeFileName, patch);

                this.Patches.Add(patchModel);
            }
        }

        public string FileName { get; private set; }

        public string ExeFileName { get; private set; }

        public IList<PatchModel> Patches { get; private set; }

        public void Apply(PatchModel patch)
        {
            var patcher = new Patcher();
            patcher.Patches.Add(patch.ToPatch());

            patcher.Apply(this.ExeFileName, true);
        }

        public void Restore(PatchModel patch)
        {
            var patcher = new Patcher();
            patcher.Patches.Add(patch.ToPatch());

            patcher.Apply(this.ExeFileName, false);
        }
    }
}
