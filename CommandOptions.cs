using Rhino.Input.Custom;
using System;

namespace ScrewThread
{
    internal class Options
    {
        public OptionDouble Pitch;
        public OptionDouble Diameter; 
        public OptionDouble Length;
        public Chamfer ChamferOption;
        public ProfileType ProfileType;

        const string section = "ScrewThread";
        const string lastSave = "LastUsedOptions";

        public Options()
        {
            var plug = ScrewThreadPlugin.Instance;
            var lastOptions = plug?.CommandSettings(lastSave);

            double pitch = lastOptions?.GetDouble("Pitch", 6.0) ?? 6.0;
            double diameter = lastOptions?.GetDouble("Diameter", 60.0) ?? 60.0;
            double length = lastOptions?.GetDouble("Length", 100.0) ?? 100.0;
            Chamfer chamfer = Enum.TryParse(lastOptions?.GetString("Chamfer", Chamfer.None.ToString()), out Chamfer c) ? c : Chamfer.None;
            ProfileType profileType = Enum.TryParse(lastOptions?.GetString("ProfileType", ProfileType.Female.ToString()), out ProfileType p) ? p : ProfileType.Female;

            Pitch = new OptionDouble(pitch);
            Diameter = new OptionDouble(diameter);
            Length = new OptionDouble(length);
            ChamferOption = chamfer;
            ProfileType = profileType;
        }

        public void Save()
        {
            var plug = ScrewThreadPlugin.Instance;
            var lastOptions = plug?.CommandSettings(lastSave);
            if (lastOptions == null) return;
            lastOptions.SetDouble("Pitch", Pitch.CurrentValue);
            lastOptions.SetDouble("Diameter", Diameter.CurrentValue);
            lastOptions.SetDouble("Length", Length.CurrentValue);
            lastOptions.SetString("Chamfer", ChamferOption.ToString());
            lastOptions.SetString("ProfileType", ProfileType.ToString());
        }

        public void UpdateEnums(Chamfer chamfer, ProfileType profileType)
        {
            ChamferOption = chamfer;
            ProfileType = profileType;
        }
    }
}
