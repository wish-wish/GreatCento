using System;

namespace Structures
{
	public class RBBranch<T> : RBNode<T> where T : IComparable {
		public T value;
		public RBNode<T> left;
		public RBNode<T> right;

		public RBBranch(T value, RBBranch<T> parent) {
			this.parent = parent;
			this.value = value;
			red = true;
			left = new RBLeaf<T> (this);
			right = new RBLeaf<T> (this);
		}

		public RBBranch(T value) : this(value, null) {}

		public override void Insert (RBBranch<T> newNode)
		{
			T t = newNode.value;
			int compareResult = t.CompareTo (value);
			if (compareResult == 0)
				throw new ArgumentException (string.Format("Duplicate value {0} specified", t));
			else if (compareResult > 0)
				right.Insert (newNode);
			else
				left.Insert (newNode);
		}

		public override void Remove (T t) {
			GetNode (t).Delete ();
		}

		public override RBBranch<T> GetNode (T t) {
			int compareResult = t.CompareTo (value);
			if (compareResult == 0)
				return this;
			else if (compareResult > 0)
				return right.GetNode (t);
			else
				return left.GetNode (t);
		}

		public void RotateLeft() {
			if (!(right is RBBranch<T>))
				throw new InvalidOperationException ("Can't rotate left a branch with a right leaf");
			RBBranch<T> newLocalRoot = (RBBranch<T>)right;
			RBNode<T> middle = newLocalRoot.left;
			if (parent != null) {
				if (this == parent.left)
					parent.left = newLocalRoot;
				else
					parent.right = newLocalRoot;
			}
			newLocalRoot.parent = this.parent;
			this.parent = newLocalRoot;
			newLocalRoot.left = this;
			this.right = middle;
			middle.parent = this;
		}

		public void RotateRight() {
			if (!(left is RBBranch<T>))
				throw new InvalidOperationException ("Can't rotate right a branch with a left leaf");
			RBBranch<T> newLocalRoot = (RBBranch<T>)left;
			RBNode<T> middle = newLocalRoot.right;
			if (parent != null) {
				if (this == parent.left)
					parent.left = newLocalRoot;
				else
					parent.right = newLocalRoot;
			}
			newLocalRoot.parent = this.parent;
			this.parent = newLocalRoot;
			newLocalRoot.right = this;
			this.left = middle;
			middle.parent = this;
		}

		public void InsertRepair() {
			if (parent == null) {
				red = false;
			} else if (!parent.red) {
				return;
			} else if (Uncle ().red) {
				RepaintEldersAndPercolateInsertRepair ();
			} else {
				RotateParentIntoGrandparentPosition ();
			}
		}

		void RepaintEldersAndPercolateInsertRepair() {
			parent.red = false;
			Uncle ().red = false;
			Grandparent ().red = true;
			Grandparent ().InsertRepair ();
		}

		void RotateParentIntoGrandparentPosition () {
			RepairRotateToOutside ().RepairRotateParentToGrandparent ();
		}

		RBBranch<T> RepairRotateToOutside() {
			RBBranch<T> originalParent = parent;
			if (parent == Grandparent ().left && this == parent.right) {
				parent.RotateLeft ();
				return originalParent;
			} else if (parent == Grandparent().right && this == parent.left) {
				parent.RotateRight ();
				return originalParent;
			}
			return this;
		}

		void RepairRotateParentToGrandparent() {
			parent.red = false;
			Grandparent().red = true;
			if (this == parent.left)
				Grandparent().RotateRight ();
			else
				Grandparent().RotateLeft ();
		}

		RBBranch<T> MinBranch() {
			if (!(left is RBBranch<T>))
				return this;
			RBBranch<T> branch = (RBBranch<T>)left;
			return branch.MinBranch();
		}

		RBBranch<T> MaxBranch() {
			if (!(right is RBBranch<T>))
				return this;
			RBBranch<T> branch = (RBBranch<T>)right;
			return branch.MaxBranch();
		}

		public void Delete() {
			RBBranch<T> minBranch = this;
			if (right is RBBranch<T>) {
				RBBranch<T> rightBranch = (RBBranch<T>)right;
				minBranch = rightBranch.MinBranch ();
				value = minBranch.value;
			}
			minBranch.DeleteBranchWithNoMoreThanOneChildBranch ();
		}

		void ReplaceNode(RBNode<T> child) {
			child.parent = parent;
			if (parent != null) {
				if (this == parent.left)
					parent.left = child;
				else
					parent.right = child;
			}
		}

		void DeleteBranchWithNoMoreThanOneChildBranch() {
			RBNode<T> child;
			if (!(left is RBBranch<T>))
				child = right;
			else
				child = left;

			ReplaceNode (child);
			if (!red) {
				if (child.red)
					child.red = false;
				else
					child.DeleteRepair ();
			}
		}

		public T Successor() {
			if (right is RBBranch<T>) {
				RBBranch<T> branch = (RBBranch<T>)right;
				return branch.MinBranch().value;
			}
			return NextRightParent ().value;
		}

		public T Predecessor() {
			if (left is RBBranch<T>) {
				RBBranch<T> branch = (RBBranch<T>)left;
				return branch.MaxBranch().value;
			}
			return NextLeftParent ().value;
		}

		RBBranch<T> NextLeftParent() {
			if (parent == null)
				throw new ArgumentOutOfRangeException ("Asked for predecessor of element with least value");
			if (this == parent.right)
			    return parent;
			return parent.NextLeftParent ();
		}

		RBBranch<T> NextRightParent() {
			if (parent == null)
				throw new ArgumentOutOfRangeException ("Asked for successor of element with greatest value");
			if (this == parent.left)
				return parent;
			return parent.NextRightParent ();
		}
	}
}

