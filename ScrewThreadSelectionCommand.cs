using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using System.Collections.Generic;
using RhinoWindows;

namespace ScrewThread
{
    public class ScrewThreadSelectionCommand: Command
    {
        public ScrewThreadSelectionCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
            Panels.RegisterPanel(PlugIn,typeof(Views.MainPanel), LOC.STR("Screw threads"), null);

        }

        ///<summary>The only instance of this command.</summary>
        public static ScrewThreadSelectionCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "ScrewOptions";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //Views.SampleCsWpfDialog dialog = new Views.SampleCsWpfDialog();
            //dialog.ShowSemiModal(RhinoApp.MainWindowHandle());
            Panels.OpenPanel(typeof(Views.MainPanel).GUID);
            return Result.Success;
        }
   }
}
