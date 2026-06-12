using AIOutfitStylist.Domain.Common;

namespace AIOutfitStylist.Domain.Entities;

public sealed class UserPhoto : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string BlobName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public PhotoAnalysis? Analysis { get; set; }
}
