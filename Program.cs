using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment13Database
{
    class Program
    {
        //list of IDs the same list is used to query both databases.
        static List<int> rnglist = new List<int>();
        static void Main(string[] args)
        {
            /*ADJUST THIS VARIABLE TO CHANGE THE DEPTH OF RELATIONS (A's of A's of A's ....) */
            var DEPTH=3;
            /*************************************************** */
            System.Console.WriteLine("Populating list with random Ids:");
            Populatelist();
            printIds();
            ExecuteNeo4j(DEPTH);
            System.Console.WriteLine("\n----------------CONTAINER SWITCH--------------------------------");
            ExecuteMySql(DEPTH);

            System.Console.WriteLine("\n\n\nDone...press any key to terminate");
            Console.ReadKey();

        }



        //pick out 20 random id's between 0-500000
        static void Populatelist(){
            Random rng = new Random();

            for (int i = 0; i < 20; i++)
            {
                rnglist.Add(rng.Next(0,500000));
            }
        }

        //prettyprint MYSQL
        static void ExecuteMySql(int depth){
            MySql MS = new MySql(rnglist);
            List<long> res = new List<long>();
            for (int j = 1; j <= depth; j++)
            {
                System.Console.WriteLine($"\n\n--------------MYSQL-DEPTH {j}---------------------------------------");
                res = MS.readData(j);
                res.Sort();
                var median = (res.Count)/2;
                System.Console.WriteLine($"RESULT :\n{res.Count} executions of depth {j}\nTotal excutiontime: {res.Sum()} ms.\nAvg executiontime: {res.Average()} ms.\nMedian executiontime: {res[median]} ms.");
            }    
        }

        //prettyprint NEO4J
        static void ExecuteNeo4j(int depth){
            MyNeo MN = new MyNeo(rnglist);
            for (int j = 1; j <= depth; j++)
            {
                System.Console.WriteLine($"\n\n--------------NEO4J-DEPTH {j}---------------------------------------");
                var res = MN.CalcDepth(j);
                res.Sort();
                var median = (res.Count)/2;
                System.Console.WriteLine($"RESULT :\n{res.Count} executions of depth {j}\nTotal excutiontime: {res.Sum()} ms.\nAvg executiontime: {res.Average()} ms.\nMedian executiontime: {res[median]} ms.");
            }

        }

        //prettyprint ID-list
        static void printIds(){
            System.Console.WriteLine("The following Ids, will beused for both the Neo4j and MySql queries");
            var i =0;
            System.Console.WriteLine("\n------------------------------------------------------");
            foreach (var item in rnglist)
            {
                System.Console.Write($"[{item}] ");
                i++;
                if(i%5==0)
                    System.Console.WriteLine();
            }
        }
    }
}
