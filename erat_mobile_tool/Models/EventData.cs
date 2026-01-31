namespace erat_mobile_tool.Models
{
    public class EventConfig
    {
        public string EventName { get; set; } = "イベント名未設定";
        public long GoalPoints { get; set; } = 10000000;
        public DateTime StartTime { get; set; } = DateTime.Today.AddHours(15);
        public DateTime EndTime { get; set; } = DateTime.Today.AddDays(9).AddHours(21);
    }

    public class PointLog
    {
        public DateTime Timestamp { get; set; }
        public long Value { get; set; }
    }
}
