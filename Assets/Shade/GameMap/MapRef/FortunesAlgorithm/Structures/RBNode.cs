using System;

namespace Structures
{
	public abstract class RBNode<T> where T : IComparable
	{
		public bool red;
		public RBBranch<T> parent;

		public abstract void Insert (RBBranch<T> newNode);
		public abstract void Remove (T t);
		public abstract RBBranch<T> GetNode(T t);

		protected RBBranch<T> Grandparent() {
			if (parent == null)
				return null;
			return parent.parent;
		}

		protected RBNode<T> Sibling() {
			if (parent == null)
				return null;
			if (parent.left == this)
				return parent.right;
			return parent.left;
		}

		protected RBNode<T> Uncle() {
			if (parent == null)
				return null;
			return parent.Sibling ();
		}

		public void DeleteRepair() {
			if (parent == null) {
				return;
			}
			
			if (Sibling ().red) {
				parent.red = true;
				Sibling ().red = false;
				if (this == parent.left)
					parent.RotateLeft ();
				else
					parent.RotateRight ();
			}

			// Sibling is a branch (not a leaf) because it has more black children than we do.
			RBBranch<T> sibling = (RBBranch<T>)Sibling();

			if ((!parent.red) &&
			    (!sibling.red) &&
			    (!sibling.left.red) &&
			    (!sibling.right.red)) {
				sibling.red = true;
				parent.DeleteRepair ();
				return;
			}

			if ((parent.red) &&
			    (!sibling.red) &&
			    (!sibling.left.red) &&
			    (!sibling.right.red)) {
				sibling.red = true;
				parent.red = false;
				return;
			}

			if ((this == parent.left) &&
			    (!sibling.right.red)) {
				sibling.left.red = false;
				sibling.red = true;
				sibling.RotateRight ();
			} else if ((this == parent.right) &&
				(!sibling.left.red)) {
				sibling.left.red = false;
				sibling.red = true;
				sibling.RotateLeft ();
			}

			// Sibling is still a branch either from the earlier reasoning, or because our 
			// earlier sibling was rotated and a rotation must put a branch at the root.
			sibling = (RBBranch<T>)Sibling();

			sibling.red = parent.red;
			parent.red = false;

			if (this == parent.left) {
				sibling.right.red = false;
				parent.RotateLeft ();
			} else {
				sibling.left.red = false;
				parent.RotateRight ();
			}
		}
	}
}

