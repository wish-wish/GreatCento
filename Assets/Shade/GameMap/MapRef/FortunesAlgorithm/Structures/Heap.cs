using System;
using System.Collections.Generic;

namespace Structures
{
	public class Heap<T>
	{
		int length;
		List<T> list;
		Func<T,T,bool> compare;

		public Heap(Func<T,T,bool> compare)
		{
			length = 0;
			list = new List<T>();
			this.compare = compare;
		}

		public void Add(T element) {
			int index = length;
			list.Add (element);
			length += 1;
			BubbleUp (index);
		}

		public bool IsEmpty() {
			return (length == 0);
		}

		public T Pop() {
			if (IsEmpty())
				throw new InvalidOperationException("Can't pop an empty heap");
			T result = First();
			SetFirst (Last ());
			list.RemoveAt (--length);
			BubbleDown ();
			return result;
		}

		public T Peek() {
			if (IsEmpty())
				throw new InvalidOperationException("Can't peek an empty heap");
			return First ();
		}

		void BubbleUp(int index) {
			if (!Root (index) && compare (list [index], list [Parent (index)])) {
				Swap (index, Parent (index));
				BubbleUp (Parent (index));
			}
		}

		void BubbleDown(int index = 0) {
			int preferredIndex;
			if (!LeftChildExists (index))
				return;
			if (RightChildExists (index) && compare (RightChild (index), LeftChild (index)))
				preferredIndex = RightChildIndex (index);
			else
				preferredIndex = LeftChildIndex (index);
			if (compare (list[preferredIndex], list[index])) {
				Swap (index, preferredIndex);
				BubbleDown (preferredIndex);
			}
		}

		void Swap(int aIndex, int bIndex) {
			T a = list [aIndex];
			list [aIndex] = list [bIndex];
			list [bIndex] = a;
		}

		bool Root(int index) {
			return index == 0;
		}

		int Parent(int index) {
			index -= 1;
			return index / 2;
		}

		int LeftChildIndex(int index) {
			return index * 2 + 1;
		}

		int RightChildIndex(int index) {
			return index * 2 + 2;
		}

		T LeftChild(int index) {
			return list [LeftChildIndex (index)];
		}

		T RightChild(int index) {
			return list [RightChildIndex (index)];
		}

		bool LeftChildExists(int index) {
			return IndexExists (LeftChildIndex (index)); 
		}

		bool RightChildExists(int index) {
			return IndexExists (RightChildIndex (index)); 
		}

		bool IndexExists(int index) {
			return index < length;
		}

		T Last() {
			return list[length-1];
		}

		T First() {
			return list [0];
		}

		void SetFirst(T element) {
			list [0] = element;
		}
	}
}