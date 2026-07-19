namespace SlackReminder;

public interface ISlackReminderService
{
    Task SendDailyReminderAsync(CancellationToken cancellationToken = default);

    Task SendMessageAsync(
        string slackApiUrl,
        string channel,
        string message,
        CancellationToken cancellationToken = default);    
}