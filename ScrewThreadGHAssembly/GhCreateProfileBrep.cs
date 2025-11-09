using System;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ScrewThread
{
    public class GhCreateProfileBrep : GH_Component
    {
        public GhCreateProfileBrep()
          : base("CreateProfileBrep", "ProfileBrep",
              "Creates a screw thread Brep using ProfileSettings",
              "ScrewThread", "Geometry")
        { }

        public override Guid ComponentGuid => new Guid("A1B2C3D4-E5F6-47A8-9B0C-123456789ABC");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Pitch", "P", "Thread pitch", GH_ParamAccess.item, 6.0);
            pManager.AddNumberParameter("Diameter", "D", "Thread diameter", GH_ParamAccess.item, 64.0);
            pManager.AddNumberParameter("Length", "L", "Thread length", GH_ParamAccess.item, 200.0);
            pManager.AddNumberParameter("Tolerance", "T", "Modeling tolerance", GH_ParamAccess.item, 0.01);
            //pManager.AddIntegerParameter("Chamfer", "C", "Chamfer option (0=None, 1=Left, 2=Right, 3=Both)", GH_ParamAccess.item, 0);
            var chamferParam = pManager.AddIntegerParameter("Chamfer", "C", "Chamfer option", GH_ParamAccess.item, 0);
            var chamferGHParam = pManager[4] as Grasshopper.Kernel.Parameters.Param_Integer;
            if (chamferGHParam != null)
            {
                chamferGHParam.AddNamedValue("None", 0);
                chamferGHParam.AddNamedValue("Left", 1);
                chamferGHParam.AddNamedValue("Right", 2);
                chamferGHParam.AddNamedValue("Both", 3);
            }
            var profileParam = pManager.AddIntegerParameter("ProfileType", "PT", "Profile type", GH_ParamAccess.item, 0);
            var profileGHParam = pManager[5] as Grasshopper.Kernel.Parameters.Param_Integer;
            if (profileGHParam != null)
            {
                profileGHParam.AddNamedValue("Female", 0);
                profileGHParam.AddNamedValue("Male", 1);
            }
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("ProfileBrep", "B", "Resulting screw thread Brep", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double pitch = 0, diameter = 0, length = 0, tolerance = 0;
            int chamferInt = 0, profileTypeInt = 0;

            if (!DA.GetData(0, ref pitch)) return;
            if (!DA.GetData(1, ref diameter)) return;
            if (!DA.GetData(2, ref length)) return;
            if (!DA.GetData(3, ref tolerance)) return;
            if (!DA.GetData(4, ref chamferInt)) return;
            if (!DA.GetData(5, ref profileTypeInt)) return;

            var chamfer = (Chamfer)chamferInt;
            var profileType = (ProfileType)profileTypeInt;

            var settings = new ProfileSettings(pitch, diameter, length, tolerance, chamfer, profileType);
            var profile = new Profile();
            Brep brep = null;
            try
            {
                brep = profile.CreateProfileBrep(settings);
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.Message);
                return;
            }

            DA.SetData(0, brep);
        }
        protected override System.Drawing.Bitmap Icon => ScrewThreadGHAssembly.Properties .Resources.logo;
    }
}