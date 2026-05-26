namespace ShopView.Infrastructure
{
    public interface ICartCountProvider
    {
        Task<int> GetCartItemCountAsync(CancellationToken cancellationToken = default);
    }
}
