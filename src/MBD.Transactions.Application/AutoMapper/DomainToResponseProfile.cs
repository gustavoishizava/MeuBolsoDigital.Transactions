using AutoMapper;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Entities;

namespace MBD.Transactions.Application.AutoMapper
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Category, CategoryResponse>(MemberList.Destination);
            CreateMap<Category, CategoryWithSubCategoriesResponse>(MemberList.Destination);
        }
    }
}