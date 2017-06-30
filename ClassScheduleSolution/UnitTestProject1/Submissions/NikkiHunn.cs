using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassSchedule
{
    public partial class BasicTest
    {
        private static List<Course> _courseList = null;
        public static bool CanGraduate_NikkiHunn(int numAllCourses, List<Tuple<int, int>> prerequisites)
        {
            // basic argument checks
            if (prerequisites == null) return false;  // bad data
            if (numAllCourses < 0) return false;  // bad data
            if (numAllCourses == 0) return true; // if you don't need any, guess you're golden

            // We must process the entire list to know all the prereqs. Can't start counting until we're sure.
            // the FIRST int, Item1, is the prereq for the SECOND item, Item2, in the Tuple
            _courseList =
                prerequisites
                    .Select(x => x.Item1)
                    .Union(prerequisites.Select(y => y.Item2))
                    .Distinct().Select(x => new Course()
                    {
                        CourseNumber = x,
                        Prereqs = prerequisites.Where(y => y.Item2 == x).Select(z => z.Item1).ToList()
                    }).ToList();

            // if the number of "root" courses (no prereqs) is greater than the asked for numCourses, 
            // we don't need to get fancy
            var rootCourses = _courseList.Where(x => x.Prereqs.Count == 0).ToList();
            int totalCompletableSoFar = rootCourses.Count();
            if (totalCompletableSoFar >= numAllCourses)
            {
                return true;
            }
            else if (totalCompletableSoFar == 0)
            {
                // no root courses, can't complete anything
                return false;
            }
            else
            {
                // to be completable, a course must be able to be traced back to its root course(s)
                // e.g. if 501 needs 401 which needs 301, which needs 201 and 202, and 201 needs 101 while 202 needs 102
                // bad data may include infinite loop condition such as 201 needs 101 but 101 has 201 prereq
                // or even one prereq is completable but the other has a loop condition
                foreach (var childCourse in _courseList.Where(x => x.Prereqs.Count > 0))
                {
                    _courseList.ForEach(x => x.Visited = false);
                    if (HasPathToRoot(childCourse.CourseNumber, rootCourses)) totalCompletableSoFar++;
                    if (totalCompletableSoFar >= numAllCourses) return true;  // short circuit, we are done
                }
                return false;
            }

        }

        private static bool HasPathToRoot(int courseNumber, List<Course> rootCourses)
        {
            var course = _courseList.First(x => x.CourseNumber == courseNumber);
            if (rootCourses.Any(x => x.CourseNumber == courseNumber)) return true;
            else if (course.Prereqs.Count() == 0)
            {
                return false; // has no prereq but is not a root? bad data
            }
            else if (course.Visited)
            {
                return false;  // circular
            }
            else
            {
                course.Visited = true;
                return course.Prereqs.All(x => HasPathToRoot(x, rootCourses));
            }
        }
    }
    internal class Course
    {
        internal int CourseNumber { get; set; }
        internal List<int> Prereqs { get; set; }
        internal bool Visited { get; set; }
    }

}
