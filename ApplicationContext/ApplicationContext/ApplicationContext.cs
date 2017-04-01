namespace ToleLibraries.ApplicationContext
{
    public interface IApplicationContext
    {
        bool IsAuthenticated { get; }
        string UserId { get; }
    }
}
