using MBD.Transactions.Domain.Entities;

namespace MBD.Transactions.Application.Response.Models
{
    public class CategoryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public CategoryModel(Category category)
        {
            Id = category.Id.ToString();
            Name = category.Name;
        }
    }
}