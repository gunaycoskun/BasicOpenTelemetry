namespace BasicOpenTelemetry.Data;

public sealed record StudentUpdateDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int Age { get; init; }
}