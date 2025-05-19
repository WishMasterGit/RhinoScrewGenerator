using LanguageExt;
using LanguageExt.Pretty;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace ScrewThread
{
    internal readonly struct ProfileSettings
    {
        public ProfileSettings(double pitch, double diameter, double length, double tolerance)
        {
            Pitch = pitch;
            Diameter = diameter;
            Length = length;
            Tolerance = tolerance;
        }

        public double Pitch { get; }
        public double Diameter { get; }
        public double Length { get; }
        public double Tolerance { get; }
        public double ThreadAngle => 30;
        public double TurnCount => Length / Pitch;
        public double Radius => Diameter / 2;
        public double Height => Pitch / (Math.Tan(ToRadians(ThreadAngle)) * 2);
        private double F1 => Diameter / 2 - Height * 5 / 8;
        private double F2 => F1 - Pitch * Pitch / (16 * Height);
        private double F3 => Diameter / 2 + Pitch * Pitch / (32 * Height);

        public Point3d P1 => new Point3d(0, 0, F1);
        public Point3d P1e => new Point3d(Pitch / 8, 0, F2);
        public Vector3d P1h => new Vector3d(Pitch / 8, 0, -Height / 4);
        public Point3d P2 => new Point3d(Pitch / 4, 0, F2);
        public Point3d P3 => new Point3d(9 * Pitch / 16, 0, Diameter / 2);
        public Point3d P3e => new Point3d(5 * Pitch / 8, 0, F3);
        public Point3d P3h => new Point3d(5 * Pitch / 8, 0, Height / 8);
        public Point3d P4 => new Point3d(11 * Pitch / 16, 0, Diameter / 2);
        public Point3d P5 => new Point3d(Pitch, 0, F1);

        private double ToRadians(double angle)
        {
            return (Math.PI * angle / 180);
        }

    }
    /// <summary>
    /// This class represents a screw thread profile.
    /// https://en.wikipedia.org/wiki/ISO_metric_screw_thread
    /// </summary>
    internal class Profile
    {

        public ArcCurve ArcMale(ProfileSettings data)
        {
            return this.ArcMale(Vector3d.Zero, data);
        }
        public ArcCurve ArcMale(Vector3d translate, ProfileSettings data)
        {
            var arc = new ArcCurve(new Arc(data.P1, data.P1h, data.P2));
            arc.Translate(translate);
            return arc;
        }
        public PolylineCurve PolylineMale(ProfileSettings data)
        {
            return this.PolylineMale(Vector3d.Zero, data);
        }
        public PolylineCurve PolylineMale(Vector3d translate, ProfileSettings data)
        {
            var polyline = new Polyline(new List<Point3d> { data.P2, data.P3, data.P4, data.P5 }).ToPolylineCurve();
            polyline.Translate(translate);
            return polyline;
        }

        public Curve HelixCurve(ProfileSettings data)
        {
            Point3d axisStart = new Point3d(0, 0, 0);
            Vector3d axisDir = Vector3d.XAxis;
            Point3d radiusPoint = new Point3d(0, 0, data.Radius);
            var spiral = NurbsCurve.CreateSpiral(axisStart, axisDir, radiusPoint, data.Pitch, data.TurnCount, data.Radius, data.Radius);
            return spiral.DuplicateCurve();
        }
        public Curve CreateProfileCurve(ProfileSettings data)
        {
            var maleProfile = new PolyCurve();
            var stepVector = new Vector3d(0, 0, 0);
            var arcMale = ArcMale(stepVector, data);
            var pLineMale = PolylineMale(stepVector, data);
            maleProfile.Append(arcMale);
            maleProfile.Append(pLineMale);
            return maleProfile;

        }

        public Brep[] CreateMaleSurface(ProfileSettings data)
        {
            var maleProfile = CreateProfileCurve(data);
            var helix = HelixCurve(data);
            Line revolvingAxis = new Line(new Point3d(0, 0, 0), Vector3d.XAxis, 1);
            var threadsSurface = NurbsSurface.CreateRailRevolvedSurface(maleProfile, helix, revolvingAxis, false);
            Brep brep = threadsSurface.ToBrep();
            brep.Faces.SplitKinkyFaces();
            return Tools.ExplodeAndMerge(brep, data.Tolerance);
        }
        public List<Brep> CuttingPlanes(ProfileSettings data)
        {
            var cuttingCircle = CuttingCircle(data.Pitch, data);
            var cuttingCircle2 = CuttingCircle(data.Height * (data.TurnCount - 1), data);
            return new List<Brep> { cuttingCircle, cuttingCircle2 };
        }

        private Brep CuttingCircle(double translation, ProfileSettings data)
        {
            return Brep.CreatePlanarBreps((new Circle(new Plane(new Point3d(0, 0, 0), -Vector3d.XAxis), new Point3d(translation, 0, 0), data.Diameter)).ToNurbsCurve(), data.Tolerance)[0];
        }

        public double DiameterMinor(double diameterMajor, ProfileSettings data)
        {
            var diameterMinor = diameterMajor - 2 * (5 / 8) * data.Height;
            return diameterMinor;
        }

        public double DiameterPitch(double diameterMajor,ProfileSettings data)
        {
            var diameterPitch = diameterMajor - 2 * (3 / 8) * data.Height;
            return diameterPitch;
        }
    }
}
