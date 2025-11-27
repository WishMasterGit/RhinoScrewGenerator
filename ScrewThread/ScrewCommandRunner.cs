using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace ScrewThread
{
    internal static class ScrewCommandRunner
    {
        public static Result RunOptionsCommand<TGet>(RhinoDoc doc, Options options, TGet gp)
            where TGet : GetBaseClass
        {
            Chamfer chamfer = options.ChamferOption;
            ProfileType profileType = options.ProfileType;

            int opIndex = gp.AddOptionEnumList("Chamfer", chamfer);
            int pTypeOptionIndex = gp.AddOptionEnumList("ScrewType", profileType);

            gp.SetDefaultString("Screw");
            gp.SetCommandPrompt("Screw parameters");
            gp.AddOptionDouble("Pitch", ref options.Pitch);
            gp.AddOptionDouble("Diameter", ref options.Diameter);
            gp.AddOptionDouble("Length", ref options.Length);
            gp.AddOptionToggle("Cutter", ref options.Cutter);

            Point3d basePoint = Point3d.Origin;

            while (true)
            {
                GetResult get_rc = GetResult.Cancel;
                if (IsGetPoint<TGet>())
                    get_rc = (gp as GetPoint).Get();
                else if (IsGetString<TGet>())
                    get_rc = (gp as GetString).Get();

                if (gp.CommandResult() != Result.Success)
                    return gp.CommandResult();

                // Handle option changes
                if (get_rc == GetResult.Option)
                {
                    if (gp.OptionIndex() == opIndex)
                        chamfer = gp.GetSelectedEnumValue<Chamfer>();
                    if (gp.OptionIndex() == pTypeOptionIndex)
                        profileType = gp.GetSelectedEnumValue<ProfileType>();
                    options.UpdateEnums(chamfer, profileType);
                    continue;
                }

                // Handle completion
                bool isComplete =
                    (IsGetPoint<TGet>() && get_rc == GetResult.Point) ||
                    (IsGetString<TGet>() && get_rc == GetResult.String) ||
                    get_rc == GetResult.Nothing;

                if (isComplete)
                {
                    if (IsGetPoint<TGet>() && get_rc == GetResult.Point)
                        basePoint = (gp as GetPoint).Point();

                    options.Save();
                    CreateProfile(doc, options, basePoint);
                    break;
                }

                // Any other result: exit
                break;
            }
            return Result.Success;
        }
        private static bool IsGetString<TGet>() where TGet : GetBaseClass
            => typeof(TGet) == typeof(GetString);
        private static bool IsGetPoint<TGet>() where TGet : GetBaseClass
            => typeof(TGet) == typeof(GetPoint);

        public static void CreateProfile(RhinoDoc doc, Options options, Point3d origin)
        {
            var tolerance = doc.ModelAbsoluteTolerance;
            var profileSettings = new ProfileSettings(options.Pitch.CurrentValue, options.Diameter.CurrentValue, options.Length.CurrentValue, tolerance, options.ChamferOption, options.ProfileType, options.Cutter.CurrentValue);
            var profile = new Profile();
            var screwSurface = profile.CreateProfileBrep(profileSettings);
                var xform = Transform.Translation(origin.X, origin.Y, origin.Z);
                screwSurface.Transform(xform);
            doc.Objects.AddBrep(screwSurface);
            doc.Views.Redraw();
        }
    }
}