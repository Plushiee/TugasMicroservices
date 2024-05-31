using OrderServices.DAL;
using OrderServices.DAL.Interfaces;
using OrderServices.DTO.OrderHeader;
using OrderServices.DTO.Customer;
using OrderServices.DTO.OrderDetail;
using OrderServices.Models;
using OrderServices.Services;
using Microsoft.OpenApi.Models;
using OrderServices.DTO.Product;
using Polly;
using BCryptHash = BCrypt.Net.BCrypt;
using OrderServices.DTO.Wallet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "DESAIN ARSITEKTUR MICROSERVICES - Order Services",
        Description = "Oleh 72210456 - Nikolaus Pastika Bara Satyaradi",
    });
});

builder.Services.AddScoped<ICustomer, CustomerDapper>();
builder.Services.AddScoped<IOrderHeader, OrderHeaderDapper>();
builder.Services.AddScoped<IOrderDetail, OrderDetailDapper>();

// register HttpClient
builder.Services.AddHttpClient<IProductService, ProductService>().
AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(1000)));
builder.Services.AddHttpClient<IWalletService, WalletService>().
AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(1000)));

var app = builder.Build();

// MapGrouping
var customerServices = app.MapGroup("/orderservices/api/customer").WithTags("Category Service API");
var orderHeaderServices = app.MapGroup("/orderservices/api/orderheader").WithTags("Order Header Service API");
var orderDetailServices = app.MapGroup("/orderservices/api/orderdetail").WithTags("Order Detail Service API");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Customers Start
customerServices.MapGet("/getall", (ICustomer customer) =>
{
    try
    {
        List<CustomerReadDTO> customerDTO = new List<CustomerReadDTO>();
        var customersFromDb = customer.GetAll();
        foreach (var c in customersFromDb)
        {
            customerDTO.Add(new CustomerReadDTO
            {
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName
            });
        }
        return Results.Ok(customerDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

customerServices.MapGet("/getbycustomerid/{id}", (ICustomer customer, int id) =>
{
    try
    {
        CustomerReadDTO customerDTO = new CustomerReadDTO();

        var customerFromDb = customer.GetByCustomerId(id);
        if (customerFromDb == null)
        {
            return Results.NotFound();
        }
        customerDTO.CustomerId = customerFromDb.CustomerId;
        customerDTO.CustomerName = customerFromDb.CustomerName;

        return Results.Ok(customerDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

customerServices.MapPost("/add", (ICustomer customer, CustomerAddDTO customerDTO) =>
{
    try
    {
        if (customerDTO.CustomerName != null)
        {
            Customer customers = new Customer
            {
                CustomerName = customerDTO.CustomerName
            };

            customer.Add(customers);

            return Results.Created($"/orderservices/api/customer/getbyid/{customers.CustomerId}", customers);
        }
        else
        {
            return Results.BadRequest("CustomerName cannot be null.");
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

customerServices.MapPut("/update/{id}", (ICustomer customer, CustomerUpdateDTO customerDTO) =>
{
    try
    {
        var customerFromDb = customer.GetByCustomerId(customerDTO.CustomerId);
        if (customerFromDb == null)
        {
            return Results.NotFound();
        }

        if (customerDTO.CustomerName != null)
        {
            customerFromDb.CustomerName = customerDTO.CustomerName;

            customer.Update(customerFromDb);

            return Results.Ok(customerFromDb);
        }
        else
        {
            return Results.BadRequest("CustomerName cannot be null.");
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

customerServices.MapDelete("/delete/{id}", (ICustomer customer, int id) =>
{
    try
    {
        var customerFromDb = customer.GetByCustomerId(id);
        if (customerFromDb == null)
        {
            return Results.NotFound();
        }

        customer.Delete(id);

        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
// Customers End

// Order Headers Start
orderHeaderServices.MapGet("/getall", (IOrderHeader orderHeader) =>
{
    try
    {
        List<OrderHeaderReadDTO> orderHeaderDTO = new List<OrderHeaderReadDTO>();
        var orderHeadersFromDb = orderHeader.GetAll();
        foreach (var oh in orderHeadersFromDb)
        {
            orderHeaderDTO.Add(new OrderHeaderReadDTO
            {
                OrderHeaderId = oh.OrderHeaderId,
                CustomerId = oh.CustomerId,
                OrderDate = oh.OrderDate,
                WalletId = oh.WalletId
            });
        }

        return Results.Ok(orderHeaderDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

orderHeaderServices.MapGet("/getbyheaderid/{id}", (IOrderHeader orderHeader, int id) =>
{
    try
    {
        OrderHeaderReadDTO orderHeaderDTO = new OrderHeaderReadDTO();

        var orderHeaderFromDb = orderHeader.GetByOrderHeaderId(id);
        if (orderHeaderFromDb == null)
        {
            return Results.NotFound();
        }

        orderHeaderDTO.OrderHeaderId = orderHeaderFromDb.OrderHeaderId;
        orderHeaderDTO.CustomerId = orderHeaderFromDb.CustomerId;
        orderHeaderDTO.OrderDate = orderHeaderFromDb.OrderDate;
        orderHeaderDTO.WalletId = orderHeaderFromDb.WalletId;

        return Results.Ok(orderHeaderDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

orderHeaderServices.MapGet("/getbycustomerid/{id}", (IOrderHeader orderHeader, int id) =>
{
    try
    {
        List<OrderHeaderReadDTO> orderHeaderDTO = new List<OrderHeaderReadDTO>();

        var orderHeaderFromDb = orderHeader.GetByCustomerId(id);
        if (orderHeaderFromDb == null)
        {
            return Results.NotFound();
        }

        foreach (var oh in orderHeaderFromDb)
        {
            orderHeaderDTO.Add(new OrderHeaderReadDTO
            {
                OrderHeaderId = oh.OrderHeaderId,
                CustomerId = oh.CustomerId,
                OrderDate = oh.OrderDate,
                WalletId = oh.WalletId
            });
        }

        return Results.Ok(orderHeaderDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

orderHeaderServices.MapPost("/add", async (IOrderHeader orderHeader, OrderHeaderAddDTO obj, IWalletService walletService) =>
{
    try
    {
        ICustomer customer = new CustomerDapper();
        if (customer.GetByCustomerId(obj.CustomerId) == null)
        {
            return Results.BadRequest("Customer not found");
        }

        var wallet = await walletService.GetByWalletId(obj.WalletId);

        if (wallet == null)
        {
            return Results.BadRequest("Wallet not found");
        }

        if (string.IsNullOrEmpty(wallet.Password))
        {
            return Results.BadRequest("Wallet password is missing.");
        }

        if (wallet.Balance < 0)
        {
            return Results.BadRequest("Wallet balance cannot be negative or null!");
        }

        if (!BCryptHash.Verify(obj.Password, wallet.Password))
        {
            return Results.BadRequest("Password is incorrect!");
        }

        OrderHeader orderHeaders = new OrderHeader
        {
            CustomerId = obj.CustomerId,
            OrderDate = obj.OrderDate,
            WalletId = obj.WalletId
        };

        orderHeader.Add(orderHeaders);

        return Results.Created($"/orderservices/api/orderheader/getbyid/{orderHeaders.OrderHeaderId}", orderHeaders);
        // return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

orderHeaderServices.MapPut("/update/{id}", (IOrderHeader orderHeader, OrderHeaderUpdateDTO orderHeaderDTO) =>
{
    try
    {
        var orderHeaderFromDb = orderHeader.GetByOrderHeaderId(orderHeaderDTO.OrderHeaderId);
        if (orderHeaderFromDb == null)
        {
            return Results.NotFound();
        }

        if (orderHeader.GetByCustomerId(orderHeaderDTO.CustomerId) == null)
        {
            return Results.BadRequest("Customer not found");
        }

        orderHeaderFromDb.CustomerId = orderHeaderDTO.CustomerId;
        orderHeaderFromDb.OrderDate = orderHeaderDTO.OrderDate;
        orderHeaderFromDb.WalletId = orderHeaderDTO.WalletId;

        orderHeader.Update(orderHeaderFromDb);

        return Results.Ok(orderHeaderFromDb);

    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

orderHeaderServices.MapDelete("/delete/{id}", (IOrderHeader orderHeader, IOrderDetail orderDetail, int id) =>
{
    try
    {
        var orderHeaderFromDb = orderHeader.GetByOrderHeaderId(id);
        if (orderHeaderFromDb == null)
        {
            return Results.NotFound();
        }

        var orderDetailFromDb = orderDetail.GetByOrderHeaderId(id);
        if (orderDetailFromDb != null)
        {
            return Results.BadRequest("Order Detail still exists! Please delete Order Detail first!");
        }

        orderHeader.Delete(id);

        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
// Order Headers End

// Order Detail Start
orderDetailServices.MapGet("/getall", (IOrderDetail orderDetail) =>
{
    try
    {
        List<OrderDetailReadDTO> orderDetailDTO = new List<OrderDetailReadDTO>();
        var orderDetailsFromDb = orderDetail.GetAll();
        foreach (var od in orderDetailsFromDb)
        {
            orderDetailDTO.Add(new OrderDetailReadDTO
            {
                OrderDetailId = od.OrderDetailId,
                OrderHeaderId = od.OrderHeaderId,
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                Price = od.Price
            });
        }

        return Results.Ok(orderDetailDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

orderDetailServices.MapGet("/getbyorderdetailid/{id}", (IOrderDetail orderDetail, int id) =>
{
    try
    {
        OrderDetailReadDTO orderDetailDTO = new OrderDetailReadDTO();

        var orderDetailFromDb = orderDetail.GetByOrderDetailId(id);
        if (orderDetailFromDb == null)
        {
            return Results.NotFound();
        }

        orderDetailDTO.OrderDetailId = orderDetailFromDb.OrderDetailId;
        orderDetailDTO.OrderHeaderId = orderDetailFromDb.OrderHeaderId;
        orderDetailDTO.ProductId = orderDetailFromDb.ProductId;
        orderDetailDTO.Quantity = orderDetailFromDb.Quantity;
        orderDetailDTO.Price = orderDetailFromDb.Price;

        return Results.Ok(orderDetailDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

orderDetailServices.MapGet("/getbyorderheaderid/{id}", (IOrderDetail orderDetail, int id) =>
{
    try
    {
        OrderDetailReadDTO orderDetailDTO = new OrderDetailReadDTO();

        var orderDetailFromDb = orderDetail.GetByOrderHeaderId(id);

        if (orderDetailFromDb == null)
        {
            return Results.NotFound();
        }

        orderDetailDTO.OrderDetailId = orderDetailFromDb.OrderDetailId;
        orderDetailDTO.OrderHeaderId = orderDetailFromDb.OrderHeaderId;
        orderDetailDTO.ProductId = orderDetailFromDb.ProductId;
        orderDetailDTO.Quantity = orderDetailFromDb.Quantity;
        orderDetailDTO.Price = orderDetailFromDb.Price;

        return Results.Ok(orderDetailDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

orderDetailServices.MapPost("/add", async (IOrderDetail orderDetailService, IOrderHeader orderHeaderService, IProductService productService, IWalletService walletService, OrderDetailAddDTO obj) =>
{
    try
    {
        var product = await productService.GetByProductId(obj.ProductId);
        var productHeader = orderHeaderService.GetByOrderHeaderId(obj.OrderHeaderId);
        var wallet = await walletService.GetByWalletId(productHeader.WalletId);
        var orderHeaderFromDb = orderHeaderService.GetByOrderHeaderId(obj.OrderHeaderId);

        if (orderHeaderFromDb == null)
        {
            return Results.BadRequest("Order Header not found!");
        }

        if (product == null)
        {
            return Results.BadRequest("Product not found!");
        }

        if (wallet.Balance < obj.Quantity * product.Price)
        {
            return Results.BadRequest("Wallet balance is not enough!");
        }

        if (obj.Quantity > product.Quantity || obj.Quantity <= 0)
        {
            return Results.BadRequest("Quantity must less than stock and not or bellow 0!");
        }

        var orderDetail = new OrderDetail
        {
            OrderHeaderId = obj.OrderHeaderId,
            ProductId = obj.ProductId,
            Quantity = obj.Quantity,
            Price = product.Price
        };

        var addedOrderDetail = orderDetailService.Add(orderDetail);

        var productUpdateQuantityDTO = new ProductUpdateQuantityDTO
        {
            ProductID = obj.ProductId,
            Quantity = obj.Quantity
        };

        await productService.UpdateStockAfterOrder(productUpdateQuantityDTO);

        var walletMinus = new WalletUpdateBalanceDTO
        {
            WalletId = wallet.WalletId,
            Balance = obj.Quantity * product.Price
        };

        await walletService.MinusWallet(walletMinus);

        return Results.Created($"/orderservices/api/orderdetail/getbyid/{addedOrderDetail.OrderDetailId}", obj);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

orderDetailServices.MapPut("/update/{id}", async (IOrderDetail orderDetail, IOrderHeader orderHeader, IProductService productService, IWalletService walletService, OrderDetailUpdateDTO obj) =>
{
    try
    {
        var orderDetailFromDb = orderDetail.GetByOrderDetailId(obj.OrderDetailId);
        if (orderDetailFromDb == null)
        {
            return Results.NotFound();
        }

        var orderHeaderFromDb = orderHeader.GetByOrderHeaderId(obj.OrderHeaderId);
        if (orderHeaderFromDb == null)
        {
            return Results.BadRequest("Order Header not found!");
        }

        var product = await productService.GetByProductId(obj.ProductId);
        if (product == null)
        {
            return Results.BadRequest("Product not found!");
        }

        if (obj.Quantity > product.Quantity || obj.Quantity <= 0)
        {
            return Results.BadRequest("Quantity must be less than stock and greater than 0!");
        }

        var wallet = await walletService.GetByWalletId(orderHeaderFromDb.WalletId);
        var totalCost = obj.Quantity * product.Price;
        if (wallet.Balance < totalCost)
        {
            return Results.BadRequest("Wallet balance is not enough!");
        }

        var oldProduct = await productService.GetByProductId(orderDetailFromDb.ProductId);

        // Update stock of new product
        var productUpdateQuantityDTO = new ProductUpdateQuantityDTO
        {
            ProductID = obj.ProductId,
            Quantity = obj.Quantity - (orderDetailFromDb.ProductId == obj.ProductId ? orderDetailFromDb.Quantity : 0)
        };
        await productService.UpdateStockAfterOrder(productUpdateQuantityDTO);

        // Restore stock of old product if changing product
        if (obj.ProductId != orderDetailFromDb.ProductId)
        {
            var oldProductUpdateQuantityDTO = new ProductUpdateQuantityDTO
            {
                ProductID = orderDetailFromDb.ProductId,
                Quantity = -orderDetailFromDb.Quantity
            };
            await productService.UpdateStockAfterOrder(oldProductUpdateQuantityDTO);
        }

        // Update order detail
        orderDetailFromDb.OrderHeaderId = obj.OrderHeaderId;
        orderDetailFromDb.ProductId = obj.ProductId;
        orderDetailFromDb.Quantity = obj.Quantity;
        orderDetailFromDb.Price = product.Price;

        orderDetail.Update(orderDetailFromDb);

        // Update wallet balance
        var walletUpdateBalanceDTO = new WalletUpdateBalanceDTO
        {
            WalletId = wallet.WalletId,
            Balance = totalCost
        };
        await walletService.MinusWallet(walletUpdateBalanceDTO);

        return Results.Ok(orderDetailFromDb);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});


orderDetailServices.MapDelete("/delete/{id}", async (IOrderDetail orderDetail, int id, IProductService productService, IOrderHeader orderHeader, IWalletService walletService) =>
{
    try
    {
        var orderDetailFromDb = orderDetail.GetByOrderDetailId(id);
        var orderHeaderFromDb = orderHeader.GetByOrderHeaderId(orderDetailFromDb.OrderHeaderId);

        if (orderDetailFromDb == null)
        {
            return Results.NotFound();
        }

        var productUpdateQuantityDTO = new ProductUpdateQuantityDTO
        {
            ProductID = orderDetailFromDb.ProductId, // Corrected from 'id' to 'orderDetailFromDb.ProductId'
            Quantity = -orderDetailFromDb.Quantity
        };

        await productService.UpdateStockAfterOrder(productUpdateQuantityDTO);

        // If the order involves wallet balance, update the wallet balance here
        var wallet = await walletService.GetByWalletId(orderHeaderFromDb.WalletId);
        if (wallet != null)
        {
            var walletUpdateBalanceDTO = new WalletUpdateBalanceDTO
            {
                WalletId = wallet.WalletId,
                Balance = orderDetailFromDb.Quantity * orderDetailFromDb.Price
            };

            await walletService.WalletTopUp(walletUpdateBalanceDTO);
        }

        orderDetail.Delete(id);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
// Order Detail End

app.Run();