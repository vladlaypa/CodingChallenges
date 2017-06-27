using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassSchedule
{
    public partial class BasicTest
    {
        public bool CanGraduate_DavidMaman(int numAllCourses, List<Tuple<int, int>> prerequisites)
        {
            try
            {
                if (prerequisites == null)
                {
                    return false;
                }
                if (numAllCourses <= 0)
                {
                    return false;
                }

                List<int> courses = new List<int>();
                List<int> distinctCourses = new List<int>();

                foreach (Tuple<int, int> p in prerequisites)
                {
                    courses.Add(p.Item1);
                }

                distinctCourses = courses.Distinct<int>().ToList<int>();
                distinctCourses.Sort();

                List<Tuple<int, List<int>>> courseAndPrereqs = new List<Tuple<int, List<int>>>();
                foreach (int dc in distinctCourses)
                {
                    Tuple<int, List<int>> temp = new Tuple<int, List<int>>(dc, new List<int>());
                    courseAndPrereqs.Add(temp);
                }

                //find all courses that need prereqs, add to course list
                foreach (Tuple<int, List<int>> cp in courseAndPrereqs)
                {
                    int course = cp.Item1;

                    foreach (Tuple<int, int> p in prerequisites)
                    {
                        if (p.Item1.Equals(course) && !cp.Item2.Contains(p.Item2))
                        {
                            cp.Item2.Add(p.Item2);
                        }
                    }
                }

                //if course has a prereq and that prereq has that course in it's list, remove from their list of eligible prereqs
                foreach (Tuple<int, List<int>> tc in courseAndPrereqs)
                {
                    int course = tc.Item1;
                    foreach (Tuple<int, List<int>> tp in courseAndPrereqs)
                    {
                        if (tp.Item1.Equals(course))
                        {
                            continue;
                        }
                        else
                        {
                            if (tp.Item2.Contains(course) && tc.Item2.Contains(tp.Item1))
                            {
                                tp.Item2.Remove(course);
                                tc.Item2.Remove(tp.Item1);
                            }
                        }
                    }
                }

                //find the count of possible courses
                int count = 0;
                foreach (Tuple<int, List<int>> tcp in courseAndPrereqs)
                {
                    if (tcp.Item2.Count() > 0)
                    {
                        // 1 for current course
                        count += 1 + tcp.Item2.Count();
                    }
                }

                //Console.WriteLine(count);
                if (count >= numAllCourses)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                GC.Collect();
            }
            return true;
        }
    }
}