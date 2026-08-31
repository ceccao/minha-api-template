using System.ComponentModel.DataAnnotations;

namespace MinhaApi.Api.Options;

public class MySqlOptions
{
    [Range(1, 300)]
    public int CommandTimeout { get; init; } = 30;
}
