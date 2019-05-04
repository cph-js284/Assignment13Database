using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Neo4j.Driver.V1;
using Neo4jClient;

namespace Assignment13Database
{

    public class PersonCLR
    {
        public int id { get; set; }
        public string name { get; set; }
        public string job { get; set; }
        public string birthday { get; set; }
    }

    public class MyNeo
    {
        private List<int> RandomIdList;
        private Stopwatch stopwatch;

        public MyNeo(List<int> _list)
        {
            RandomIdList = _list;
            stopwatch = new Stopwatch();
        }


        public List<long> CalcDepth(int depth){
                List<long> esecList = new List<long>();
                using (var client = new BoltGraphClient("bolt://localhost:7687", "neo4j", "test1234"))
                {
                    client.Connect();
                    for (int i = 0; i < RandomIdList.Count; i++)
                    {
                        stopwatch.Reset();
                        stopwatch.Start();
                        var query = client.Cypher
                                    .Match("(a:person {pid:"+ RandomIdList[i] +"})-[:Endorsments*1.."+ depth +"]->(b:person)")
                                    .Return ((b) => new {
                                        Person = b.As<PersonCLR>()
                                    });
                        var res = query.Results;
                        
                    stopwatch.Stop();
                    esecList.Add(stopwatch.ElapsedMilliseconds);
                    //I've left this line of code inhere should you want to verify
                    //System.Console.WriteLine($"\nFound relationships : {res.Count()} for person with id : {RandomIdList[i]}" );
                    }
                }
                return esecList;
        }
    }
}
