namespace Hexagrams.Extensions.Testing.Tests;

public class AutoFakingServiceProviderTests
{
    [Fact]
    public async Task Missing_services_are_faked()
    {
        Task TestAction(MyService service)
        {
            service.GetLyrics.Should().BeEmpty();

            service.GetDependencyValue.Should().BeEmpty();

            return Task.CompletedTask;
        }

        await ServiceTestHarness<MyService>.Create(TestAction)
            .TestAsync();
    }

    [Fact]
    public async Task Supplied_services_are_used()
    {
        var suppliedDependency = new MyBologna();

        Task TestAction(MyService service)
        {
            service.GetLyrics.Should().Be(suppliedDependency.SingAlong());

            service.GetDependencyValue.Should().BeEmpty();

            return Task.CompletedTask;
        }

        await ServiceTestHarness<MyService>.Create(TestAction)
            .WithDependency<IMySharona>(suppliedDependency)
            .TestAsync();
    }

    private class MyService
    {
        private readonly IDependency _dependency;
        private readonly IMySharona _woo;

        public MyService(IDependency dependency, IMySharona woo)
        {
            _dependency = dependency;
            _woo = woo;
        }

        public string GetDependencyValue => _dependency.Value;

        public string GetLyrics => _woo.SingAlong();
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
        public string SingAlong() => "M-m-m-my bologna";
    }
}
