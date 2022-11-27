using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.Transactions.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.Transactions.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class CategoryMapping : BsonClassMapConfiguration
    {
        public CategoryMapping() : base("categories")
        {
        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<Category>();

            map.MapProperty(x => x.TenantId)
                    .SetElementName("tenant_id");

            map.MapProperty(x => x.ParentCategoryId)
                .SetElementName("parent_category_id");

            map.MapProperty(x => x.Name)
                .SetElementName("name");

            map.MapProperty(x => x.Type)
                .SetElementName("type");

            map.MapProperty(x => x.Status)
                .SetElementName("status");

            map.MapField("_subCategories")
                .SetShouldSerializeMethod(x => false);

            return map;
        }
    }
}