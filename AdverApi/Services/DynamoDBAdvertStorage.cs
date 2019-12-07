using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace AdverApi.Services
{
    public class DynamoDBAdvertStorage : IadvertStorageService
    {
        private readonly IMapper _mapper;

        public DynamoDBAdvertStorage(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async  Task<string> Add(AdvertModel model)
        {
            System.IO.File.WriteAllText(@"C:\inetpub\logfiles\log.txt", "Inside Add");
            var dbmodel = _mapper.Map<AdvertDBModel>(model);
            dbmodel.Id = new Guid().ToString();
            dbmodel.CreationDateTime = DateTime.UtcNow;
            dbmodel.Status = AdvertStatus.Pending;
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync(dbmodel);
                }
            }
            return dbmodel.Id;
        }

        public async Task<bool> CheckHealthAsync()
        {
            using (var client = new AmazonDynamoDBClient())
            {
                var tabledata = await client.DescribeTableAsync("Adverts");
                return string.Compare(tabledata.Table.TableStatus, "active", true) == 0;
            }
        }

        public async Task Confirm(ConfirmAdvertModel model)
        {
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    var record = await context.LoadAsync<AdvertDBModel>(model.Id);
                    if (record==null)
                    {
                        throw new KeyNotFoundException($"A record with ID={model.Id} was not found");
                    }
                    if (model.Status==AdvertStatus.Active)
                    {
                        record.Status = AdvertStatus.Active;
                        await context.SaveAsync(record);
                    }
                    else
                    {
                        await context.DeleteAsync(record); 
                    }
                }
            }
        }
    }
}
