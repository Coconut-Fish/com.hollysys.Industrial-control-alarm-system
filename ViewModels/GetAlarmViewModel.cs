namespace Server.ViewModels
{
    public class GetAlarmViewModel
    {
        // 报警发生时间
        public DateTime? Start_AlarmTime { get; set; }

        public DateTime? End_AlarmTime { get; set; }

        // 报警源 (字符串)
        public string? Source { get; set; }

        // 报警类型
        public string? Type { get; set; }

        //// 报警确认时间
        //public DateTime? ConfirmTime { get; set; }

        //// 报警恢复时间
        //public DateTime? RecoverTime { get; set; }

        // 报警确认状态
        public bool? IsConfirmed { get; set; }

        // 报警恢复状态
        public bool? IsRecovered { get; set; }

        // 报警级别
        public int? Level { get; set; }

    }
}
