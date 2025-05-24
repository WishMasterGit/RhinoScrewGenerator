using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using System.Collections.Generic;

namespace ScrewThread
{
    public class ScrewThreadCommand : Command
    {
        public ScrewThreadCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static ScrewThreadCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "Screw";


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            GetString gp = new GetString();
            gp.SetDefaultString("Screw");
            gp.SetCommandPrompt("Screw parameters");
            var pitch = new OptionDouble(6);
            var diameter = new OptionDouble(60);
            var length = new OptionDouble(100);
            var chamfer = Chamfer.None;
            var profileType = ProfileType.Female;
            var opIndex = gp.AddOptionEnumList("Chamfer", Chamfer.None);
            var pTypeOptionIndex = gp.AddOptionEnumList("ScrewType", ProfileType.Female);
            gp.AddOptionDouble("Pitch", ref pitch);
            gp.AddOptionDouble("Diameter", ref diameter);
            gp.AddOptionDouble("Length", ref length);

            while (true)
            {
                GetResult get_rc = gp.Get();
                if (gp.CommandResult() != Result.Success)
                    return gp.CommandResult();

                if (get_rc == GetResult.String)
                {
                    CreateProfile(doc, pitch, diameter, length, chamfer, profileType);
                }
                else if (get_rc == GetResult.Option)
                {
                    if(gp.OptionIndex() == opIndex) chamfer = gp.GetSelectedEnumValue<Chamfer>();
                    if(gp.OptionIndex() == pTypeOptionIndex) profileType = gp.GetSelectedEnumValue<ProfileType>();
                    continue;
                }
                break;
            }

            return Result.Success;
        }

        private static void CreateProfile(RhinoDoc doc, OptionDouble pitch, OptionDouble diameter, OptionDouble length, Chamfer chamfer, ProfileType profileType)
        {
            var tolerance = doc.ModelAbsoluteTolerance;
            var profileSettings = new ProfileSettings(pitch.CurrentValue, diameter.CurrentValue, length.CurrentValue, tolerance, chamfer, profileType);
            var profile = new Profile();
            var screwSurface = profile.CreateProfileBrep(profileSettings);
            doc.Objects.AddBrep(screwSurface);
            doc.Views.Redraw();
        }
    }
}
