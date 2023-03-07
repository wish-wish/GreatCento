using System;

namespace Structures
{
	public class RBLeaf<T> : RBNode<T> where T : IComparable {
		public RBLeaf(RBBranch<T> parent) {
			this.parent = parent;
			red = false;
		}

		public RBLeaf() : this(null) {}

		public override void Insert (RBBranch<T> newNode)
		{
			if (parent != null) {
				if (parent.left == this)
					parent.left = newNode;
				else
					parent.right = newNode;
			}
			newNode.parent = parent;
		}

		public override void Remove (T t) {
			throw new ArgumentException (string.Format("Value {0} to remove not found", t));
		}

		public override RBBranch<T> GetNode(T t) {
			throw new ArgumentException (string.Format ("Value {0} not found in tree", t));
		}
	}
}

