namespace Infrastructure.Persistence.Naming;

public interface ITableNameResolver
{
	string ResolveTableName(Type entityType);
	string ResolveIdColumnName(Type entityType) => "id";
}
