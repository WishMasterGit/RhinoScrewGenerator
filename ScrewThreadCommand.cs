using System;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Rhino.Input;
using Rhino.Input.Custom;

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

            (double diameter, double pitch, double length, Result result) = InitCommandParametrs.Init();

            if(result == Result.Failure)
            {
                RhinoApp.WriteLine("incorrect parameters");
                return result;
            }

            var profileSettings = new ProfileSettings(pitch, diameter, length, doc.ModelAbsoluteTolerance);
            var tolerance = doc.ModelAbsoluteTolerance;

            var profile = new Profile();
            var screwSurface = profile.CreateMaleSurface(profileSettings);
            var cuttingPlanes = profile.CuttingPlanes(profileSettings);

            var surfaces = new List<Brep>();
            surfaces.AddRange(screwSurface);
            surfaces.AddRange(cuttingPlanes);
            var breps = Brep.CreateSolid(surfaces, tolerance);

            doc.Objects.AddBrep(breps[0]);
            doc.Views.Redraw();
            return Result.Success;
        }

    }
}
