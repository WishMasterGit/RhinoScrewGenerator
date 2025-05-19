using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System.Collections.Generic;

namespace ScrewThread
{
    internal class Tools
    {
        internal static List<Brep> Explode(Brep brep)
        {
            BrepFaceList faces = brep.Faces;
            List<Brep> explodedSurfaces = new List<Brep>();
            foreach (BrepFace face in faces)
            {
                Brep singleSurface = face.DuplicateFace(false);
                if (singleSurface != null)
                    explodedSurfaces.Add(singleSurface);
            }
            return explodedSurfaces;
        }
        internal static Brep[] Merge(List<Brep> brep, double tollerance)
        {
            return Brep.JoinBreps(brep, tollerance);
        }

        internal static Brep[] ExplodeAndMerge(Brep brep, double tollerance)
        {
            var explodedBreps = Explode(brep);
            var mergedBrep = Merge(explodedBreps, tollerance);
            return mergedBrep;
        }
    }
}
