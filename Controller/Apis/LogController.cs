using Amazon.Runtime;
using Emqx.Exhook.V2;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;


namespace MQTT_Api_Server_Lifesaver.Controller.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly IMongoDatabase mongo;
        private readonly IdGenerator Idgen;
        public LogController(IMongoDatabase mongo, IdGenerator idgen)
        {
            this.mongo = mongo;
            this.Idgen = idgen;
        }
        [HttpGet]
        [Route("GetAllLogs")]
        public async Task<Result> GetAllLogs(int pagesize, int pagenumber, string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new InvalidApiRequestException("参数keywords为空，无法解析！");
            }
            if (keywords == "#")
            {
                long len = await mongo.DbGetCollection<LogFormat>().Find(s => true).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<LogFormat>().Find(s => true).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
            else
            {
                long len = await mongo.DbGetCollection<LogFormat>().Find(s => s.Message.Contains(keywords)).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<LogFormat>().Find(s => s.Message.Contains(keywords)).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
        }
        [HttpGet]
        [Route("GetAllInfoLogs")]
        public async Task<Result> GetAllInfoLogs(int pagesize, int pagenumber, string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new InvalidApiRequestException("参数keywords为空，无法解析！");
            }
            if (keywords == "#")
            {
                long len = await mongo.DbGetCollection<LogFormat>().Find(s => s.Type != OprateType.Login).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<LogFormat>().Find(s => s.Type != OprateType.Login).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
            else
            {
                long len = await mongo.DbGetCollection<LogFormat>().Find(s => s.Message.Contains(keywords) && s.Type != OprateType.Login).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<LogFormat>().Find(s => s.Message.Contains(keywords) && s.Type != OprateType.Login).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
        }

        [HttpGet]
        [Route("GetAllLoginLogs")]
        public async Task<Result> GetAllLoginLogs(int pagesize, int pagenumber, string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new InvalidApiRequestException("参数keywords为空，无法解析！");
            }
            if (keywords == "#")
            {
                long len = await mongo.DbGetCollection<LogFormat>().Find(s => s.Type == OprateType.Login).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<LogFormat>().Find(s => s.Type == OprateType.Login).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
            else
            {
                long len = await mongo.DbGetCollection<LogFormat>().Find(s => s.Message.Contains(keywords) && s.Type == OprateType.Login).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<LogFormat>().Find(s => s.Message.Contains(keywords) && s.Type == OprateType.Login).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
        }


        [HttpDelete("DeleteLogs")]
        public async Task<Result> DeleteLogs(List<LogFormat> logs)
        {
            if (logs.Count == 0)
            {
                throw new InvalidApiRequestException("参数logs为空，无法解析！");
            }
            var filter = Builders<LogFormat>.Filter.Or(logs.Select(lo => Builders<LogFormat>.Filter.Eq(s => s.Id, lo.Id)).ToList());
            var res = await mongo.DbGetCollection<LogFormat>().DeleteManyAsync(filter);
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
