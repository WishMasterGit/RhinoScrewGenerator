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

        public Brep CreateMaleSurface(ProfileSettings data)
        {
            var maleProfile = CreateProfileCurve(data);
            var helix = HelixCurve(data);
            Line revolvingAxis = new Line(new Point3d(0, 0, 0), Vector3d.XAxis, 1);
            var threadsSurface = NurbsSurface.CreateRailRevolvedSurface(maleProfile, helix, revolvingAxis, false);
            Brep brep = threadsSurface.ToBrep();
            brep.Faces.SplitKinkyFaces();
            var screw = Tools.ExplodeAndMerge(brep, data.Tolerance);
            var cuttingPlanes = CuttingPlanes(data);
            var surfaces = new List<Brep>();
            surfaces.AddRange(screw);
            surfaces.AddRange(cuttingPlanes);
            var breps = Brep.CreateSolid(surfaces, data.Tolerance);
            var result = breps[0];
            result.Translate(Vector3d.XAxis * -data.Pitch);
            if(data.ChamferOption != Chamfer.None)
            {
                result = ChamferProfile(result, data);
            }
            return result;
        }
        private List<Brep> CuttingPlanes(ProfileSettings data)
        {
            var cuttingCircle = CuttingCircle(data.Pitch, data);
            var cuttingCircle2 = CuttingCircle(data.Height * (data.TurnCount - 1), data);
            return new List<Brep> { cuttingCircle, cuttingCircle2 };
        }

        private Brep CuttingCircle(double translation, ProfileSettings data)
        {
            return Brep.CreatePlanarBreps((new Circle(new Plane(new Point3d(0, 0, 0), -Vector3d.XAxis), new Point3d(translation, 0, 0), data.Diameter)).ToNurbsCurve(), data.Tolerance)[0];
        }

        public Brep ChamferProfile(Brep screwProfile, ProfileSettings data)
        {
            var rot = Vector3d.XAxis;
            rot.Transform(Transform.Rotation(data.ChamferAngle, Vector3d.ZAxis, Point3d.Origin));
            double length = data.Pitch * (data.TurnCount);
            var line = new Line(data.Pc1, rot, length);
            var line2 = new Line(data.Pc1, rot, length);
            line2.Transform(Transform.Mirror(Plane.WorldYZ));
            var polyline = new Polyline(new List<Point3d> { data.Pc1, line.PointAtLength(length), line2.PointAtLength(length), data.Pc1 });
            var chamferCutter = RevSurface.Create(polyline, new Line(Point3d.Origin, Vector3d.XAxis * length), 0, 2 * Math.PI).ToBrep();
            var startChamfer = Brep.CreateBooleanIntersection(chamferCutter, screwProfile, data.Tolerance);
            if(data.ChamferOption == Chamfer.Left) return startChamfer[0];
            chamferCutter.Translate(Vector3d.XAxis * (data.Height*(data.TurnCount-1)-data.Pitch));
            if(data.ChamferOption == Chamfer.Right) return Brep.CreateBooleanIntersection(screwProfile, chamferCutter, data.Tolerance)[0];
            var endChamfer = Brep.CreateBooleanIntersection(startChamfer[0], chamferCutter, data.Tolerance);
            return endChamfer[0];
        }

        public double DiameterMinor(double diameterMajor, ProfileSettings data)
        {
            var diameterMinor = diameterMajor - 2 * (5 / 8) * data.Height;
            return diameterMinor;
        }

        public double DiameterPitch(double diameterMajor, ProfileSettings data)
        {
            var diameterPitch = diameterMajor - 2 * (3 / 8) * data.Height;
            return diameterPitch;
        }
    }
}
