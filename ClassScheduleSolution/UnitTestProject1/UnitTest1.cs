using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ClassSchedule
{
    [TestClass]
    public class UnitTest1
    {
        private readonly int _numClasses = 512;
        private readonly List<Tuple<int, int>> _courses;
        public UnitTest1()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClassSchedule.data.json"))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();

                _courses = JsonConvert.DeserializeObject<List<Tuple<int, int>>>(result);
                Debug.WriteLine($"{_courses.Count} prerequisites listed.");
            }
        }

        [TestMethod]
        public void VladLaypaAnswer()
        {
            var stopwatch = new Stopwatch();
            for (int i = 0; i < 1000; i++)
            {
                stopwatch.Restart();
                Assert.IsTrue(IsSchedulePossibleVladLaypa(_numClasses, _courses));
                stopwatch.Stop();
            }
           
        }


        [TestMethod]
        public void NathanHaaseAnswer()
        {
            Assert.IsTrue(CanGraduateNathanHaase(_numClasses, _courses));

        }

        [TestMethod]
        public void DavidMamanAnswer()
        {
            Assert.IsTrue(CanGraduateMaman(_numClasses, _courses));

        }

        static bool CanGraduateNathanHaase(int numAllCourses, List<Tuple<int, int>> prerequisites)
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

        public bool CanGraduateMaman(int numAllCourses, List<Tuple<int, int>> prerequisites)
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

                Console.WriteLine(count);
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


        /// <summary>
        /// Kahn's algorithm for topological sort
        /// </summary>
        /// <param name="numOfClasses"></param>
        /// <param name="prereqs">Vertices</param>
        /// <returns></returns>
        private static bool IsSchedulePossibleVladLaypa(int numOfClasses, ICollection<Tuple<int, int>> prereqs)
        {
            var sortedList = new List<int>();

            var canTakeList = new HashSet<int>(prereqs
                .Where(w => prereqs.All(p => p.Item2 != w.Item1))
                .Select(s => s.Item1));

            while (canTakeList.Any())
            {
                var availCourse = canTakeList.First();
                canTakeList.Remove(availCourse);
                sortedList.Add(availCourse);

                if (numOfClasses <= sortedList.Count) return true;

                foreach (var tuple in prereqs.Where(e => e.Item1 == availCourse).ToList())
                {
                    var candidate = tuple.Item2;
                    prereqs.Remove(tuple);

                    if (prereqs.All(me => me.Item2 != candidate))
                        canTakeList.Add(candidate);
                }
            }
            Debug.WriteLine($"Needed to finish {numOfClasses} courses.");
            Debug.WriteLine($"Able to finish {sortedList.Count} courses.");
           
            return numOfClasses <= sortedList.Count;
        }


        /// <summary>
        /// Used to generate prereqs
        /// </summary>
        /// <returns></returns>
        private string GenerateClasses()
        {
            int numofClasses = 1000;
            var classes = new HashSet<Tuple<int, int>>();

            var rng = new Random();
            var courses = new HashSet<int>();
            for (var i = 0; i < numofClasses; i++)
            {
                courses.Add(rng.Next(numofClasses));
            }
          
            foreach (var course in courses)
            {
                var numofprereqs = rng.Next(0, 4);
                for (var i = 0; i < numofprereqs; i++)
                {
                    var prereq = courses.ElementAt(rng.Next(courses.Count - 1));
                    if (prereq > course)
                    {
                        classes.Add(new Tuple<int, int>(course, prereq));
                    }
                }
            }
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\data.json",
                  JsonConvert.SerializeObject(classes));
            return JsonConvert.SerializeObject(classes);
        }
    }
}