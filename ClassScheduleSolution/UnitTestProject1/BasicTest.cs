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
    public partial class BasicTest
    {
        private bool CanGraduate(int numAllCourses, List<Tuple<int, int>> prerequisites)
        {
            return CanGraduate_Dorota(numAllCourses, prerequisites);
            //return CanGraduate_MichaelMoore(numAllCourses, prerequisites);
            //return CanGraduate_DavidMaman(numAllCourses, prerequisites);
            //return CanGraduate_JoseArroyo(numAllCourses, prerequisites);
            //return CanGraduate_DavidGreen(numAllCourses, prerequisites);
            //return CanGraduate_JoeRohde(numAllCourses, prerequisites);
            //return CanGraduate_NathanHaase(numAllCourses, prerequisites);
            //return CanGraduate_VladLaypa(numAllCourses, prerequisites);
        }


        [TestMethod]
        public void AvgTimeTest()
        {
            var result = true;
            const double numTests = 1000;

            var stopwatch = new Stopwatch();
            var testResults = new List<double>();
            for (var i = 0; i < numTests; i++)
            {
                const int numClasses = 512;
                List<Tuple<int, int>> courses;
                using (var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("ClassSchedule.data.json"))
                using (var reader = new StreamReader(stream))
                {
                    var fileString = reader.ReadToEnd();

                    courses = JsonConvert.DeserializeObject<List<Tuple<int, int>>>(fileString);
                }

                stopwatch.Restart();
                if (!CanGraduate(numClasses, courses))
                    result = false;
                stopwatch.Stop();
                testResults.Add(stopwatch.ElapsedMilliseconds);
            }

            Assert.IsTrue(result);
            Debug.WriteLine($"Avg. time: {testResults.Sum() / numTests} ms for {numTests} runs;");
        }

        //[TestMethod]
        //public void HugeDataset()
        //{
        //    var result = true;
        //    var stopwatch = new Stopwatch();
        //    List<Tuple<int, int>> courses;
        //    using (var stream = Assembly.GetExecutingAssembly()
        //        .GetManifestResourceStream("ClassSchedule.hugedata.json"))
        //    using (var reader = new StreamReader(stream))
        //    {
        //        var fileString = reader.ReadToEnd();

        //        courses = JsonConvert.DeserializeObject<List<Tuple<int, int>>>(fileString);
        //    }


        //    stopwatch.Start();
        //    if (!CanGraduate(100000, courses))
        //        result = false;
        //    stopwatch.Stop();

        //    Assert.IsTrue(result);
        //    Debug.WriteLine($"time: {stopwatch.ElapsedMilliseconds} ms.");
        //}
        
        [TestMethod]
        public void BasicTestWithOnePrereq()
        {
            Assert.IsTrue(CanGraduate(2, new List<Tuple<int, int>>
            {
                new Tuple<int, int>(101, 206)
            }));
        }

        [TestMethod]
        public void NoNeedToTakeAnyClasses()
        {
            Assert.IsTrue(CanGraduate(0, new List<Tuple<int, int>>
            {
                new Tuple<int, int>(101, 206),
                new Tuple<int, int>(206, 101)
            }));
        }

        //There is 1 course needed to take. 
        //To take course 206 you should have finished course 101. 
        //And to finish 101 you need to take 206 . So it is impossible.
        [TestMethod]
        public void CircularReferenceTest()
        {
            Assert.IsFalse(CanGraduate(1, new List<Tuple<int, int>>
            {
                new Tuple<int, int>(101, 206),
                new Tuple<int, int>(206, 101)
            }));
        }

        //There are a total of 3 courses you must take.
        //And there is a total of 5 courses possible to take. 
        //206 and 101 are not possible for the same reason as above. 
        //105 and 106 are both possible because they have no prerequisites, 
        //and since both can be completed so can 211. So it is Possible
        [TestMethod]
        public void HaveToComplete3OutOf5()
        {
            Assert.IsTrue(CanGraduate(3, new List<Tuple<int, int>>
            {
                new Tuple<int, int>(101, 206),
                new Tuple<int, int>(206, 101),
                new Tuple<int, int>(211, 105),
                new Tuple<int, int>(211, 106),
            }));
        }

        //should be sthe same result as RunTest3
        [TestMethod]
        public void DuplicatePrereqs()
        {
            Assert.IsTrue(CanGraduate(3, new List<Tuple<int, int>>
            {
                new Tuple<int, int>(101, 206),
                new Tuple<int, int>(206, 101),
                new Tuple<int, int>(211, 105),
                new Tuple<int, int>(211, 106),
                new Tuple<int, int>(101, 206),
                new Tuple<int, int>(206, 101),
                new Tuple<int, int>(211, 105),
                new Tuple<int, int>(211, 106),
            }));
        }

        //should fail since we are asking for 5 courses, should be 3 max
        [TestMethod]
        public void AsktoComplete5butOnly3available()
        {
            Assert.IsFalse(CanGraduate(5, new List<Tuple<int, int>>
            {
                new Tuple<int, int>(101, 206),
                new Tuple<int, int>(206, 101),
                new Tuple<int, int>(211, 105),
                new Tuple<int, int>(211, 106),
                new Tuple<int, int>(101, 206),
                new Tuple<int, int>(206, 101),
                new Tuple<int, int>(211, 105),
                new Tuple<int, int>(211, 106),
            }));
        }

        //not enough coures left
        [TestMethod]
        public void ShouldntPassNotEnoughClasses()
        {
            Assert.IsFalse(CanGraduate(7, new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(3, 4),
                new Tuple<int, int>(5, 6),
            }));
        }

        [TestMethod]
        public void HasZerosAndNegatives()
        {
            Assert.IsTrue(CanGraduate(3, new List<Tuple<int, int>>
            {
                new Tuple<int, int>(-1, 2),
                new Tuple<int, int>(3, 4),
                new Tuple<int, int>(5, 6),
                new Tuple<int, int>(5, -1),
                new Tuple<int, int>(0, -1),
                new Tuple<int, int>(6, 5)
            }));
        }


        //[TestMethod]
        //public void HasZejhgfhgjfrosAndNegatives()
        //{
        //    GenerateClasses();
        //    Assert.IsTrue(true);
        //}


        /// <summary>
        /// Used to generate prereqs
        /// </summary>
        /// <returns></returns>
        private string GenerateClasses()
        {
            int numofClasses = 500000;
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