using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace Assignment13Database
{
    public class MySql
    {
        private string connectionString;
        private Stopwatch stopwatch;
        private List<long> esecList;
        private List<int> RandomIdList;


        public MySql(List<int> _RandomIdList)

        {
            connectionString=$"Server=localhost;Database=social;Uid=root;Pwd=pass1234;";
            stopwatch = new Stopwatch();
            RandomIdList=_RandomIdList;
            // RandomIdList=new List<int>();
            // RandomIdList.Add(0);
        }   


        private string sqlbuilder(int depth){
            var beginning = @"select count(distinct persons" + depth + @".node_id) from nodes";
            var joins="";            
            for (int i = 1; i <= depth; i++)
            {
             joins += @" left join edges as endorsments" + i + @" on nodes.node_id = endorsments" + i + @".source_node_id
                        left join nodes as persons" + i + @" on endorsments" + i + @".target_node_id= persons" + i + @".node_id";
            }           
            var ending = @" where nodes.node_id = ";

            return beginning + joins + ending;
        }



        public List<long> readData(int depth){
            esecList = new List<long>();
            string sqlstr="";


            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                    for (int i = 0; i < RandomIdList.Count-15; i++)
                    {
                        sqlstr = sqlbuilder(depth) + RandomIdList[i];
                        stopwatch.Reset();
                        stopwatch.Start();    
                            using (var command = new MySqlCommand(sqlstr, conn))
                                {var reader = command.ExecuteReaderAsync(); }
                        stopwatch.Stop();
                        esecList.Add(stopwatch.ElapsedMilliseconds);
                    }
            }
            return esecList;
        }
        
    }
}