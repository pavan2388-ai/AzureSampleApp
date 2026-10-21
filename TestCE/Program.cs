//using Azure.Identity;
//using Azure.Security.KeyVault.Secrets;

//var builder = WebApplication.CreateBuilder(args);

//var vaultUrl = builder.Configuration["KeyVault:VaultUrl"]
//    ?? throw new InvalidOperationException(
//        "KeyVault:VaultUrl is missing.");

//var tenantId = builder.Configuration["KeyVault:TenantId"]
//    ?? throw new InvalidOperationException(
//        "KeyVault:TenantId is missing.");

//builder.Services.AddSingleton<SecretClient>(_ =>
//{
//    var credential = new InteractiveBrowserCredential(
//        new InteractiveBrowserCredentialOptions
//        {
//            TenantId = tenantId
//        });

//    return new SecretClient(new Uri(vaultUrl), credential);
//});

//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();

//---------------------------------------------------------------------
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var vaultUrl = builder.Configuration["KeyVault:VaultUrl"]
    ?? throw new InvalidOperationException(
        "KeyVault:VaultUrl is missing.");

var tenantId = builder.Configuration["KeyVault:TenantId"]
    ?? throw new InvalidOperationException(
        "KeyVault:TenantId is missing.");

var blobUri = builder.Configuration["BlobStorage:ServiceUri"]
    ?? throw new InvalidOperationException(
        "BlobStorage:ServiceUri is missing.");

TokenCredential credential;

if (builder.Environment.IsDevelopment())
{
    credential = new InteractiveBrowserCredential(
        new InteractiveBrowserCredentialOptions
        {
            TenantId = tenantId
        });
}
else
{
    credential = new DefaultAzureCredential();
}

builder.Services.AddSingleton(
    new SecretClient(new Uri(vaultUrl), credential));

builder.Services.AddSingleton(
    new BlobServiceClient(new Uri(blobUri), credential));

builder.Services.AddScoped<BlobAttachmentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
if (builder.Environment.IsDevelopment())
{
    credential = new InteractiveBrowserCredential(
        new InteractiveBrowserCredentialOptions
        {
            TenantId = tenantId
        });
}
else
{
    credential = new DefaultAzureCredential();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

