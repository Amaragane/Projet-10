public class BypassSslValidationHandler : DelegatingHandler
{
    public BypassSslValidationHandler()
    {
        InnerHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    }
}
