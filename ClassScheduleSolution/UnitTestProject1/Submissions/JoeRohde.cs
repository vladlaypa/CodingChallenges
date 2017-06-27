using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassSchedule
{
    public partial class BasicTest
    {
        public static bool CanGraduate_JoeRohde(int numAllCourses, List<Tuple<int, int>> prerequisites)
        {
            var result = false;
            var numberOfClasses = 0;
            var classList = new List<int>();
            var addedClass1 = false;
            var addedClass2 = false;

            //Build Class List
            foreach (var classes in prerequisites)
            {
                // Does Class 1 Exist
                if (!classList.Exists(x => x.Equals(classes.Item1)))
                {
                    // No... Add it to the Class List
                    classList.Add(classes.Item1);
                    addedClass1 = true;
                    numberOfClasses++;
                }

                // Does Class 2 Exist
                if (!classList.Exists(x => x.Equals(classes.Item2)))
                {
                    // No... Add it to the Class List
                    classList.Add(classes.Item2);
                    addedClass2 = true;
                    numberOfClasses++;
                }

                // Check for the Circular classes
                if (prerequisites.Any(c => c.Item1 == classes.Item2 && c.Item2 == classes.Item1) && (addedClass1 && addedClass2))
                {
                    // Remove from the Number of Classes Count
                    numberOfClasses = numberOfClasses - 2;
                }

                // Does this meet the number of classes
                if (numberOfClasses >= numAllCourses)
                {
                    result = true;
                    break;
                }

                addedClass1 = false;
                addedClass2 = false;
            }

            return result;
        }
    }
}
