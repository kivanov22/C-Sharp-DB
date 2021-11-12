namespace FastFood.Services.Interfaces
{
    using FastFood.Services.DTO.Category;
    using System.Collections.Generic;

    public interface ICategoryService
    {
        void Create(CreateCategoryDto dto);

        ICollection<ListAllCategoriesDto> All();
    }
}
