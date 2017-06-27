using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassSchedule
{
    public partial class BasicTest
    {
        public bool CanGraduate_JoseArroyo(int numAllCourses, List<Tuple<int, int>> prerequisites)
        {
            //Edge case, wanted to have all bases covered     
            if (numAllCourses <= 0)
                return true;

            var coursesTaken = new HashSet<int>();
            coursesTaken = GetCoursesWithNoPrereqs(prerequisites);

            //if we meet the number necessary to graduate with the courses with no prereqs, return true
            if (coursesTaken.Count >= numAllCourses)
                return true;
            else if (coursesTaken.Count == 0)
                return false;

            return CanGraduateRecursive(numAllCourses, prerequisites, coursesTaken);
        }
        /// <summary>
        /// Gets all courses with no prereqs 
        /// </summary>
        /// <param name="prerequisites"></param>
        /// <returns></returns>
        private HashSet<int> GetCoursesWithNoPrereqs(List<Tuple<int, int>> prerequisites)
        {
            var coursesWithNoPrereqs = new HashSet<int>();
            for (int i = 0; i < prerequisites.Count; i++)
            {
                var course1 = prerequisites[i].Item1;
                bool canTakeCourse = !prerequisites.Any(x => x.Item2 == course1);
                if (canTakeCourse)
                {
                    coursesWithNoPrereqs.Add(course1);
                }
            }
            return coursesWithNoPrereqs;
        }

        /// <summary>
        /// iterates trought the courses to deteremine if the user can graduate. 
        /// </summary>
        /// <param name="numAllCourses"></param>
        /// <param name="prerequisites"></param>
        /// <param name="coursesTaken"></param>
        /// <returns></returns>
        private bool CanGraduateRecursive(int numAllCourses, List<Tuple<int, int>> prerequisites, HashSet<int> coursesTaken)
        {
            //get all courses that can be unlocked with the current courses taken
            var coursesUnlocked = prerequisites.Where(x => coursesTaken.Contains(x.Item1))
                .Select(x => x.Item2).ToList();

            //checks if the criteria for a course with multiple prereqs is met, if not, remove from courses unlocked
            var coursesWithMultiplePrereqs = coursesUnlocked.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key);
            foreach (var course in coursesWithMultiplePrereqs)
            {
                //if is not already in the coursesTaken, check if we can take the course
                if (!(coursesTaken.Contains(course)))
                {
                    HashSet<int> preReqsForCourse = GetPrereqsForCourse(prerequisites, course);
                    if (!(meetsRequirement(preReqsForCourse, coursesTaken)))
                        coursesUnlocked.RemoveAll(x => x == course);
                }
            }

            //if no courses are unlocked, return false
            if (coursesUnlocked.Count() == 0)
                return false;

            else
            {
                var newCoursesTaken = coursesUnlocked.Except(coursesTaken);
                //if not new courses where taken, return false
                if (newCoursesTaken.Count() == 0)
                    return false;
                //check if we have the required numbers of courses to graduate, if not, check again with the additional courses taken               
                coursesTaken.UnionWith(newCoursesTaken);
                if (coursesTaken.Count >= numAllCourses)
                    return true;
                else
                    return CanGraduateRecursive(numAllCourses, prerequisites, coursesTaken);
            }

        }

        /// <summary>
        /// gets the prereqs courses for a course
        /// </summary>
        /// <param name="prerequisites"></param>
        /// <param name="course"></param>
        /// <returns></returns>
        private HashSet<int> GetPrereqsForCourse(List<Tuple<int, int>> prerequisites, int course)
        {
            return new HashSet<int>(prerequisites.Where(x => x.Item2 == course).Select(x => x.Item1));
        }

        /// <summary>
        /// determines if the curses taken has the required courses to take an specifc class 
        /// </summary>
        /// <param name="preReqsForCourse"></param>
        /// <param name="coursesTaken"></param>
        /// <returns></returns>
        private bool meetsRequirement(HashSet<int> preReqsForCourse, HashSet<int> coursesTaken)
        {
            return preReqsForCourse.IsSubsetOf(coursesTaken);
        }
    }
}
