namespace AIOutfitStylist.Infrastructure.Options;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string ChatModel { get; set; } = "gpt-4.1-mini";
    public string VisionModel { get; set; } = "gpt-4.1-mini";
    public string ImageModel { get; set; } = "gpt-image-1";
}
