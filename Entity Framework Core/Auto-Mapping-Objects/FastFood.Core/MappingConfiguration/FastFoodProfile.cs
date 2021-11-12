namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Categories;
    using FastFood.Models;
    using FastFood.Services.DTO.Category;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));


            //Categories
            this.CreateMap<CreateCategoryInputModel, CreateCategoryDto>();

            this.CreateMap<ListAllCategoriesDto, CategoryAllViewModel>()
                .ForMember(x=>x.Name, y=>y.MapFrom(s=>s.CategoryName));


            this.CreateMap<CreateCategoryDto, Category>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.CategoryName));

            this.CreateMap<Category, ListAllCategoriesDto>()
                .ForMember(x => x.CategoryName, y => y.MapFrom(s => s.Name));
        }
    }
}
