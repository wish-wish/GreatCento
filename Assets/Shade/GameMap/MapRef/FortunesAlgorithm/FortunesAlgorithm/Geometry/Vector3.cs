using System;

namespace FortunesAlgorithm
{
	public class Vector3
	{
		public float x;
		public float y;
		public float z;

		public Vector3 (float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3 CrossProduct(Vector3 that) {
			return new Vector3 (this.y * that.z - this.z * that.y, this.z * that.x - this.x * that.z, this.x * that.y - this.y * that.x);
		}

		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			Vector3 that = (Vector3)obj;
			return x == that.x && y == that.y && z == that.z;
		}

		public bool Colinear(Vector3 that) {
			Vector3 crossProduct = this.CrossProduct (that);
			Vector3 target = new Vector3 (0, 0, 0);
			return crossProduct.Equals (target);
		}

		public override int GetHashCode ()
		{
			return Hash.Triple (x, y, z);
		}

		public override string ToString ()
		{
			return string.Format ("V({0:F}, {1:F}, {2:F})", x, y, z);
		}
	}
}

