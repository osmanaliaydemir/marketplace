using System.Linq;

namespace Infrastructure.Persistence.Naming;

public sealed class SnakeCaseColumnNameResolver : IColumnNameResolver
{
	public string ResolveColumnName(string propertyName)
	{
		return ToSnakeCase(propertyName);
	}

	public string ResolveIdColumnName(Type entityType) => "id";

	private static string ToSnakeCase(string name)
	{
		return string.Concat(name.Select((ch, i) => i > 0 && char.IsUpper(ch)
			? "_" + char.ToLowerInvariant(ch)
			: char.ToLowerInvariant(ch).ToString()));
	}
}
