using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = true;
            const long numTests = 1;
            
            for (var i = 0; i < numTests; i++)
            {
                const int numClasses = 512;
                List<Tuple<int, int>> courses;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PerformanceTest.data.json"))
                using (var reader = new StreamReader(stream))
                {
                    var fileString = reader.ReadToEnd();

                    courses = JsonConvert.DeserializeObject<List<Tuple<int, int>>>(fileString);
                    //Debug.WriteLine($"{numClasses} courses...");
                    //Debug.WriteLine($"{courses.Count} prerequisites....");
                }


                var first = courses.Select(s => s.Item1).ToList();
                var second = courses.Select(s => s.Item2).ToList();
                

                var takenCount = 0;

                //var canTakeList = new HashSet<int>(courses.Where(w => courses.All(p => p.Item2 != w.Item1)).Select(s => s.Item1).ToList());
                var canTakeList = new HashSet<int>(first.Except(second));

                while (canTakeList.Any())
                {
                    var availCourse = canTakeList.First();
                    canTakeList.Remove(availCourse);
                    takenCount++;

                    if (numClasses <= takenCount)
                        return;

                    foreach (var tuple in courses.Where(e => e.Item1 == availCourse).ToList())
                    {
                        var candidate = tuple.Item2;
                        courses.Remove(tuple);

                        if (courses.Any(me => me.Item2 == candidate))
                            continue;
                            canTakeList.Add(candidate);
                    }
                }
                //Debug.WriteLine($"Needed to finish {numOfClasses} courses.");
                //Debug.WriteLine($"Able to finish {sortedList.Count} courses.");

                return ;
            }
        }
    }
}
