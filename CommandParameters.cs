using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using LanguageExt;
namespace ScrewThread
{
    using CommandResult = Either<string, double>;

    internal class InitCommandParametrs
    {
        public static (double, double, double, Result) Init()
        {
            var fail = (double.NaN, double.NaN, double.NaN, Result.Failure);

            var result = GetDiameter()
                .Bind(diameter => GetPitch()
                .Bind(pitch => GetLength(pitch)
                .Map(length => (diameter, pitch, length))))
                .Map((val) => (val.diameter, val.pitch, val.length, Result.Success))
                .IfLeft(
                    err =>
                    {
                        RhinoApp.WriteLine(err);
                        return fail;
                    }
                );
            return result;
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
