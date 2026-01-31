namespace erat_mobile_tool.Models
{
    public class RunSettings
    {
        public int TargetPoints { get; set; } = 10000000; // 目標ポイント
        public int CurrentPoints { get; set; } = 0;
        public int PointsPerPlay { get; set; } = 25000;  // 1回あたりの獲得量
        public int LiveBonusConsume { get; set; } = 3;   // 炊き数

        // 残り必要ポイントの計算
        public int RemainingPoints => Math.Max(0, TargetPoints - CurrentPoints);

        // 必要プレイ回数の計算
        public int RequiredPlays => (int)Math.Ceiling((double)RemainingPoints / PointsPerPlay);
    }
}
