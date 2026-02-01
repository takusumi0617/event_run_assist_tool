using System.Text.Json;
using Microsoft.JSInterop;
using erat_mobile_tool.Models;

namespace erat_mobile_tool.Services;

public class AppState
{
    private readonly IJSRuntime _js;
    private const string ConfigKey = "erat_event_config";
    private const string LogsKey = "erat_logs";
    private const string PresetsKey = "erat_target_presets";

    public EventConfig Config { get; set; } = new();
    public List<PointLog> Logs { get; set; } = new();
    public Dictionary<string, Dictionary<string, long>> Presets { get; set; } = new();
    public AppState(IJSRuntime js) => _js = js;

    public async Task LoadConfigAsync()
    {
        // 既存のロード処理
        var jsonConfig = await _js.InvokeAsync<string>("localStorage.getItem", ConfigKey);
        if (!string.IsNullOrEmpty(jsonConfig)) Config = JsonSerializer.Deserialize<EventConfig>(jsonConfig) ?? new();

        var jsonLogs = await _js.InvokeAsync<string>("localStorage.getItem", LogsKey);
        if (!string.IsNullOrEmpty(jsonLogs)) Logs = JsonSerializer.Deserialize<List<PointLog>>(jsonLogs) ?? new();

        // 目安データのロード
        var jsonPresets = await _js.InvokeAsync<string>("localStorage.getItem", PresetsKey);
        if (!string.IsNullOrEmpty(jsonPresets))
        {
            Presets = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, long>>>(jsonPresets) ?? new();
        }
        else
        {
            InitializeDefaultPresets(); // データがなければ初期値をセット
        }
    }

    public async Task SaveConfigAsync()
    {
        var json = JsonSerializer.Serialize(Config);
        await _js.InvokeVoidAsync("localStorage.setItem", ConfigKey, json);
    }

    public async Task SaveLogsAsync()
    {
        var json = JsonSerializer.Serialize(Logs);
        await _js.InvokeVoidAsync("localStorage.setItem", LogsKey, json);
    }

    public async Task SavePresetsAsync()
    {
        var json = JsonSerializer.Serialize(Presets);
        await _js.InvokeVoidAsync("localStorage.setItem", PresetsKey, json);
    }

    private void InitializeDefaultPresets()
    {
        string[] units = { "Leo/need", "MORE MORE JUMP！", "Vivid BAD SQUAD", "ワンダーランズ×ショウタイム", "25時、ナイトコードで。", "(混合)" };
        string[] ranks = { "1位", "50位", "100位", "200位(精度低)", "500位(精度低)", "1000位" };

        foreach (var u in units) Presets[u] = new Dictionary<string, long>();

        // Leo/need
        Presets["Leo/need"]["1位"] = 319000000; Presets["Leo/need"]["50位"] = 117510000; Presets["Leo/need"]["100位"] = 88880000;
        Presets["Leo/need"]["200位(精度低)"] = 38100000; Presets["Leo/need"]["500位(精度低)"] = 23280000; Presets["Leo/need"]["1000位"] = 17390000;

        // MORE MORE JUMP！
        Presets["MORE MORE JUMP！"]["1位"] = 315000000; Presets["MORE MORE JUMP！"]["50位"] = 103850000; Presets["MORE MORE JUMP！"]["100位"] = 78040000;
        Presets["MORE MORE JUMP！"]["200位(精度低)"] = 39000000; Presets["MORE MORE JUMP！"]["500位(精度低)"] = 27320000; Presets["MORE MORE JUMP！"]["1000位"] = 19410000;

        // Vivid BAD SQUAD
        Presets["Vivid BAD SQUAD"]["1位"] = 350000000; Presets["Vivid BAD SQUAD"]["50位"] = 150450000; Presets["Vivid BAD SQUAD"]["100位"] = 101110000;
        Presets["Vivid BAD SQUAD"]["200位(精度低)"] = 54190000; Presets["Vivid BAD SQUAD"]["500位(精度低)"] = 35110000; Presets["Vivid BAD SQUAD"]["1000位"] = 24680000;

        // ワンダーランズ×ショウタイム
        Presets["ワンダーランズ×ショウタイム"]["1位"] = 340000000; Presets["ワンダーランズ×ショウタイム"]["50位"] = 171540000; Presets["ワンダーランズ×ショウタイム"]["100位"] = 139450000;
        Presets["ワンダーランズ×ショウタイム"]["200位(精度低)"] = 60780000; Presets["ワンダーランズ×ショウタイム"]["500位(精度低)"] = 38620000; Presets["ワンダーランズ×ショウタイム"]["1000位"] = 29710000;

        // 25時、ナイトコードで。
        Presets["25時、ナイトコードで。"]["1位"] = 382000000; Presets["25時、ナイトコードで。"]["50位"] = 172520000; Presets["25時、ナイトコードで。"]["100位"] = 145950000;
        Presets["25時、ナイトコードで。"]["200位(精度低)"] = 57770000; Presets["25時、ナイトコードで。"]["500位(精度低)"] = 42060000; Presets["25時、ナイトコードで。"]["1000位"] = 36380000;

        // (混合)
        Presets["(混合)"]["1位"] = 309390000; Presets["(混合)"]["50位"] = 135170000; Presets["(混合)"]["100位"] = 90610000;
        Presets["(混合)"]["200位(精度低)"] = 58680000; Presets["(混合)"]["500位(精度低)"] = 40880000; Presets["(混合)"]["1000位"] = 30840000;
    }

    // インポート時の一括保存用
    public async Task SaveAllAsync()
    {
        await SaveConfigAsync();
        await SaveLogsAsync();
        await SavePresetsAsync();
    }
}