using MQTT_Api_Server_Lifesaver.DB;
using System;
using System.Collections.Concurrent;

namespace MQTT_Api_Server_Lifesaver.Log
{
    public class LogFormat
    {
        public LogFormat()
        {
            this.Time = APPTime.GetAPPTime();
        }
        public long Id { get; set; }
        public long OpratorId { get; set; }
        public string? Message { get; set; }
        public OprateType? Type { get; set; } = OprateType.Other;
        public DateTime? Time { get; set; }

    }

    public enum OprateType
    {
        Delete = 0,
        Update = 1,
        Insert = 2,
        Login = 3,
        Other = 4,
    }
    public static class LogHelper
    {
        public static void Log(long Uid, string msg, OprateType oprateType)
        {
            logqueue.Add(new LogFormat()
            {
                Id = IdGenFunc.CreateOneId(),
                Message = msg,
                OpratorId = Uid,
                Time = DateTime.Now,
                Type = oprateType,
            });
            if (logqueue.Count >= logBufferSize || DateTime.Now - _lastFlushTime >= FlushInterval)
            {
                LogToDb();
                _lastFlushTime = DateTime.Now; // 更新上一次写入时间
            }
        }
        public static void LogDelete(long Uid, string msg)
        {
            Log(Uid, msg, OprateType.Delete);
        }

        public static void LogInsert(long Uid, string msg)
        {
            Log(Uid, msg, OprateType.Insert);
        }
        public static void LogInfo(long Uid, string msg)
        {
            Log(Uid, msg, OprateType.Other);
        }
        /// <summary>
        /// 登录信息记录
        /// </summary>
        /// <param name="Uid"></param>
        /// <param name="msg"></param>
        public static void LogLogin(long Uid, string msg)
        {
            Log(Uid, msg, OprateType.Login);
        }
        public static void LogUpdate(long Uid, string msg)
        {
            Log(Uid, msg, OprateType.Update);
        }
        public static async void LogToDb()
        {
            if (logqueue.Count == 0)
            {
                return;
            }
            await MongoDBHelper.Db!.DbGetCollection<LogFormat>().InsertManyAsync(logqueue);
            logqueue.Clear();
        }
        public static List<LogFormat> logqueue = new List<LogFormat>();
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(30);
        private static readonly int logBufferSize = 100;
        private static DateTime _lastFlushTime = DateTime.Now;

    }

}
