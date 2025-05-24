using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using LanguageExt;
namespace ScrewThread
{
    using CommandResult = Either<string, object>;

    internal class InitCommandParametrs
    {
        public static (Chamfer, double, double, double, Result) Init()
        {
            var fail = (Chamfer.None, double.NaN, double.NaN, double.NaN, Result.Failure);

            var result = GetChamfer()
                .Bind(chamfer => GetDiameter()
                .Bind(diameter => GetPitch()
                .Bind(pitch => GetLength((double)pitch)
                .Map(length => (chamfer,diameter, pitch, length)))))
                .Map((val) => ((Chamfer)val.chamfer,(double)val.diameter, (double)val.pitch, (double)val.length, Result.Success))
                .IfLeft(
                    err =>
                    {
                        RhinoApp.WriteLine(err);
                        return fail;
                    }
                );
            return result;
        }

        private static CommandResult GetChamfer()
        {
            var chamfer = new GetOption();
            chamfer.SetCommandPrompt("Select chamfer type");
            chamfer.AddOptionEnumList("Chamfer", Chamfer.None, new Chamfer[] { Chamfer.None, Chamfer.Left, Chamfer.Right,  Chamfer.Both});
            var d = new OptionDouble(64);
            chamfer.SetDefaultString("None");
            var get = chamfer.Get();
            if(get == GetResult.Cancel)
            {
                return "Command cancelled.";
            }
            if (chamfer.GotDefault())
            {
                return Chamfer.None;
            }
            return chamfer.GetSelectedEnumValue<Chamfer>();
        }
        private static CommandResult GetDiameter()
        {
            var getDiameter = new GetNumber();
            getDiameter.SetCommandPrompt("Enter the diameter of the screw");
            getDiameter.SetDefaultNumber(64);
            getDiameter.SetLowerLimit(0.01, true);
            if (getDiameter.Get() != GetResult.Number)
            {
                return "Invalid diameter.";
            }
            return getDiameter.Number();
        }

        private static CommandResult GetLength(double pitch)
        {
            var getLength = new GetNumber();
            getLength.SetCommandPrompt("Enter the length of the screw");
            getLength.SetDefaultNumber(100);
            getLength.SetLowerLimit(pitch, true);
            if (getLength.Get() != GetResult.Number)
            {
                return "Invalid length";
            }
            return getLength.Number();
        }

        private static CommandResult GetPitch()
        {
            var getPitch = new GetNumber();
            getPitch.SetCommandPrompt("Enter the pitch of the screw");
            getPitch.SetDefaultNumber(6);
            getPitch.SetLowerLimit(0.01, true);
            if (getPitch.Get() != GetResult.Number)
            {
                return "Invalid pitch.";
            }
            return getPitch.Number();
        }
    }
}
