# アイデアノート

## 実装すること
- グラフ
  - ポイントグラフ(ポイント数推移+当日目標数)
  - 順位グラフ(順位推移+目標順位)
- 指標
  - 残り時間(秒単位)
  - 総ポイント数
  - 全体速度
  - 順位
  - 当日ポイント数
  - 当日速度
- ポイント変更
  - 原則画像認識(データ累積計算)
  - ポイント修正機能(適宜)

## data.csv 書式
 | time | type | value | sum | comment |
 ----|----|----|----|----
 | 日付と時間 | 1…加算、2…修正 | 数値 | 総数 | コメント(通常使用しない) |