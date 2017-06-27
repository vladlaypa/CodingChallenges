using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassSchedule
{
    public partial class BasicTest
    {
        public static bool CanGraduate_VladLaypa(int numOfClasses, List<Tuple<int, int>> prereqs)
        {
            var takenCount = 0;
            var firstItems = prereqs.Select(s => s.Item1);
            var secondItems = prereqs.Select(s => s.Item2);
            var canTakeList = new List<int>(firstItems.Except(secondItems));

            while (canTakeList.Any())
            {
                var availCourse = canTakeList.First();
                canTakeList.RemoveAt(0);
                takenCount++;
                if (numOfClasses <= takenCount) return true;
                foreach (var tuple in prereqs.Where(e => e.Item1 == availCourse).ToList())
                {
                    var candidate = tuple.Item2;
                    prereqs.Remove(tuple);

                    if (prereqs.Any(me => me.Item2 == candidate)) continue;
                    canTakeList.Add(candidate);
                }
            }
            return numOfClasses <= takenCount;
        }
    }
}