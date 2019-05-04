# Assignment13Database
This is the 13th. assignment for PBA Database soft2019spring

# What it is
This is a C# project containing the answers for [assignment 13](https://github.com/datsoftlyngby/soft2019spring-databases/blob/master/assignments/assignment13.md)<br>
The project makes use of 2 docker containers, one for each of the databases (MySql and Neo4j).<br>
The target runtime for C# is 2.2 (dotnetCore) [LinkHere](https://dotnet.microsoft.com/download)

# Setup

## Neo4j-setup<br>

1) Clone the repo
2) Download the data and decompress the data
```
wget https://github.com/datsoftlyngby/soft2019spring-databases/raw/master/data/archive_graph.tar.gz
tar -zxvf archive_graph.tar.gz
```
3) Spin up a docker container for Neo4j (I've used 8Gb, you should adjust accordingly to your hardware)
```
sudo docker run -d --name neo4j --rm --publish=7474:7474 --publish=7687:7687 -v $(pwd):/var/lib/neo4j/import --env NEO4J_AUTH=neo4j/test1234 --env=NEO4J_dbms_memory_pagecache_size=8G --env=NEO4J_dbms_memory_heap_initial__size=8G --env=NEO4J_dbms_memory_heap_max__size=8G neo4j
```
4) Log on to neo4j by navigating to [localhost:7474/browser](http://localhost:7474/browser/)
Credentials are 
```
username: neo4j
password: test1234
```
5a) Import the nodes-data by executing the following cypher in neo4j IDE <br>
*This will take several seconds: Added 500000 labels, created 500000 nodes, set 2000000 properties, completed after 32229 ms.*

```
USING PERIODIC COMMIT
LOAD CSV WITH HEADERS FROM "file:///social_network_nodes.csv" AS row  FIELDTERMINATOR ","
WITH row
CREATE (a:person {
    pid: toInteger(row.node_id),
    name: row.name,
    job: row.job,
    birthday: row.birthday
})
```

6) Create index on id on the person node
```
create index on :person(pid)
```

7) Import the edges-data and create the relations(Endorsments) by executing the following cypher in neo4j IDE <br>
*This cypher will take minutes(approx. 15 min.) to execute: Created 11205208 relationships, completed after 900644 ms.*

```
USING PERIODIC COMMIT
LOAD CSV WITH HEADERS FROM "file:///social_network_edges.csv" AS row  FIELDTERMINATOR ","
WITH row
Match (p1:person {pid: toInteger(row.source_node_id)}), (p2:person {pid: toInteger(row.target_node_id) })
create (p1)-[:Endorsments]->(p2)
```
## MySql-setup

1) Spin up a docker container with MySql
```
sudo docker run --rm --name mysql01 -p 3306:3306 -e MYSQL_ROOT_PASSWORD=pass1234 -d mysql
```
2) Copy the csv files into the container
```
sudo docker cp ./social_network_edges.csv mysql01:/SNE.csv
sudo docker cp ./social_network_nodes.csv mysql01:/SNN.csv
```
3) Open bash inside container
```
sudo docker exec -it mysql01 bash
```
4) Open Mysql shell (username: root - password: pass1234)<br>
```
mysql -u root -p  --local-infile
```
5) Adjust settings
```
SET GLOBAL local_infile = 1;
```
6) Create new database and tables
```
CREATE DATABASE social;
USE social;
CREATE TABLE nodes (node_id int, name varchar(100), job varchar(100), birthday varchar(50));
CREATE TABLE edges (source_node_id int, target_node_id int);
```
7) Load data from the csv-files (This will take a couple of minutes depending on hardware (approx. 2min. on my laptop)
```
LOAD DATA LOCAL INFILE './SNN.csv'  INTO TABLE nodes  FIELDS TERMINATED BY ','  ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS;

LOAD DATA LOCAL INFILE './SNE.csv'  INTO TABLE edges  FIELDS TERMINATED BY ','  ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS;
```
8) Add indicies (index on the nodes table is fast, but edges table will take a couple of mins.)
```
ALTER TABLE `social`.`nodes`
CHANGE COLUMN `node_id` `node_id` INT(11) NOT NULL ,
ADD PRIMARY KEY (`node_id`);

ALTER TABLE `social`.`edges`
ADD INDEX `inx_src` (`source_node_id` ASC),
ADD INDEX `inx_trg` (`target_node_id` ASC);
```

# Executing the program

1) Make sure you have atleast the dotnet-runtime installed
2) Execute the program 
```
dotnet run
```

*The main-method in the program.cs file (line 14) contains a variable*
```
var DEPTH=3;
```
*This has been set to 3 inorder not to bork down your computer - to run the full depth adjust to 5 -this will result in about 30mins. of runtime - see file TimerResults*

# Timing results

The follow is taken from the console output of the program, it is included in this [file](https://github.com/cph-js284/Assignment13Database/blob/master/TimerResults.txt)

```
Populating list with random Ids:
The following Ids, will beused for both the Neo4j and MySql queries

------------------------------------------------------
[459476] [118681] [160192] [350874] [366743] 
[473610] [97471] [36477] [82983] [408860] 
[128495] [362277] [275972] [406809] [221434] 
[427763] [429125] [197669] [202024] [384795] 


--------------NEO4J-DEPTH 1---------------------------------------
RESULT :
20 executions of depth 1
Total excutiontime: 611 ms.
Avg executiontime: 30.55 ms.
Median executiontime: 8 ms.


--------------NEO4J-DEPTH 2---------------------------------------
RESULT :
20 executions of depth 2
Total excutiontime: 1523 ms.
Avg executiontime: 76.15 ms.
Median executiontime: 42 ms.


--------------NEO4J-DEPTH 3---------------------------------------
RESULT :
20 executions of depth 3
Total excutiontime: 30418 ms.
Avg executiontime: 1520.9 ms.
Median executiontime: 750 ms.


--------------NEO4J-DEPTH 4---------------------------------------
RESULT :
20 executions of depth 4
Total excutiontime: 72947 ms.
Avg executiontime: 3647.35 ms.
Median executiontime: 2310 ms.


--------------NEO4J-DEPTH 5---------------------------------------
RESULT :
20 executions of depth 5
Total excutiontime: 1608423 ms.
Avg executiontime: 80421.15 ms.
Median executiontime: 61427 ms.


----------------CONTAINER SWITCH--------------------------------


--------------MYSQL-DEPTH 1---------------------------------------
RESULT :
20 executions of depth 1
Total excutiontime: 17 ms.
Avg executiontime: 0.85 ms.
Median executiontime: 1 ms.


--------------MYSQL-DEPTH 2---------------------------------------
RESULT :
20 executions of depth 2
Total excutiontime: 46 ms.
Avg executiontime: 2.3 ms.
Median executiontime: 2 ms.


--------------MYSQL-DEPTH 3---------------------------------------
RESULT :
20 executions of depth 3
Total excutiontime: 342 ms.
Avg executiontime: 17.1 ms.
Median executiontime: 6 ms.


--------------MYSQL-DEPTH 4---------------------------------------
RESULT :
20 executions of depth 4
Total excutiontime: 8873 ms.
Avg executiontime: 443.65 ms.
Median executiontime: 73 ms.


--------------MYSQL-DEPTH 5---------------------------------------
RESULT :
20 executions of depth 5
Total excutiontime: 186368 ms.
Avg executiontime: 9318.4 ms.
Median executiontime: 4870 ms.



Done...press any key to terminate

```

# Conclusion and observations
Due to the fact that the list of the 20 random Id's get recalculated each time the program is executed its difficult to compare results.<br>
<br>
There seems to be a spin-lag when the program is connecting to the databases in the DEPTH 1 executions (it might be something entirely different)
<br><br>
The Mysql connections are set up to be *async* the Neo4j connection is not.
