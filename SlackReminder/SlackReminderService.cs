using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SlackReminder;

public class SlackReminderService : ISlackReminderService
{
    private readonly ILogger<SlackReminderService> _logger;
    private readonly HttpClient _httpClient;
    private readonly SlackOptions _options;

    public SlackReminderService(
        ILogger<SlackReminderService> logger,
        IOptions<SlackOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.BotToken);
    }

    public async Task SendDailyReminderAsync(
        CancellationToken cancellationToken = default)
    {
        // TODO:
        // Get reminder data from database
        await SendMessageAsync(_options.SlackAPIUrl, _options.ChannelId, _options.MessageTemplate, cancellationToken);

        _logger.LogInformation("Sending daily Slack reminder...");

        await Task.Delay(1000, cancellationToken);

        _logger.LogInformation("Daily reminder sent.");
    }

    public async Task SendMessageAsync(
        string slackApiUrl,
        string channel,
        string message,
        CancellationToken cancellationToken = default)
    {
        // TODO:
        // Call Slack API
        var payload = new
        {
            channel = channel,
            text = message
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(slackApiUrl, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode || !responseBody.Contains("\"ok\":true"))
        {
            Console.Write($"Slack API error: {responseBody}");
        }

        _logger.LogInformation(
            "Sending Slack message to {Channel}: {Message}",
            channel,
            message);

        await Task.Delay(500, cancellationToken);
    }
}