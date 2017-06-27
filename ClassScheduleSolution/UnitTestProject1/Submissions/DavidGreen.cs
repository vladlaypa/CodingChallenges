using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule
{
    public partial class BasicTest
    {
        public bool CanGraduate_DavidGreen(int numAllCourses, List<Tuple<int, int>> prerequisites)
        {
            var planner = new SchedulePlanner(numAllCourses, prerequisites);

            return planner.CanGraduate();
        }
    }

    public class SchedulePlanner
    {
        private int _coursesNeeded;
        private HashSet<int> _coursesTotal;
        private HashSet<int> _validCourses;
        private HashSet<int> _invalidCourses;
        private List<Tuple<int, int>> _prerequisites;
        private List<Tuple<int, int>> _unprocessedCourses;
        private Stack<int> _blockedCourses;

        public SchedulePlanner(int numAllCourses, List<Tuple<int, int>> prerequisites)
        {
            _coursesNeeded = numAllCourses;
            _prerequisites = prerequisites;
            _unprocessedCourses = prerequisites.ToList();

            _validCourses = new HashSet<int>();
            _invalidCourses = new HashSet<int>();
            _coursesTotal = new HashSet<int>();
            _blockedCourses = new Stack<int>();

            _prerequisites.ForEach(x =>
            {
                _coursesTotal.Add(x.Item1);
                _coursesTotal.Add(x.Item2);
            });
        }

        public bool CanGraduate()
        {
            TakeClasses();

            return HasEnoughCouses();
        }

        private bool HasEnoughCouses()
        {
            if (_validCourses.Count < _coursesNeeded)
                return false;
            else
                return true;
        }

        private bool EnoughCouresLeft()
        {
            var remainingCourses = _coursesTotal.Count - _invalidCourses.Count;
            if (remainingCourses < _coursesNeeded)
                return false;
            else
                return true;
        }


        public void TakeClasses()
        {
            var currentCourses = _prerequisites;

            foreach (var current in currentCourses)
            {
                var preReq = current.Item1;
                var possibleCourse = current.Item2;

                if (_unprocessedCourses.Contains((current)))
                {
                    _blockedCourses.Push(possibleCourse);

                    if (CanTake(preReq))
                        _validCourses.Add(possibleCourse);

                    _blockedCourses.Pop();

                    _unprocessedCourses.Remove(current);
                }
                if (HasEnoughCouses() || !(EnoughCouresLeft()))
                    return;
            }
        }


        public bool CanTake(int parentPrereq)
        {
            var courses = _unprocessedCourses.ToList();
            foreach (var dependent in courses)
            {
                var dependentPrereq = dependent.Item1;
                var dependentCourse = dependent.Item2;

                if (dependentCourse == parentPrereq)
                {
                    _unprocessedCourses.Remove(dependent);
                    _blockedCourses.Push(dependentCourse);

                    if (!_blockedCourses.Contains(dependentPrereq) && CanTake(dependentPrereq))
                    {
                        _blockedCourses.Pop();
                        _validCourses.Add(dependentCourse);
                    }
                    else
                    {
                        _blockedCourses.Pop();
                        _invalidCourses.Add(dependentCourse);
                        _invalidCourses.Add(parentPrereq);
                        return false;
                    }
                }

                if (HasEnoughCouses() || !(EnoughCouresLeft()))
                    return false; //all done, we have a decision
            }

            _validCourses.Add(parentPrereq);
            return true;
        }
    }
}
