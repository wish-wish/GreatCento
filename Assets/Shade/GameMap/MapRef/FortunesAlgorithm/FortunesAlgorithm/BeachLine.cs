using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Structures;

namespace FortunesAlgorithm
{
    internal class BeachLine
    {
        RBTree<BeachSection> beachSections;
        EventQueue eventQueue;

        public BeachLine(IEnumerable<BeachSection> initialBeachSections, IEnumerable<Point> otherPoints) {

            beachSections = new RBTree<BeachSection>();
            eventQueue = new EventQueue();

            foreach (BeachSection bs in initialBeachSections)
                beachSections.Add(bs);

            foreach (Point point in otherPoints)
                eventQueue.Add(new SiteEventPoint(point));
        }

        public bool HasMoreEvents()
        {
            return !eventQueue.IsEmpty();
        }

        public IEventPoint PopEvent()
        {
            return eventQueue.Pop();
        }

        public void SplitBeachSection(Point site)
        {
            BeachSection containingBeachSection = BeachSectionContainingPoint(site);
            BeachSection newBeachSectionLeft = new BeachSection(containingBeachSection.focus, containingBeachSection.leftBoundary, site);
            BeachSection newBeachSectionCentre = new BeachSection(site, containingBeachSection.focus, containingBeachSection.focus);
            BeachSection newBeachSectionRight = new BeachSection(containingBeachSection.focus, site, containingBeachSection.rightBoundary);
            beachSections.Remove(containingBeachSection);
            beachSections.Add(newBeachSectionLeft);
            beachSections.Add(newBeachSectionCentre);
            beachSections.Add(newBeachSectionRight);

            foreach (IntersectEventPoint iep in IntersectEventPoint.FromBeachSections(new List<BeachSection> { newBeachSectionLeft, newBeachSectionRight }))
                if (iep.Point().y<= site.y)
                    eventQueue.Add(iep);

            foreach (IntersectEventPoint iep in IntersectEventPoint.FromBeachSections(new List<BeachSection> { containingBeachSection }))
                if (iep.Point().y<= site.y)
                    eventQueue.Remove(iep);
        }

        public void ConsumeBeachSection(IntersectEventPoint intersectEventPoint)
        {
            BeachSection consumedBeachSection = intersectEventPoint.consumedBeachSection;
            BeachSection leftBeachSection = beachSections.Predecessor(consumedBeachSection);
            BeachSection rightBeachSection = beachSections.Successor(consumedBeachSection);
            BeachSection newLeftBeachSection = new BeachSection(leftBeachSection.focus, leftBeachSection.leftBoundary, rightBeachSection.focus);
            BeachSection newRightBeachSection = new BeachSection(rightBeachSection.focus, leftBeachSection.focus, rightBeachSection.rightBoundary);
            beachSections.Remove(consumedBeachSection);
            beachSections.Remove(leftBeachSection);
            beachSections.Remove(rightBeachSection);
            beachSections.Add(newLeftBeachSection);
            beachSections.Add(newRightBeachSection);

            foreach (IntersectEventPoint iep in IntersectEventPoint.FromBeachSections(new List<BeachSection> { leftBeachSection, rightBeachSection }))
                if (iep.Point().y<= intersectEventPoint.Point().y)
                    eventQueue.Remove(iep);

            foreach (IntersectEventPoint iep in IntersectEventPoint.FromBeachSections(new List<BeachSection> { newLeftBeachSection, newRightBeachSection }))
                if (iep.Point().y<= intersectEventPoint.Point().y)
                    if (!iep.Equals(intersectEventPoint))
                        eventQueue.Add(iep);
        }

        public BeachSection BeachSectionContainingPoint(Point point)
        {
            RBBranch<BeachSection> candidate = (RBBranch<BeachSection>)beachSections.root;
            while (true)
            {
                int compareResult = candidate.value.CompareTo(point);
                if (compareResult == 0)
                    return candidate.value;
                if (compareResult > 0)
                    candidate = (RBBranch<BeachSection>)candidate.left;
                if (compareResult < 0)
                    candidate = (RBBranch<BeachSection>)candidate.right;
            }
        }
    }
}
