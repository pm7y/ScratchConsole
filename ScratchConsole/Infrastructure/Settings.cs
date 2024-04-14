namespace ScratchConsole.Infrastructure;

public class Settings
{
    public const string SectionName = nameof(Settings);
    public required int NumberSetting { get; init; }
    public required string StringSetting { get; init; }
}