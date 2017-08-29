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



**Tools Used:-**
- Visual Studio 2017
- Docker

**Installation Instruction**

```
docker-compose -f docker-compose.ci.build.yml up
docker-compose up
```

This is a rough idea of the process of this system.
![alt text](http://www.codedsphere.com/wp-content/uploads/2017/08/Gonsystem.png)
