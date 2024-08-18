namespace Server.ViewModels
{
    public class AddAlarmViewModel
    {
        // 报警源 (字符串)
        public string Source { get; set; } = string.Empty;

        // 报警类型
        public string Type { get; set; } = string.Empty;

        // 报警级别
        public int Level { get; set; }

        // 与报警相关的其他信息
        public string AdditionalInfo { get; set; } = string.Empty;
    }
}
