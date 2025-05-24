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


            // For this example we will use a GetPoint class, but all of the custom
            // "Get" classes support command line options.
            GetString gp = new GetString();
            gp.SetDefaultString("Screw");
            gp.SetCommandPrompt("Screw parameters");
            // set up the options
            var pitch = new OptionDouble(6);
            var diameter = new OptionDouble(60);
            var length = new OptionDouble(100);
            var chamfer = Chamfer.None;
            var opIndex = gp.AddOptionEnumList("Chamfer", Chamfer.None, new Chamfer[] { Chamfer.None, Chamfer.Left, Chamfer.Right, Chamfer.Both });
            gp.AddOptionDouble("Pitch", ref pitch);
            gp.AddOptionDouble("Diameter", ref diameter);
            gp.AddOptionDouble("Length", ref length);

            while (true)
            {
                // perform the get operation. This will prompt the user to input a point, but also
                // allow for command line options defined above
                Rhino.Input.GetResult get_rc = gp.Get();
                if (gp.CommandResult() != Rhino.Commands.Result.Success)
                    return gp.CommandResult();

                if (get_rc == Rhino.Input.GetResult.String)
                {
                    var tolerance = doc.ModelAbsoluteTolerance;
                    var profileSettings = new ProfileSettings(pitch.CurrentValue, diameter.CurrentValue, length.CurrentValue, tolerance, chamfer);
                    var profile = new Profile();
                    var screwSurface = profile.CreateMaleSurface(profileSettings);
                    doc.Objects.AddBrep(screwSurface);
                    doc.Views.Redraw();
                }
                else if (get_rc == GetResult.Option)
                {
                    if(gp.OptionIndex() == opIndex) chamfer = gp.GetSelectedEnumValue<Chamfer>();
                    continue;
                }
                break;
            }
            //return Rhino.Commands.Result.Success;
            //var units = Dialogs.ShowListBox("Screw Thread", "Select the type of screw thread", new[] { "Metric", "Imperial" });

            //(Chamfer chamfer, double diameter, double pitch, double length, Result result) = InitCommandParametrs.Init();

            //var screwType = Dialogs.ShowListBox("Screw Type", "Select type", new[] { "Male", "Female" });
            //var chamfer = (Chamfer)Dialogs.ShowListBox("Screw Type", "Chamfer", new[] { Chamfer.None, Chamfer.Left, Chamfer.Right, Chamfer.Both});

            //if (result == Result.Failure)
            //{
            //    RhinoApp.WriteLine("incorrect parameters");
            //    return result;
            //}

            return Result.Success;
        }

    }
}
