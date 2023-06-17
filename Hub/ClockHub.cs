using Microsoft.AspNetCore.SignalR;
using System;

namespace MQTT_Api_Server_Lifesaver.Hub
{
    public class ClockHub : Hub<IClock>
    {
        public void SendTimeToClients(DateTime dateTime)
        {
            Worker.hubEvents.Enqueue(new HubQueueData() { HubEvent = HubEvent.TimeCheck, Data = dateTime });
        }
        public void SendDeviceUpdateToClients(string Id)
        {
            Worker.hubEvents.Enqueue(new HubQueueData() { HubEvent = HubEvent.DeviceUpdate, Data = Id });
        }
        public void MqttMsgUpdate()
        {
            Worker.hubEvents.Enqueue(new HubQueueData() { HubEvent = HubEvent.MqttMsgUpdate, Data = "" });
        }
    }

    public interface IClock
    {
        Task DeviceOnline(string Id);
        Task MqttMsgUpdate();
        Task ShowTime(DateTime currentTime);
    }
    public class Worker : BackgroundService
    {

        private readonly IHubContext<ClockHub, IClock> _clockHub;

        public static ConcurrentQueue<HubQueueData> hubEvents { get; set; } = new ConcurrentQueue<HubQueueData>();

        public Worker(IHubContext<ClockHub, IClock> clockHub)
        {
            _clockHub = clockHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HubQueueData hubQueueData = new HubQueueData();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (hubEvents.TryDequeue(out hubQueueData!))
                {
                    switch (hubQueueData.HubEvent)
                    {
                        case HubEvent.TimeCheck:
                            await _clockHub.Clients.All.ShowTime((DateTime)hubQueueData.Data);
                            break;
                        case HubEvent.DeviceUpdate:
                            await _clockHub.Clients.All.DeviceOnline((string)hubQueueData.Data);
                            break;
                        case HubEvent.MqttMsgUpdate:
                            await _clockHub.Clients.All.MqttMsgUpdate();
                            break;
                        default:
                            break;
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        public static void SendTimeToClients(DateTime dateTime)
        {
            hubEvents.Enqueue(new HubQueueData() { HubEvent = HubEvent.TimeCheck, Data = dateTime });
        }
        public static void SendDeviceUpdateToClients(string Id)
        {
            hubEvents.Enqueue(new HubQueueData() { HubEvent = HubEvent.DeviceUpdate, Data = Id });
        }
        public static void MqttMsgUpdate()
        {
            hubEvents.Enqueue(new HubQueueData() { HubEvent = HubEvent.MqttMsgUpdate, Data = "" });
        }
    }
    public enum HubEvent
    {
        TimeCheck = 0,
        DeviceUpdate = 1,
        MqttMsgUpdate = 2,
    }
    public class HubQueueData
    {
        public HubEvent HubEvent { get; set; }
        public object Data { get; set; }

    }
}
