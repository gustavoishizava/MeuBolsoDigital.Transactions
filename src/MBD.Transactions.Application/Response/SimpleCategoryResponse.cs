using System;

namespace MBD.Transactions.Application.Response
{
    public class SimpleCategoryResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }

        public SimpleCategoryResponse(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}