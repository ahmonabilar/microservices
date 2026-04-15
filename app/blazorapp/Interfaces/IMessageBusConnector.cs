namespace blazorapp.Interfaces
{
    public interface IMessageBusConnector
    {
        Task<(TSuccess success, TFailure failure)> SendMessageSync<TRequest, TSuccess, TFailure>(TRequest request)
            where TSuccess : class
            where TFailure : class
            where TRequest : class;

    }
}
