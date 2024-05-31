using CatalogServices.DAL;
using CatalogServices.DAL.Interfaces;
using CatalogServices.DTO.Category;
using CatalogServices.DTO.CatProd;
using CatalogServices.DTO.Product;
using CatalogServices.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "DESAIN ARSITEKTUR MICROSERVICES - Catalog Services",
        Description = "Oleh 72210456 - Nikolaus Pastika Bara Satyaradi",
    });
});

builder.Services.AddScoped<ICategory, CategoryDapper>();
builder.Services.AddScoped<IProduct, ProductDapper>();
builder.Services.AddScoped<ICatProd, CatProdDapper>();

var app = builder.Build();

// MapGrouping
var categoryServices = app.MapGroup("/catalogservices/api/category").WithTags("Category Service API");
var productServices = app.MapGroup("/catalogservices/api/product").WithTags("Product Services API");
var categoryProductServices = app.MapGroup("/catalogservices/api/catprod").WithTags("Category Product Services API");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Category Start
categoryServices.MapGet("", (ICategory categoryDal) =>
{
    List<CategoryDTO> categoriesDto = new List<CategoryDTO>();
    var categories = categoryDal.GetAll();
    foreach (var category in categories)
    {
        categoriesDto.Add(new CategoryDTO
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
        });
    }
    return Results.Ok(categoriesDto);
});

categoryServices.MapGet("/id/{id}", (ICategory categoryDal, int id) =>
{
    CategoryDTO categoryDto = new CategoryDTO();
    var categories = categoryDal.GetByID(id);
    if (categories == null)
    {
        return Results.NotFound();
    }
    categoryDto.CategoryName = categories.CategoryName;
    categoryDto.CategoryID = categories.CategoryID;

    return Results.Ok(categoryDto);
});

categoryServices.MapGet("/name/{name}", (ICategory categoryDal, string name) =>
{
    List<CategoryDTO> categoriesDto = new List<CategoryDTO>();
    var categories = categoryDal.GetByName(name);
    if (categories == null)
    {
        return Results.NotFound();
    }
    foreach (var category in categories)
    {
        categoriesDto.Add(new CategoryDTO
        {
            CategoryName = category.CategoryName,
            CategoryID = category.CategoryID,
        });
    }
    return Results.Ok(categoriesDto);
});

categoryServices.MapPost("/categories", (ICategory categoryDAL, CategoryCreateDTO category) =>
{
    try
    {
        Category categoryDto = new Category
        {
            CategoryName = category.CategoryName
        };

        categoryDAL.Insert(categoryDto);

        // Return 201 Created
        return Results.Created($"/api/categories/{category}", category);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

categoryServices.MapDelete("/categories/{id}", (ICategory categoryDAL, int id) =>
{
    try
    {
        categoryDAL.Delete(id);

        // Return 201 Created
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

categoryServices.MapPut("/categories", (ICategory categoryDAL, CategoryUpdateDTO category) =>
{
    try
    {
        Category categoryDto = new Category
        {
            CategoryName = category.CategoryName,
            CategoryID = category.CategoryID,
        };

        categoryDAL.Update(categoryDto);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
// Category End

// Product Start
productServices.MapGet("", (IProduct productDal) =>
{
    List<ProductDTO> productsDto = new List<ProductDTO>();
    var products = productDal.GetAll();
    foreach (var product in products)
    {
        productsDto.Add(new ProductDTO
        {
            ProductID = product.ProductID,
            CategoryID = product.CategoryID,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
        });
    }
    return Results.Ok(productsDto);
});

productServices.MapGet("/id/{id}", (IProduct productDal, int id) =>
{
    ProductDTO productDto = new ProductDTO();
    var product = productDal.GetByID(id);
    if (product == null)
    {
        return Results.NotFound();
    }

    productDto.ProductID = product.ProductID;
    productDto.CategoryID = product.CategoryID;
    productDto.Name = product.Name;
    productDto.Description = product.Description;
    productDto.Price = product.Price;
    productDto.Quantity = product.Quantity;

    return Results.Ok(productDto);
});

productServices.MapGet("/categoryid/{id}", (IProduct productDal, int id) =>
{
    ProductDTO productDto = new ProductDTO();
    var product = productDal.GetByCategoryID(id);
    if (product == null)
    {
        return Results.NotFound();
    }

    productDto.ProductID = product.ProductID;
    productDto.CategoryID = product.CategoryID;
    productDto.Name = product.Name;
    productDto.Description = product.Description;
    productDto.Price = product.Price;
    productDto.Quantity = product.Quantity;

    return Results.Ok(productDto);
});

productServices.MapGet("/name/{name}", (IProduct productDal, string name) =>
{
    List<ProductDTO> productsDto = new List<ProductDTO>();
    var products = productDal.GetByName(name);
    if (products == null)
    {
        return Results.NotFound();
    }
    foreach (var product in products)
    {
        productsDto.Add(new ProductDTO
        {
            ProductID = product.ProductID,
            CategoryID = product.CategoryID,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
        });
    }
    return Results.Ok(productsDto);
});

productServices.MapGet("/description/{description}", (IProduct productDal, string description) =>
{
    List<ProductDTO> productsDto = new List<ProductDTO>();
    var products = productDal.GetByDescription(description);
    if (products == null)
    {
        return Results.NotFound();
    }
    foreach (var product in products)
    {
        productsDto.Add(new ProductDTO
        {
            ProductID = product.ProductID,
            CategoryID = product.CategoryID,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
        });
    }
    return Results.Ok(productsDto);
});

productServices.MapPost("", (IProduct productDAL, ProductCreateDTO product) =>
{
    try
    {
        Product productDto = new Product
        {
            CategoryID = product.CategoryID,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
        };

        productDAL.Insert(productDto);

        ProductDTO products = new ProductDTO
        {
            CategoryID = product.CategoryID,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
        };

        // Return 201 Created
        return Results.Created($"/catalogservice/api/products/{product}", product);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

productServices.MapDelete("/{id}", (IProduct productDAL, int id) =>
{
    try
    {
        productDAL.Delete(id);

        // Return 201 Created
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

productServices.MapPut("", (IProduct productDAL, ProductUpdateDTO product) =>
{
    try
    {
        Product productDto = new Product
        {
            ProductID = product.ProductID,
            CategoryID = product.CategoryID,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
        };

        productDAL.Update(productDto);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

productServices.MapPut("/updatequantity", (IProduct productDAL, ProductUpdateQuantityDTO product) =>
{
    try
    {
        ProductUpdateQuantityDTO productUpdateQuantityDTO = new ProductUpdateQuantityDTO
        {
            ProductID = product.ProductID,
            Quantity = product.Quantity,
        };

        productDAL.UpdateStockAfterOrder(productUpdateQuantityDTO);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
// Product End

// Category Product Start
categoryProductServices.MapGet("", (ICatProd catProdDal) =>
{
    List<CatProdDTO> catProdDto = new List<CatProdDTO>();
    var outputs = catProdDal.GetAll();
    foreach (var output in outputs)
    {
        catProdDto.Add(new CatProdDTO
        {
            ProductID = output.ProductID,
            CategoryID = output.CategoryID,
            CategoryName = output.CategoryName,
            Name = output.Name,
            Description = output.Description,
            Price = output.Price,
            Quantity = output.Quantity,
        });
    }
    return Results.Ok(catProdDto);
});

categoryProductServices.MapGet("/productid/{productid}", (ICatProd catProdDal, int productid) =>
{
    CatProdDTO catProdDto = new CatProdDTO();
    var output = catProdDal.GetByID(productid);
    if (output == null)
    {
        return Results.NotFound();
    }

    catProdDto.ProductID = output.ProductID;
    catProdDto.CategoryID = output.CategoryID;
    catProdDto.CategoryName = output.CategoryName;
    catProdDto.Name = output.Name;
    catProdDto.Description = output.Description;
    catProdDto.Price = output.Price;
    catProdDto.Quantity = output.Quantity;


    return Results.Ok(catProdDto);
});

categoryProductServices.MapGet("/categoryid/{categoryid}", (ICatProd catProdDal, int categoryid) =>
{
    CatProdDTO catProdDto = new CatProdDTO();
    var output = catProdDal.GetByCategoryID(categoryid);
    if (output == null)
    {
        return Results.NotFound();
    }

    catProdDto.ProductID = output.ProductID;
    catProdDto.CategoryID = output.CategoryID;
    catProdDto.CategoryName = output.CategoryName;
    catProdDto.Name = output.Name;
    catProdDto.Description = output.Description;
    catProdDto.Price = output.Price;
    catProdDto.Quantity = output.Quantity;

    return Results.Ok(catProdDto);
});

categoryProductServices.MapGet("/productname/{productname}", (ICatProd catProdDal, string productname) =>
{
    List<CatProdDTO> catProdDto = new List<CatProdDTO>();
    var outputs = catProdDal.GetByName(productname);
    if (outputs == null)
    {
        return Results.NotFound();
    }
    foreach (var output in outputs)
    {
        catProdDto.Add(new CatProdDTO
        {
            ProductID = output.ProductID,
            CategoryID = output.CategoryID,
            CategoryName = output.CategoryName,
            Name = output.Name,
            Description = output.Description,
            Price = output.Price,
            Quantity = output.Quantity,
        });
    }
    return Results.Ok(catProdDto);
});

categoryProductServices.MapGet("/categoryname/{categoryname}", (ICatProd catProdDal, string categoryname) =>
{
    List<CatProdDTO> catProdDto = new List<CatProdDTO>();
    var outputs = catProdDal.GetByCategoryName(categoryname);
    if (outputs == null)
    {
        return Results.NotFound();
    }
    foreach (var output in outputs)
    {
        catProdDto.Add(new CatProdDTO
        {
            ProductID = output.ProductID,
            CategoryID = output.CategoryID,
            CategoryName = output.CategoryName,
            Name = output.Name,
            Description = output.Description,
            Price = output.Price,
            Quantity = output.Quantity,
        });
    }
    return Results.Ok(catProdDto);
});

categoryProductServices.MapGet("/description/{description}", (ICatProd catProdDal, string description) =>
{
    List<CatProdDTO> catProdDto = new List<CatProdDTO>();
    var outputs = catProdDal.GetByDescription(description);
    if (outputs == null)
    {
        return Results.NotFound();
    }
    foreach (var output in outputs)
    {
        catProdDto.Add(new CatProdDTO
        {
            ProductID = output.ProductID,
            CategoryID = output.CategoryID,
            CategoryName = output.CategoryName,
            Name = output.Name,
            Description = output.Description,
            Price = output.Price,
            Quantity = output.Quantity,
        });
    }
    return Results.Ok(catProdDto);
});
// Category Product End

app.Run();