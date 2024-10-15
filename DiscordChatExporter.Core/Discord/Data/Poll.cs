using System;
using System.Text.Json;
using System.Collections.Generic;
using DiscordChatExporter.Core.Discord.Data.Common;
using DiscordChatExporter.Core.Utils.Extensions;
using JsonExtensions.Reading;

namespace DiscordChatExporter.Core.Discord.Data;

// https://discord.com/developers/docs/resources/poll#poll-object-poll-object-structure
public record Poll(
    string Content,
    DateTimeOffset Expiry,
    IReadOnlyList<PollOption> Options,
    int TotalVotes
)
{
    public static Poll Parse(JsonElement json)
    {
        var question = json.GetProperty("question").GetProperty("text").GetString() ?? string.Empty;
        var expiryTimestamp = json.GetProperty("expiry").GetDateTimeOffset();
        var options = new List<PollOption>();
        var totalVotes = 0;
        foreach (var optionCount in json.GetProperty("results").GetProperty("answer_counts").EnumerateArray())
        {
            totalVotes += optionCount.GetProperty("count").GetInt32();
        }
        foreach (var option in json.GetProperty("answers").EnumerateArray())
        {
            var count = 0;
            var optionId = option.GetProperty("answer_id").GetInt32();
            foreach (var optionCount in json.GetProperty("results").GetProperty("answer_counts").EnumerateArray())
            {
                if (optionCount.GetProperty("id").GetInt32() == optionId)
                {
                    count = optionCount.GetProperty("count").GetInt32();
                    break;
                }
            }
            options.Add(PollOption.Parse(option, count, totalVotes));
        }
        return new Poll(question, expiryTimestamp, options, totalVotes);
    }
}
