using System;
using Structures;
using System.Collections.Generic;

namespace FortunesAlgorithm
{
    internal class EventQueue
	{

		Heap<IEventPoint> eventQueue;
		HashSet<IEventPoint> intersectionsToIgnore;

		public EventQueue ()
		{
			eventQueue = new Heap<IEventPoint> ((a, b) => a.Point().y> b.Point().y);
			intersectionsToIgnore = new HashSet<IEventPoint> ();
		}

		public bool IsEmpty() {
			if (eventQueue.IsEmpty ())
				return true;
			IEventPoint next = eventQueue.Peek ();
			if (intersectionsToIgnore.Contains (next)) {
                eventQueue.Pop();
                return IsEmpty();
            }
            return false;
		}

		public void Add(IEventPoint eventPoint) {
			eventQueue.Add (eventPoint);
		}

		public void Remove(IEventPoint eventPoint) {
			intersectionsToIgnore.Add (eventPoint);
		}

		public IEventPoint Pop() {
			IEventPoint next = eventQueue.Pop ();
			if (intersectionsToIgnore.Contains (next)) {
				return Pop ();
			}
			return next;
		}

	}
}

