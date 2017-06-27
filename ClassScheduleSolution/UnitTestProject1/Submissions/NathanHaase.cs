using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassSchedule
{
    public partial class BasicTest
    {
        public static bool CanGraduate_NathanHaase(int numAllCourses, List<Tuple<int, int>> prerequisites)
        {
            int courseCount;
            var coursesTaken = new List<int>();
            Tuple<int, int> prerequisite;
            while (true)
            {
                courseCount = prerequisites.Count();
                for (int j = prerequisites.Count(); j-- > 0;)
                {
                    prerequisite = prerequisites[j];
                    if (!prerequisites.Exists(x => x.Item2 == prerequisite.Item1))
                    {
                        prerequisites.RemoveAt(j);
                        if (!coursesTaken.Contains(prerequisite.Item1))
                            coursesTaken.Add(prerequisite.Item1);
                        if (!prerequisites.Exists(x => x.Item2 == prerequisite.Item2) &&
                            !coursesTaken.Contains(prerequisite.Item2))
                            coursesTaken.Add(prerequisite.Item2);
                        if (coursesTaken.Count() >= numAllCourses)
                            return true;
                    }
                }
                if (courseCount == prerequisites.Count() || prerequisites.Count() == 0)
                    return false;
            }
        }
    }
}