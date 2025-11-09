using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;

namespace ScrewThread
{
    public class ScrewThreadCommand : Command
    {
        public ScrewThreadCommand()
        {
            Instance = this;
        }

        public static ScrewThreadCommand Instance { get; private set; }

        public override string EnglishName => "Screw";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var options = new Options();
            var gp = new GetString();

            return ScrewCommandRunner.RunOptionsCommand(doc, options, gp);
        }
    }
}
