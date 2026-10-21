using Azure;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly SecretClient _secretClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        SecretClient secretClient,
        ILogger<HomeController> logger)
    {
        _secretClient = secretClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var quickBookSecret = await _secretClient.GetSecretAsync(
                "QuickBookWebHookVerifierToken");

            var openAISecret = await _secretClient.GetSecretAsync(
                "OpenAIKey");

            var connectionStringSecret = await _secretClient.GetSecretAsync(
                "ConnectionStringCE");

            ViewBag.QuickBookToken = quickBookSecret.Value.Value;
            ViewBag.OpenAIKey = openAISecret.Value.Value;
            ViewBag.ConnectionString = connectionStringSecret.Value.Value;

            ViewBag.KeyVaultStatus = "All secrets retrieved successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Key Vault access failed.");
            ViewBag.KeyVaultStatus =
                "Key Vault access failed. Check application logs.";
        }

        return View();
    }
}