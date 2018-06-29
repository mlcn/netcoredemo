using AutoFixture;

namespace Tests
{
	public class BaseTest
    {
		protected readonly Fixture Fixture = new Fixture();
		protected readonly TestLoggerFactory LoggerFactory = new TestLoggerFactory();
    }
}
