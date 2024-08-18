namespace Server.Models
{
    public class Alarm
    {
        public int Id { get; set; }
        // 报警发生时间 (分辨率为1毫秒)
        public DateTime AlarmTime { get; set; }

        // 报警源 (字符串)
        public string Source { get; set; } = string.Empty;

        // 报警类型
        public string Type { get; set; } = string.Empty;

        // 报警确认时间
        public DateTime? ConfirmTime { get; set; }

        // 报警恢复时间
        public DateTime? RecoverTime { get; set; }

        // 报警确认状态
        public bool IsConfirmed { get; set; }

        // 报警恢复状态
        public bool IsRecovered { get; set; }

        // 报警级别
        public int Level { get; set; }

        // 与报警相关的其他信息
        public string AdditionalInfo { get; set; } = string.Empty;
    }
}
