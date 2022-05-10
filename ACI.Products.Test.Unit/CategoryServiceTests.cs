using System.Threading.Tasks;
using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Domain;
using ACI.Products.Domain.Category;
using ACI.Products.Models;
using ACI.Products.Models.DTO;
using FluentAssertions;
using LanguageExt;
using LanguageExt.UnitTesting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ACI.Products.Test.Unit;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;

    private readonly ICategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_mockCategoryRepo.Object, Mock.Of<ILogger<CategoryService>>());
    }

    [Fact]
    public async Task Creating_NewUniqueCategory_Succeeds()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Audio-equipment" };

        _mockCategoryRepo
            .Setup(s => s.AddCategory(request.Name))
            .ReturnsAsync(new ProductCategory { Name = request.Name, Id = 1 });

        // Act
        var result = await _categoryService.CreateCategory(request);

        // Assert
        result.ShouldBeRight(r =>
        {
            r.Name.Should().Be(request.Name);
            r.Id.Should().Be(1);
        });
    }

    [Fact]
    public async Task Creating_DuplicateCategory_FailsWithError()
    {
        // Arrange
        var existingCategory = new ProductCategory { Id = 1, Name = "Audio-equipment" };
        var request = new CreateCategoryRequest { Name = existingCategory.Name };

        _mockCategoryRepo
            .Setup(s => s.GetCategoryByName(existingCategory.Name))
            .ReturnsAsync(existingCategory);

        // Act
        var result = await _categoryService.CreateCategory(request);

        // Assert
        result.ShouldBeLeft(err =>
        {
            err.Should().Be(AppErrors.CategoryNameAlreadyExistsError);
        });
    }

    [Fact]
    public async Task Get_ExistingCategoryById_Succeeds()
    {
        // Arrange
        var existingCategory = new ProductCategory { Id = 1, Name = "Audio-equipment" };

        _mockCategoryRepo.Setup(s => s.GetCategory(existingCategory.Id))
            .ReturnsAsync(existingCategory);

        // Act
        var result = await _categoryService.GetCategory(existingCategory.Id);

        // Assert
        result.ShouldBeSome(s =>
        {
            s.Name.Should().Be(existingCategory.Name);
            s.Id.Should().Be(existingCategory.Id);
        });
    }

    [Fact]
    public async Task Get_CategoryByNonExistentId_FailsWithError()
    {
        // this test should still function without the Arrange part, since Moq Setups don't transfer between methods.
        // However, it's good to reset the behaviour anyway

        // Arrange
        _mockCategoryRepo.Setup(s => s.GetCategory(It.IsAny<int>()))
            .ReturnsAsync(Option<ProductCategory>.None);

        // Act
        var result = await _categoryService.GetCategory(1);

        // Assert
        result.ShouldBeNone();
    }
}