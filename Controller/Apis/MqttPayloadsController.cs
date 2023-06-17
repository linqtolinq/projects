using Microsoft.AspNetCore.Mvc;

namespace MQTT_Api_Server_Lifesaver.Controller.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class MqttPayloadsController : ControllerBase
    {
        private readonly IMongoDatabase mongo;
        private readonly IdGenerator Idgen;
        public MqttPayloadsController(IMongoDatabase mongo, IdGenerator idgen)
        {
            this.mongo = mongo;
            this.Idgen = idgen;
        }
        [HttpGet]
        [Route("GetAllPayloads")]
        public async Task<Result> GetAllPayloads(int pagesize, int pagenumber, string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new InvalidApiRequestException("参数keywords为空，无法解析！");
            }
            if (keywords == "#")
            {
                long len = await mongo.DbGetCollection<MqttPayload>().Find(s => true).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<MqttPayload>().Find(s => true).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
            else
            {
                long len = await mongo.DbGetCollection<MqttPayload>().Find(s => s.ClientId.Contains(keywords)).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<MqttPayload>().Find(s => s.ClientId.Contains(keywords)).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
        }


        [HttpDelete("DeletePayloads")]
        public async Task<Result> DeletePayloads(List<MqttPayload> Payloads)
        {
            if (Payloads.Count == 0)
            {
                throw new InvalidApiRequestException("参数Payloads为空，无法解析！");
            }
            var filter = Builders<MqttPayload>.Filter.Or(Payloads.Select(lo => Builders<MqttPayload>.Filter.Eq(s => s.Id, lo.Id)).ToList());
            var res = await mongo.DbGetCollection<MqttPayload>().DeleteManyAsync(filter);
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
