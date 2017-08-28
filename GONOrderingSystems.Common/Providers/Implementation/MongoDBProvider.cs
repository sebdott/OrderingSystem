using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Common.Providers.Interface;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace GONOrderingSystems.Common.Providers.Implementation
{
    public class MongoDBProvider : IDataProvider
    {
        private MongoClient _client;
        private IMongoDatabase _db;
        private string _orderCollection;

        public MongoDBProvider(string hostUri, string dbName, string orderCollection)
        {
            _client = new MongoClient(hostUri);
            _db = _client.GetDatabase(dbName);
            _orderCollection = orderCollection;
        }

        public async Task<bool> Save(Order order)
        {
            try
            {
                var _collection = _db.GetCollection<Order>(_orderCollection);
                await _collection.Indexes.CreateOneAsync(Builders<Order>.IndexKeys.Ascending(_ => _.OrderId));
                await _collection.InsertOneAsync(order);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<Order> Get(string OrderId)
        {
            try
            {
                var _collection = _db.GetCollection<Order>(_orderCollection);

                var filter = Builders<Order>.Filter.Eq<string>(s => s.OrderId, OrderId);
                //var filter = Builders<Order>.Filter.Gte("OrderId", OrderId);
                var returnValue = await _collection.FindAsync<Order>(filter);

                return await returnValue.FirstAsync<Order>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Update(Order order)
        {
            try
            {
                var _collection = _db.GetCollection<Order>(_orderCollection);
                
                var replaceOneResult = await _collection.ReplaceOneAsync(
                 s => s.OrderId == order.OrderId,
                 order);

                return replaceOneResult.IsAcknowledged;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
