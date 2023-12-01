namespace Hexagrams.Extensions.Testing.Tests;

public class AutoFakingServiceProviderTests
{
    [Fact]
    public Task Missing_services_are_faked()
    {
        static Task TestAction(MyService service)
        {
            service.GetLyrics.Should().BeEmpty();

            service.GetDependencyValue.Should().BeEmpty();

            return Task.CompletedTask;
        }

        return ServiceTestHarness<MyService>.Create(TestAction)
            .TestAsync();
    }

    [Fact]
    public Task Supplied_services_are_used()
    {
        var suppliedDependency = new MyBologna();

        Task TestAction(MyService service)
        {
            service.GetLyrics.Should().Be(suppliedDependency.SingAlong());

            service.GetDependencyValue.Should().BeEmpty();

            return Task.CompletedTask;
        }

        return ServiceTestHarness<MyService>.Create(TestAction)
            .WithDependency<IMySharona>(suppliedDependency)
            .TestAsync();
    }

    private class MyService(IDependency dependency, IMySharona woo)
    {
        public string GetDependencyValue => dependency.Value;

        public string GetLyrics => woo.SingAlong();
    }

    public interface IDependency
    {
        string Value { get; }
    }

    public interface IMySharona
    {
        string SingAlong();
    }

    private class MyBologna : IMySharona
    {
        public string SingAlong()
        {
            return "M-m-m-my bologna";
        }
    }
}
