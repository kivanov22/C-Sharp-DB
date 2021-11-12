namespace FastFood.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using FastFood.Data;
    using FastFood.Models;
    using FastFood.Services.DTO.Category;
    using FastFood.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class CategoryService : ICategoryService
    {

        private readonly FastFoodContext dbContext;
        private readonly IMapper mapper;

        public CategoryService(FastFoodContext dbContext,IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public void Create(CreateCategoryDto dto)
        {
            Category category = this.mapper.Map<Category>(dto);

            this.dbContext.Categories.Add(category);
            this.dbContext.SaveChanges();
        }

        public ICollection<ListAllCategoriesDto> All()
        => this.dbContext.Categories.ProjectTo<ListAllCategoriesDto>(this.mapper.ConfigurationProvider).ToList();
    }
}


