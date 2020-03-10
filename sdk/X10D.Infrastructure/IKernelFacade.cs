namespace X10D.Infrastructure
{
    public interface IKernelFacade : IServicePrototype
    {
        bool IsStable { get; }

        /// <summary>
        /// Проверить системный токен доступа к ядру.
        /// </summary>
        /// <param name="token">Токен.</param>
        /// <returns></returns>
        bool ValidateToken(string token);
    }
}
