namespace Infrastructure.Persistence.Naming;

public interface IColumnNameResolver
{
	string ResolveColumnName(string propertyName);
	string ResolveIdColumnName(Type entityType) => "id";
}
