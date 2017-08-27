using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Common.Providers.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GONOrderingSystems.Common.Providers.Implementation
{
    public class LocalDBProvider : IDataProvider
    {

        public async Task<bool> Save(Order order)
        {
            return true;
        }

        public async Task<Order> Get(string ID)
        {
            return new Order();
        }

        public async Task<bool> Update(Order order)
        {
            return true;
        }

    }
}
