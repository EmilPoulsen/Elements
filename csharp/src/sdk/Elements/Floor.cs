using Hypar.Geometry;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Hypar.Elements
{
    /// <summary>
    /// A Floor is a horizontal element defined by an outer boundary and one or several holes.
    /// </summary>
    public class Floor : Element, ITessellate<Mesh>
    {
        /// <summary>
        /// The boundary of the floor.
        /// </summary>
        [JsonProperty("location")]
        public Polygon Perimeter{get;}

        /// <summary>
        /// The voids in the slab.
        /// </summary>
        [JsonProperty("voids")]
        public IEnumerable<Polygon> Voids{get;}

        /// <summary>
        /// The elevation from which the floor is extruded.
        /// </summary>
        [JsonProperty("elevation")]
        public double Elevation{get;}

        /// <summary>
        /// The thickness of the floor.
        /// </summary>
        [JsonProperty("thickness")]
        public double Thickness{get;}
        
        /// <summary>
        /// The area of the floor.
        /// Overlapping openings and openings which are outside of the floor's perimeter,
        /// will result in incorrect area results.
        /// </summary>
        [JsonProperty("area")]
        public double Area
        {
            get{return this.Perimeter.Area - this.Voids.Sum(o=>o.Area);}
        }

        /// <summary>
        /// Construct a floor.
        /// </summary>
        public Floor() : base()
        {
            this.Perimeter = Profiles.Rectangular();
            this.Elevation = 0.0;
            this.Thickness = 0.2;
            this.Material = BuiltInMaterials.Concrete;
            this.Voids = new List<Polygon>();
        }
        
        /// <summary>
        /// Construct a floor.
        /// </summary>
        /// <param name="perimeter">The perimeter of the floor.</param>
        /// <param name="elevation">The elevation of the floor.</param>
        /// <param name="thickness">The thickness of the floor.</param>
        /// <param name="voids">The voids in the floor.</param>
        /// <param name="material">The floor's material.</param>
        public Floor(Polygon perimeter, double elevation, double thickness, IList<Polygon> voids = null, Material material = null)
        {
            if (thickness <= 0.0)
            {
                throw new ArgumentOutOfRangeException("thickness", "The slab thickness must be greater than 0.0.");
            }

            this.Perimeter = perimeter;
            this.Voids = voids == null ? new List<Polygon>() : voids;
            this.Elevation = elevation;
            this.Thickness = thickness;
            this.Transform = new Transform(new Vector3(0, 0, elevation), new Vector3(1, 0, 0), new Vector3(0, 0, 1));
            this.Material = material == null ? BuiltInMaterials.Concrete : material;
        }

        /// <summary>
        /// Tessellate the slab.
        /// </summary>
        /// <returns></returns>
        public Mesh Tessellate()
        {
            var polys = new List<Polygon>();
            polys.Add(this.Perimeter);
            if (this.Voids != null)
            {
                polys.AddRange(this.Voids);
            }

            return Mesh.Extrude(polys, this.Thickness, true);
        }
    }
}