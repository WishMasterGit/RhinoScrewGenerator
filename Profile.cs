using LanguageExt;
using LanguageExt.Pretty;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace ScrewThread
{
    /// <summary>
    /// This class represents a screw thread profile.
    /// https://en.wikipedia.org/wiki/ISO_metric_screw_thread
    /// </summary>
    internal class Profile
    {
        public double Pitch { get; }
        public double Diameter { get; }
        public double Length { get; }
        public double TurnCount { get; }
        public double Tolerance { get; }
        public double Radius => Diameter / 2;
        public Point3d P1 { get; }
        public Point3d P1e { get; }
        public Vector3d P1h { get; }
        public Point3d P2 { get; }
        public Point3d P3 { get; }
        public Point3d P3e { get; }
        public Point3d P3h { get; }
        public Point3d P4 { get; }
        public Point3d P5 { get; }

        public Profile(double pitch = 6.0, double diameter = 64.0, double length = 200, double tolerance = 0.0001, double threadAngle = 30)
        {
            Pitch = pitch;
            Diameter = diameter;
            Length = length;
            TurnCount = length / pitch;
            Tolerance = tolerance;

            var height = Height(pitch, threadAngle);

            var f1 = diameter / 2 - height * 5 / 8;
            var f2 = f1 - pitch * pitch / (16 * height);
            var f3 = diameter / 2 + pitch * pitch / (32 * height);

            P1 = new Point3d(0, 0, f1);
            P1e = new Point3d(pitch / 8, 0, f2);
            P1h = new Vector3d(pitch / 8, 0, -height / 4);
            P2 = new Point3d(pitch / 4, 0, f1);
            P3 = new Point3d(9 * pitch / 16, 0, diameter / 2);
            P3e = new Point3d(5 * pitch / 8, 0, f3);
            P3h = new Point3d(5 * pitch / 8, 0, height / 8);
            P4 = new Point3d(11 * pitch / 16, 0, diameter / 2);
            P5 = new Point3d(pitch, 0, f1);
        }

        public ArcCurve ArcMale()
        {
            return this.ArcMale(Vector3d.Zero);
        }
        public ArcCurve ArcMale(Vector3d translate)
        {
            var arc = new ArcCurve(new Arc(P1, P1h, P2));
            arc.Translate(translate);
            return arc;
        }
        public PolylineCurve PolylineMale()
        {
            return this.PolylineMale(Vector3d.Zero);
        }
        public PolylineCurve PolylineMale(Vector3d translate)
        {
            var polyline = new Polyline(new List<Point3d> { P2, P3, P4, P5 }).ToPolylineCurve();
            polyline.Translate(translate);
            return polyline;
        }

        public Curve HelixCurve()
        {
            Point3d axisStart = new Point3d(0, 0, 0);
            Vector3d axisDir = Vector3d.XAxis;
            Point3d radiusPoint = new Point3d(0, 0, Radius);
            var spiral = NurbsCurve.CreateSpiral(axisStart, axisDir, radiusPoint, Pitch, TurnCount, Radius, Radius);
            return spiral.DuplicateCurve();
        }
        public Curve CreateProfileCurve()
        {
            var maleProfile = new PolyCurve();
            var stepVector = new Vector3d(0, 0, 0);
            var arcMale = ArcMale(stepVector);
            var pLineMale = PolylineMale(stepVector);
            maleProfile.Append(arcMale);
            maleProfile.Append(pLineMale);
            return maleProfile;

        }

        public Brep[] CreateMaleSurface()
        {
            var maleProfile = CreateProfileCurve();
            var helix = HelixCurve();
            Line revolvingAxis = new Line(new Point3d(0, 0, 0), Vector3d.XAxis, 1);
            var threadsSurface = NurbsSurface.CreateRailRevolvedSurface(maleProfile, helix, revolvingAxis, false);
            Brep brep = threadsSurface.ToBrep();
            brep.Faces.SplitKinkyFaces();
            return Tools.ExplodeAndMerge(brep, Tolerance);
        }
        public List<Brep> CuttingPlanes()
        {
            var cuttingCircle = CuttingCircle(Pitch);
            var cuttingCircle2 = CuttingCircle(Height(Pitch) * (TurnCount - 1));
            return new List<Brep> { cuttingCircle, cuttingCircle2 };
        }
        
        private Brep CuttingCircle(double translation)
        {
            return Brep.CreatePlanarBreps((new Circle(new Plane(new Point3d(0, 0, 0), -Vector3d.XAxis), new Point3d(translation, 0, 0),Diameter)).ToNurbsCurve(),Tolerance)[0];
        }
        public double Height(double pitch, double threadAngle = 30)
        {
            var height = pitch / (Math.Tan(ToRadians(threadAngle)) * 2);
            return height;
        }

        public double DiameterMinor(double diameterMajor, double pitch, double threadAngle = 30)
        {
            var height = Height(pitch, threadAngle);
            var diameterMinor = diameterMajor - 2 * (5 / 8) * height;
            return diameterMinor;
        }

        public double DiameterPitch(double diameterMajor, double pitch, double threadAngle = 30)
        {
            var height = Height(pitch, threadAngle);
            var diameterPitch = diameterMajor - 2 * (3 / 8) * height;
            return diameterPitch;
        }

        private double ToRadians(double angle)
        {
            return (Math.PI * angle / 180);
        }
    }
}
