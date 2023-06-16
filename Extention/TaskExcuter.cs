using FluentScheduler;

namespace MQTT_Api_Server_Lifesaver.Extention
{
    public static class TaskExcuter
    {
        public static void TaskInit(this IServiceCollection service)
        {
            JobManager.Initialize();
        }
        public static void AddTask(Action job, Action<Schedule> schedule)
        {
            LogHelper.LogInfo(IdGenFunc.CreateOneId(),$"正常日志任务成功：将于每一分钟执行一次日志记录 【{APPTime.GetAPPTime()}】");
            JobManager.AddJob(job, schedule);
        }
        public static void DeleteTask(string taskname)
        {
            if (string.IsNullOrWhiteSpace(taskname))
            {
                LogHelper.LogInfo(IdGenFunc.CreateOneId(),$"删除任务【{taskname}】失败，原因：任务不存在 【{APPTime.GetAPPTime()}】");
                return;
            }
            JobManager.RemoveJob(taskname);
        }
        public static IEnumerable<Schedule> GetAllTasks()
        {
           return JobManager.AllSchedules;
        }
        public static IEnumerable<Schedule> GetAllRuningTasks()
        {
            return JobManager.RunningSchedules;
        }
        public static void AddOnStartMonitor(Action<JobStartInfo> action)
        {
            JobManager.JobStart += action;
        }
        public static void AddOnEndMonitor(Action<JobEndInfo> action)
        {
            JobManager.JobEnd += action;
        }

    }
}
