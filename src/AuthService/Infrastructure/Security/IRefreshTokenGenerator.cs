namespace AuthService.Infrastructure.Security
{
	public interface IRefreshTokenGenerator
	{
		string Generate();
	}
}

