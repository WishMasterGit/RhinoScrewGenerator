using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace ScrewThread
{
    public class ScrewThreadAtCommand : Command
    {
        public ScrewThreadAtCommand()
        {
            Instance = this;
        }

        public static ScrewThreadAtCommand Instance { get; private set; }

        public override string EnglishName => "ScrewAt";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var options = new Options();
            var gp = new GetPoint();
            gp.SetCommandPrompt("Pick insertion point or press Enter for origin");
            gp.AcceptNothing(true);
            return ScrewCommandRunner.RunOptionsCommand(doc, options, gp);
        }
    }
}
