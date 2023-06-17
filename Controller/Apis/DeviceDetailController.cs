using Microsoft.AspNetCore.Mvc;

namespace MQTT_Api_Server_Lifesaver.Controller.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceDetailController : ControllerBase
    {
        private readonly IMongoDatabase mongo;
        private readonly IdGenerator Idgen;
        private readonly IHubContext<ClockHub, IClock> clockHub;

        public DeviceDetailController(IMongoDatabase mongo, IdGenerator idgen, IHubContext<ClockHub, IClock> clockHub)
        {
            this.mongo = mongo;
            this.Idgen = idgen;
            this.clockHub = clockHub;
        }
        [HttpGet]
        [Route("GetAllDevices")]
        public async Task<Result> GetAllDevices(int pagesize, int pagenumber, string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new InvalidApiRequestException("参数keywords为空，无法解析！");
            }
            if (keywords == "#")
            {
                long len = await mongo.DbGetCollection<DeviceDetail>().Find(s => true).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<DeviceDetail>().Find(s => true).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
            else
            {
                long len = await mongo.DbGetCollection<DeviceDetail>().Find(s => s.DeviceId.Contains(keywords)).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<DeviceDetail>().Find(s => s.DeviceId.Contains(keywords)).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
        }
    

        [HttpDelete("DeleteDevices")]
        public async Task<Result> DeleteDevices(List<DeviceDetail> Devices)
        {
            if (Devices.Count == 0)
            {
                throw new InvalidApiRequestException("参数Devices为空，无法解析！");
            }
            var filter = Builders<DeviceDetail>.Filter.Or(Devices.Select(lo => Builders<DeviceDetail>.Filter.Eq(s => s.Id, lo.Id)).ToList());
            var res = await mongo.DbGetCollection<DeviceDetail>().DeleteManyAsync(filter);
            if (res.IsAcknowledged)
            {
                return Result.Success("操作成功");
            }
            else
            {
                return Result.SuccessError("操作失败");
            }
        }
    }
}
