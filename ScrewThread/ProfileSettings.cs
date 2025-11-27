using Rhino.Geometry;
using System;
using System.Runtime.CompilerServices;

namespace ScrewThread
{
    public enum Chamfer
    {
        None,
        Left,
        Right,
        Both
    }

    public enum ProfileType
    {
        Male,
        Female
    }
    /// <summary>
    /// Represents the geometric and thread parameters for a screw thread profile.
    /// Provides calculated properties for key points and dimensions based on ISO metric thread standards.
    /// </summary>
    public readonly struct ProfileSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSettings"/> struct.
        /// </summary>
        /// <param name="pitch">The thread pitch (distance between threads).</param>
        /// <param name="diameter">The major diameter of the thread.</param>
        /// <param name="length">The total length of the thread.</param>
        /// <param name="tolerance">The modeling tolerance.</param>
        public ProfileSettings(double pitch, double diameter, double length, double tolerance, Chamfer chamferOption, ProfileType profileType, bool cutter)
        {
            Pitch = pitch;
            Diameter = diameter;
            Length = length;
            Tolerance = tolerance;
            ChamferOption = chamferOption;
            ProfileType = profileType;
            Cutter = cutter;
        }

        /// <summary>
        /// Gets the type of the profile.
        /// </summary>
        public ProfileType ProfileType { get; }

        /// <summary>
        /// Gets a value indicating whether the cutter functionality is enabled.
        /// </summary>
        public bool Cutter { get; }

        /// <summary>
        /// Gets the thread pitch (distance between threads).
        /// </summary>
        public double Pitch { get; }

        /// <summary>
        /// Gets the major diameter of the thread.
        /// </summary>
        public double Diameter { get; }

        /// <summary>
        /// Gets the total length of the thread.
        /// </summary>
        public double Length { get; }

        /// <summary>
        /// Gets the modeling tolerance.
        /// </summary>
        public double Tolerance { get; }
        public Chamfer ChamferOption { get; }

        /// <summary>
        /// Gets the thread angle in degrees (typically 30 for ISO metric threads).
        /// </summary>
        public double ThreadAngle => 30;

        /// <summary>
        /// Gets the chamfer angle in degrees (typically 45 for ISO metric threads).
        /// </summary>
        public double ChamferAngle => ToRadians(45);

        /// <summary>
        /// Gets the number of thread turns.
        /// </summary>
        public double TurnCount => Length / Pitch + 4;

        /// <summary>
        /// Gets the thread radius (half of the diameter).
        /// </summary>
        public double Radius => Diameter / 2;

        /// <summary>
        /// Gets the thread height, calculated from pitch and thread angle.
        /// </summary>
        public double Height => Pitch / (Math.Tan(ToRadians(ThreadAngle)) * 2);

        /// <summary>
        /// Gets the first key Z coordinate for the thread profile.
        /// </summary>
        private double F1 => Diameter / 2 - Height * 5 / 8;

        /// <summary>
        /// Gets the second key Z coordinate for the thread profile.
        /// </summary>
        private double F2 => F1 - Pitch * Pitch / (16 * Height);

        /// <summary>
        /// Gets the third key Z coordinate for the thread profile.
        /// </summary>
        private double F3 => Diameter / 2 + Pitch * Pitch / (32 * Height);


        private double F4 => F1 - Height/8;

        /// <summary>
        /// Gets the first profile point (start of thread root).
        /// </summary>
        public Point3d P1 => new Point3d(0, 0, F1);


        /// <summary>
        /// Gets the end point of the first profile segment.
        /// </summary>
        public Point3d P1e => new Point3d(Pitch / 8, 0, F2);

        /// <summary>
        /// Gets the vector for the first profile arc.
        /// </summary>
        public Vector3d P1h => new Vector3d(Pitch / 8, 0, -Height / 4);

        /// <summary>
        /// Gets the second profile point.
        /// </summary>
        public Point3d P2 => new Point3d(Pitch / 4, 0, F1);

        /// <summary>
        /// Gets the third profile point (crest of thread).
        /// </summary>
        public Point3d P3 => new Point3d(9 * Pitch / 16, 0, Diameter / 2);

        /// <summary>
        /// Gets the end point of the third profile segment.
        /// </summary>
        public Point3d P3e => new Point3d(5 * Pitch / 8, 0, F3);

        /// <summary>
        /// Gets the helper point for the third profile arc.
        /// </summary>
        public Point3d P3h => new Point3d(5 * Pitch / 8, 0, Height / 8);

        /// <summary>
        /// Gets the fourth profile point.
        /// </summary>
        public Point3d P4 => new Point3d(11 * Pitch / 16, 0, Diameter / 2);

        /// <summary>
        /// Gets the fifth profile point (end of thread root).
        /// </summary>
        public Point3d P5 => new Point3d(Pitch, 0, F1);

        public Point3d Pc1 => new Point3d(0, F4, 0);

        /// <summary>
        /// Converts an angle in degrees to radians.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        private double ToRadians(double angle)
        {
            return (Math.PI * angle / 180);
        }
    }
}
