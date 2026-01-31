using System.Text.Json;
using Microsoft.JSInterop;
using erat_mobile_tool.Models;

namespace erat_mobile_tool.Services;

public class AppState
{
    private readonly IJSRuntime _js;
    private const string ConfigKey = "erat_event_config";
    private const string LogsKey = "erat_logs";

    public EventConfig Config { get; set; } = new();
    public List<PointLog> Logs { get; set; } = new();

    public AppState(IJSRuntime js) => _js = js;

    public async Task LoadConfigAsync()
    {
        var jsonConfig = await _js.InvokeAsync<string>("localStorage.getItem", ConfigKey);
        if (!string.IsNullOrEmpty(jsonConfig))
            Config = JsonSerializer.Deserialize<EventConfig>(jsonConfig) ?? new();

        var jsonLogs = await _js.InvokeAsync<string>("localStorage.getItem", LogsKey);
        if (!string.IsNullOrEmpty(jsonLogs))
            Logs = JsonSerializer.Deserialize<List<PointLog>>(jsonLogs) ?? new();
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

    // インポート時の一括保存用
    public async Task SaveAllAsync()
    {
        await SaveConfigAsync();
        await SaveLogsAsync();
    }
}