using System.Linq;

namespace Infrastructure.Persistence.Naming;

public sealed class SnakeCaseTableNameResolver : ITableNameResolver
{
	private static readonly Dictionary<string, string> ExplicitNames = new(StringComparer.OrdinalIgnoreCase)
	{
		{ "AppUser", "app_users" },
		{ "Category", "categories" },
		{ "StoreCategory", "store_categories" },
		{ "OrderGroup", "order_groups" },
		{ "OrderItem", "order_items" },
		{ "PaymentSplit", "payment_splits" },
		{ "ProductVariant", "product_variants" },
		{ "LedgerTransaction", "ledger_transactions" },
		{ "LedgerPosting", "ledger_postings" },
		{ "WebhookDelivery", "webhook_deliveries" },
		{ "OutboxMessage", "outbox_messages" },
		{ "StoreApplication", "store_applications" }
	};

	public string ResolveTableName(Type entityType)
	{
		if (ExplicitNames.TryGetValue(entityType.Name, out var name))
			return name;

		return ToSnakeCasePlural(entityType.Name);
	}

	private static string ToSnakeCasePlural(string name)
	{
		var snake = string.Concat(name.Select((ch, i) => i > 0 && char.IsUpper(ch)
			? "_" + char.ToLowerInvariant(ch)
			: char.ToLowerInvariant(ch).ToString())).Trim('_');
		return snake.EndsWith("s") ? snake : snake + "s";
	}
}
