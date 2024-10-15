using System.Text.Json;
using DiscordChatExporter.Core.Discord.Data.Common;
using DiscordChatExporter.Core.Utils.Extensions;
using JsonExtensions.Reading;

namespace DiscordChatExporter.Core.Discord.Data;

public record PollOption(
    int Id,
    string Content,
    int VotesCount,
    int TotalVotes,
    double Ratio,
    Emoji? Emoji
)
{
    public static PollOption Parse(JsonElement json, int count, int totalVotes)
    {
        var id = json.GetProperty("answer_id").GetInt32();
        var question = json.GetProperty("poll_media").GetProperty("text").GetString() ?? string.Empty;
        var ratio = totalVotes == 0 ? 0 : (double)count / totalVotes;
        var emoji = json.GetProperty("poll_media").GetPropertyOrNull("emoji")?.Pipe(Emoji.Parse);
        return new PollOption(id, question, count, totalVotes, ratio, emoji);
    }
}