using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassSchedule
{
    public partial class BasicTest
    {
        public static bool CanGraduate_Dorota(int numAllCourses, List<Tuple<int, int>> prerequisites)
        {
            var goodBranches = new List<List<int>>();
            var badBranches = new List<List<int>>();

            //Sneaky if statements
            if (numAllCourses == 0)
            {
                return true;
            }

            if (numAllCourses < 0)
            {
                return false; //making sure you get good parameters
            }

            //If all the classes only show up once, you have a line, or a bunch of little ones
            List<int> classes = new List<int>();
            foreach (var prereq in prerequisites)
            {
                classes.Add(prereq.Item1);
                classes.Add(prereq.Item2);
            }

            if (classes.ToList().Distinct().Count() == classes.ToList().Count && classes.ToList().Distinct().Count() >= numAllCourses)
            {
                return true;
            }

            if (numAllCourses > classes.ToList().Distinct().Count())
            {
                return false;
            }

            foreach (Tuple<int, int> t in prerequisites)
            {
                var branch = new List<int>();
                // First one can't be a cycle so just initialize it
                var startCourse = t.Item1;
                var nextNode = t.Item2;
                branch.Add(startCourse);
                branch.Add(nextNode);

                var initialCountOfBadBranches = 0;

                //Fill out a branch
                for (var i = 0; i < prerequisites.Count; i++)
                {
                    var workingClass = prerequisites[i].Item1;

                    if (workingClass == branch.Last())
                    {
                        //Then add the next course in the circulum
                        branch.Add(prerequisites[i].Item2);
                    }
                    //Check for dupes. That means there is a cycle
                }

                //Look at content of branches
                if (branch.Count != branch.Distinct().Count())
                {
                    //Remove the branch 
                    badBranches.Add(branch);
                    initialCountOfBadBranches = initialCountOfBadBranches + 1;
                }
                else if (branch.Count == numAllCourses)
                {
                }
                //Once done with loop, clear branch and start over
                if (initialCountOfBadBranches == 0)
                {
                    goodBranches.Add(branch);
                }
            }

            //Now clean up bc your badBranch might actually reference the parent node. Unwind everything. 
            //I don't like how i did this
            foreach (var breakers in badBranches.Select(branch => branch.GroupBy(y => y)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key).Distinct()
                .ToList()))
            {
                List<List<int>> nowBad;
                while (breakers.Count > 0 && goodBranches.Count > 0)
                {
                    //find all bad branches 
                    var workingBreaker = breakers[breakers.Count - 1];

                    nowBad = goodBranches.Where(a => a.Contains(workingBreaker)).ToList();
                    breakers.AddRange(from bad in nowBad let place = bad.FindIndex(b => b.Equals(workingBreaker)) where bad.Last() != workingBreaker select bad[place + 1]);

                    //delete the goodbranches. They don't count anymore
                    foreach (var junk in nowBad)
                    {
                        goodBranches.Remove(junk);
                    }

                    breakers.Remove(workingBreaker);
                }
            }

            var legitClassesICanTake = goodBranches.SelectMany(g => g).Distinct().ToList();

            if (legitClassesICanTake.Count >= numAllCourses)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}