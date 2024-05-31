using Microsoft.OpenApi.Models;
using WalletServices.DAL.Interfaces;
using WalletServices.DAL;
using WalletServices.DTO.Wallet;
using WalletServices.DTO.Transfer;
using WalletServices.Models;
using BCryptHash = BCrypt.Net.BCrypt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "DESAIN ARSITEKTUR MICROSERVICES - Wallet Services",
        Description = "Oleh 72210456 - Nikolaus Pastika Bara Satyaradi",
    });
});

builder.Services.AddScoped<IWallet, WalletDapper>();
builder.Services.AddScoped<ITransfer, TransferDapper>();

var app = builder.Build();

// MapGrouping
var walletServices = app.MapGroup("/walletservices/api/wallet").WithTags("Wallet Service API");
var transferServices = app.MapGroup("/walletservices/api/transfer").WithTags("Transfer Service API");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Wallet Start
walletServices.MapGet("/getall", (IWallet wallet) =>
{
    try
    {
        List<WalletReadDTO> walletDTO = new List<WalletReadDTO>();
        var walletFromDb = wallet.GetAll();
        foreach (var w in walletFromDb)
        {
            walletDTO.Add(new WalletReadDTO
            {
                WalletId = w.WalletId,
                Username = w.Username,
                Email = w.Email,
                FullName = w.FullName,
                Password = w.Password,
                Balance = w.Balance
            });
        }
        return Results.Ok(walletDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

walletServices.MapGet("/getbyemail", (IWallet wallet, string email) =>
{
    try
    {
        List<WalletReadDTO> walletDTO = new List<WalletReadDTO>();
        var walletFromDb = wallet.GetByEmail(email);
        foreach (var w in walletFromDb)
        {
            walletDTO.Add(new WalletReadDTO
            {
                WalletId = w.WalletId,
                Username = w.Username,
                Email = w.Email,
                FullName = w.FullName,
                Password = w.Password,
                Balance = w.Balance
            });
        }
        return Results.Ok(walletDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

walletServices.MapGet("/getbyfullname", (IWallet wallet, string username) =>
{
    try
    {
        List<WalletReadDTO> walletDTO = new List<WalletReadDTO>();
        var walletFromDb = wallet.GetByUsername(username);
        foreach (var w in walletFromDb)
        {
            walletDTO.Add(new WalletReadDTO
            {
                WalletId = w.WalletId,
                Username = w.Username,
                Email = w.Email,
                FullName = w.FullName,
                Password = w.Password,
                Balance = w.Balance
            });
        }
        return Results.Ok(walletDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

walletServices.MapGet("/getbyusername", (IWallet wallet, string fullName) =>
{
    try
    {
        List<WalletReadDTO> walletDTO = new List<WalletReadDTO>();
        var walletFromDb = wallet.GetByFullName(fullName);
        foreach (var w in walletFromDb)
        {
            walletDTO.Add(new WalletReadDTO
            {
                WalletId = w.WalletId,
                Username = w.Username,
                Email = w.Email,
                FullName = w.FullName,
                Password = w.Password,
                Balance = w.Balance
            });
        }
        return Results.Ok(walletDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

walletServices.MapGet("/getbyid/{id}", (IWallet wallet, int id) =>
{
    try
    {
        WalletReadDTO walletDTO = new WalletReadDTO();
        var walletFromDb = wallet.GetByWalletId(id);
        if (walletFromDb == null)
        {
            return Results.NotFound();
        }

        walletDTO.WalletId = walletFromDb.WalletId;
        walletDTO.Username = walletFromDb.Username;
        walletDTO.Email = walletFromDb.Email;
        walletDTO.FullName = walletFromDb.FullName;
        walletDTO.Password = walletFromDb.Password;
        walletDTO.Balance = walletFromDb.Balance;

        return Results.Ok(walletDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

walletServices.MapGet("/getbybalance", (IWallet wallet, float start, float end) =>
{
    try
    {
        if (start > end)
        {
            return Results.BadRequest("Start balance must be less than end balance");
        }

        List<WalletReadDTO> walletDTO = new List<WalletReadDTO>();
        var walletFromDb = wallet.GetByBalance(start, end);
        foreach (var w in walletFromDb)
        {
            walletDTO.Add(new WalletReadDTO
            {
                WalletId = w.WalletId,
                Username = w.Username,
                Email = w.Email,
                FullName = w.FullName,
                Password = w.Password,
                Balance = w.Balance
            });
        }
        return Results.Ok(walletDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

walletServices.MapPost("/add", (IWallet wallet, WalletAddDTO walletDTO) =>
{
    try
    {
        var walletFromDb = wallet.GetByUsername(walletDTO.Username);

        if (walletFromDb.Count() > 0)
        {
            return Results.BadRequest("Username already exists");
        }
        var newWallet = new Wallet
        {
            Username = walletDTO.Username,
            Email = walletDTO.Email,
            FullName = walletDTO.FullName,
            Password = BCryptHash.HashPassword(walletDTO.Password),
            Balance = walletDTO.Balance
        };

        var addedWallet = wallet.Add(newWallet);

        return Results.Created($"/walletservices/api/wallet/getbyid/{addedWallet.WalletId}", addedWallet);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

walletServices.MapPut("/update", (IWallet wallet, WalletUpdateDTO walletDTO) =>
{
    try
    {
        var walletFromDb = wallet.GetByWalletId(walletDTO.WalletId);

        if (walletFromDb == null)
        {
            return Results.NotFound();
        }

        var updateWallet = new Wallet
        {
            WalletId = walletDTO.WalletId,
            Username = walletDTO.Username,
            Email = walletDTO.Email,
            FullName = walletDTO.FullName,
            Password = BCryptHash.HashPassword(walletDTO.Password),
            Balance = walletDTO.Balance
        };

        wallet.Update(updateWallet);

        return Results.Ok(walletDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

walletServices.MapPut("/topupsaldo", (IWallet wallet, int id, float balance) =>
{
    try
    {
        var walletFromDb = wallet.GetByWalletId(id);

        if (walletFromDb == null)
        {
            return Results.NotFound();
        }

        if (balance <= 0)
        {
            return Results.BadRequest("Balance must be greater than 0");
        }

        wallet.TopUpWallet(id, balance);

        return Results.Ok(wallet.GetByWalletId(id));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

walletServices.MapDelete("/delete/{id}", (IWallet wallet, int id) =>
{
    try
    {
        var walletFromDb = wallet.GetByWalletId(id);

        if (walletFromDb == null)
        {
            return Results.NotFound();
        }

        wallet.Delete(id);

        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

walletServices.MapPut("/minuswallet", (IWallet wallet, int id, float balance) =>
{
    try
    {
        var walletFromDb = wallet.GetByWalletId(id);

        if (walletFromDb == null)
        {
            return Results.NotFound();
        }

        if (balance <= 0)
        {
            return Results.BadRequest("Balance must be greater than 0");
        }

        wallet.MinusWallet(id, balance);

        return Results.Ok(wallet.GetByWalletId(id));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
// Wallet End

// Transfer Start
transferServices.MapPost("/add", (ITransfer transfer, IWallet wallet, TransferAddDTO transferDTO) =>
{
    try
    {
        var walletFromDb = wallet.GetByWalletId(transferDTO.WalletIdFrom);

        if (walletFromDb == null)
        {
            return Results.NotFound();
        }

        var walletToDb = wallet.GetByWalletId(transferDTO.WalletIdTo);

        if (walletToDb == null)
        {
            return Results.NotFound();
        }

        if (!BCryptHash.Verify(transferDTO.Password, walletFromDb.Password))
        {
            return Results.BadRequest("Invalid password");
        }

        if (walletFromDb.Balance < transferDTO.Balance)
        {
            return Results.BadRequest("Insufficient balance");
        }

        var jakartaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var jakartaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jakartaTimeZone);

        var newTransfer = new Transfer
        {
            WalletIdFrom = transferDTO.WalletIdFrom,
            WalletIdTo = transferDTO.WalletIdTo,
            Balance = transferDTO.Balance,
            Date = jakartaTime.ToString()
        };

        var addedTransfer = transfer.Add(newTransfer);

        var updatedWalletFrom = wallet.MinusWallet(transferDTO.WalletIdFrom, transferDTO.Balance);
        var updatedWalletTo = wallet.TopUpWallet(transferDTO.WalletIdTo, transferDTO.Balance);

        return Results.Created($"/walletservices/api/transfer/getbyid/{addedTransfer.TransferId}", addedTransfer);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

transferServices.MapGet("/getall", (ITransfer transfer) =>
{
    try
    {
        List<TransferReadDTO> transferDTO = new List<TransferReadDTO>();
        var transferFromDb = transfer.GetAll();
        foreach (var t in transferFromDb)
        {
            transferDTO.Add(new TransferReadDTO
            {
                TransferId = t.TransferId,
                WalletIdTo = t.WalletIdTo,
                WalletIdFrom = t.WalletIdFrom,
                Balance = t.Balance,
                Date = t.Date
            });
        }
        return Results.Ok(transferDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

transferServices.MapGet("/getbyid/{id}", (ITransfer transfer, int id) =>
{
    try
    {
        TransferReadDTO transferDTO = new TransferReadDTO();

        var transferFromDb = transfer.GetByTransferId(id);
        if (transferFromDb == null)
        {
            return Results.NotFound();
        }

        transferDTO.TransferId = transferFromDb.TransferId;
        transferDTO.WalletIdTo = transferFromDb.WalletIdTo;
        transferDTO.WalletIdFrom = transferFromDb.WalletIdFrom;
        transferDTO.Balance = transferFromDb.Balance;
        transferDTO.Date = transferFromDb.Date;

        return Results.Ok(transferDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

transferServices.MapGet("/getbywalletfrom/{id}", (ITransfer transfer, int id) =>
{
    try
    {
        List<TransferReadDTO> transferDTO = new List<TransferReadDTO>();
        var transferFromDb = transfer.GetByWalletIdFrom(id);
        foreach (var t in transferFromDb)
        {
            transferDTO.Add(new TransferReadDTO
            {
                TransferId = t.TransferId,
                WalletIdTo = t.WalletIdTo,
                WalletIdFrom = t.WalletIdFrom,
                Balance = t.Balance,
                Date = t.Date
            });
        }
        return Results.Ok(transferDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

transferServices.MapGet("/getbywalletto/{id}", (ITransfer transfer, int id) =>
{
    try
    {
        List<TransferReadDTO> transferDTO = new List<TransferReadDTO>();
        var transferFromDb = transfer.GetByWalletIdTo(id);
        foreach (var t in transferFromDb)
        {
            transferDTO.Add(new TransferReadDTO
            {
                TransferId = t.TransferId,
                WalletIdTo = t.WalletIdTo,
                WalletIdFrom = t.WalletIdFrom,
                Balance = t.Balance,
                Date = t.Date
            });
        }
        return Results.Ok(transferDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

transferServices.MapGet("/getbydate", (ITransfer transfer, string date) =>
{
    try
    {
        List<TransferReadDTO> transferDTO = new List<TransferReadDTO>();
        var transferFromDb = transfer.GetByDate(date);
        foreach (var t in transferFromDb)
        {
            transferDTO.Add(new TransferReadDTO
            {
                TransferId = t.TransferId,
                WalletIdTo = t.WalletIdTo,
                WalletIdFrom = t.WalletIdFrom,
                Balance = t.Balance,
                Date = t.Date
            });
        }
        return Results.Ok(transferDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

transferServices.MapGet("getbybalance", (ITransfer transfer, float start, float end) =>
{
    try
    {
        if (start > end)
        {
            return Results.BadRequest("Start balance must be less than end balance");
        }

        List<TransferReadDTO> transferDTO = new List<TransferReadDTO>();
        var transferFromDb = transfer.GetByBalance(start, end);
        foreach (var t in transferFromDb)
        {
            transferDTO.Add(new TransferReadDTO
            {
                TransferId = t.TransferId,
                WalletIdTo = t.WalletIdTo,
                WalletIdFrom = t.WalletIdFrom,
                Balance = t.Balance,
                Date = t.Date
            });
        }
        return Results.Ok(transferDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);

    }
});

transferServices.MapDelete("/delete/{id}", (ITransfer transfer, int id) =>
{
    try
    {
        var transferFromDb = transfer.GetByTransferId(id);

        if (transferFromDb == null)
        {
            return Results.NotFound();
        }

        transfer.Delete(id);

        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

transferServices.MapPut("/update", (ITransfer transfer, IWallet wallet, TransferUpdateDTO transferDTO) =>
{
    try
    {
        var transferFromDb = transfer.GetByTransferId(transferDTO.TransferId);
        var jakartaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var jakartaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jakartaTimeZone);


        if (transferFromDb == null)
        {
            return Results.NotFound();
        }

        var walletFromDb = wallet.GetByWalletId(transferDTO.WalletIdFrom);

        if (transferDTO.Balance > walletFromDb.Balance)
        {
            return Results.BadRequest("Insufficient balance");
        }

        if (walletFromDb == null)
        {
            return Results.NotFound();
        }

        if (transferDTO.Balance <= 0)
        {
            return Results.BadRequest("Balance must be greater than 0");
        }

        var newTransfer = new Transfer
        {
            WalletIdFrom = transferDTO.WalletIdFrom,
            WalletIdTo = transferDTO.WalletIdTo,
            Balance = transferDTO.Balance,
            Date = transferDTO.Date
        };

        if (transferDTO.Date == "")
        {
            newTransfer.Date = transferFromDb.Date;
        }

        transfer.Update(newTransfer);

        if (transferDTO.Balance > transferFromDb.Balance)
        {
            var balance = transferDTO.Balance - transferFromDb.Balance;
            wallet.MinusWallet(transferDTO.WalletIdFrom, balance);
            wallet.TopUpWallet(transferDTO.WalletIdTo, balance);
        }
        else
        {
            var balance = transferFromDb.Balance - transferDTO.Balance;
            wallet.TopUpWallet(transferDTO.WalletIdFrom, balance);
            wallet.MinusWallet(transferDTO.WalletIdTo, balance);
        }

        return Results.Ok(transferDTO);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

// Transfer End

app.Run();