namespace SlackReminder;

public static class SlackChannelReminderEndpoint
{
    public static IEndpointRouteBuilder MapSlackChannelReminderEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/slack-reminder");

        group.MapPost("/send", async (ISlackReminderService slackReminderService,
            CancellationToken cancellationToken) =>
        {
            await slackReminderService.SendDailyReminderAsync(cancellationToken);

            return Results.Ok(new
            {
                Success = true,
                Message = "Daily reminder sent successfully."
            });
        });

        return group;
    }
}