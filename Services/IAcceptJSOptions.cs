namespace Generic.AcceptJSPaymentGateway
{
    public interface IAcceptJSOptions
    {
        string AcceptJSClientKey();
        string AcceptJSApiLoginID();
        string AcceptJSApiTransactionKey();
        string PayentGatewayView();
    }
}