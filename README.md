# GON OrderingSystem

The Ordering System is a Rest API backend processor 
designed to handle group orders from multiple customers

This is the list of technologies used :- 

**Technologies:-**

- **Main** : .NET Core 1.1 , C#
- **Container**: Docker (https://www.docker.com/)
- **Metric** : Promethues (https://prometheus.io/)
- **Log Management** : Graylog (https://www.graylog.org)
- **DB**: Mongo DB  (https://www.mongodb.com/)
      Elastic Search - Graylog  (www.elastic.co/)
- **PubSub/Message/Stream** : Kafka (https://www.confluent.io/)
                          ZooKeeper (https://zookeeper.apache.org/)
- **Docker Management UI** : Portainer (https://portainer.io/)

This is a rough idea of the process of this system.
![alt text](http://www.codedsphere.com/wp-content/uploads/2017/08/Gonsystem.png)

**Tools Used:-**
- Visual Studio 2017
- Docker

**Installation Instruction**

These are the few steps required to set up all the environments in your machine.

First of all you need docker to be set up in your machine. 
For me I am running on Windos OS which my docker containers will be running on Linux settings
You can go to (https://www.docker.com/community-edition) to get the latest version of docker downloads

Go to powershell

Run this command to build this source

```
docker-compose -f docker-compose.ci.build.yml up
```

Run this command to bring up all the applications and environments in docker
```
docker-compose up
```

Once the environment is being brought up you will have the list of application running on your machine.

**Local Path/Ports** : -

- **Web Api** : localhost:801/swagger , localhost:801/api/metrics
- **Zookeeper** : localhost:32182
- **Kafka** : localhost:9092
- **Prometheus** : localhost:9090
- **Gafana** : localhost:3000
- **GrayLog** : localhost:9038
- **MongoDB** : localhost:27017
- **ElasticSearch** : localhost:9200
- **Portainer** : localhost:9000


I have ported the necessary port numbers to your machine ports
If your machine have occupied these port numbers you may change it in the docker compose config file (docker-compose.yml) 

```
Find the line -p <public port>:<private port> and do the necessary modifications.
```

There are a few applications that contains a UI screen for better user management
I have already pre-set all the username and password settings. You can login to the accounts below.


**Portainer : Docker Managmenet UI :  **
```
link : localhost:9000 
username : gonadmin 
password : gonadmin
```

**Grafana : Metric Display :**  
```
link : localhost:9038 
username : gonadmin 
password : gonadmin
```

** GrayLog : Log Management : **  
```
link : localhost:9038 
username : admin 
password : admin
```

There are some additional steps required for GrayLog in order to work properly

 - Go to GrayLog link **localhost:9038 **

 - Login to GrayLog using these credentials

```
username: admin
password: admin
```
     
 - Go to **System** > **Content Packs** and click on the **Import Contect Pack**.

Upload the file in the source
```
.\DockerConfig\graylog\content_pack.json
```


Once uploaded, you are ready to use Graylog to get all the log information from **GON Ordering system**
