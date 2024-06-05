namespace BasicOpenTelemetry.Data;

public sealed record StudentCreateDto
{
    public required string Name { get; init; }
    public required int Age { get; init; }
}