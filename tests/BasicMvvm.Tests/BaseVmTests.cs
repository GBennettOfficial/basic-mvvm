using System.ComponentModel;
using BasicMvvm.Tests.Examples;
using NSubstitute;

namespace BasicMvvm.Tests;

// xUnit creates a new test class instance for every test
public class BaseVmTests
{
    private readonly LoginVm _vm = new();
    private readonly PropertyChangingEventHandler _changing = Substitute.For<PropertyChangingEventHandler>();
    private readonly PropertyChangedEventHandler _changed = Substitute.For<PropertyChangedEventHandler>();

    public BaseVmTests()
    {
        _vm.PropertyChanging += _changing;
        _vm.PropertyChanged += _changed;
    }

    [Fact]
    public void SettingNewValue_RaisesPropertyChangingThenPropertyChanged()
    {
        _vm.Username = "george";

        Received.InOrder(() =>
        {
            _changing.Invoke(_vm, Arg.Is<PropertyChangingEventArgs>(e => e.PropertyName == nameof(LoginVm.Username)));
            _changed.Invoke(_vm, Arg.Is<PropertyChangedEventArgs>(e => e.PropertyName == nameof(LoginVm.Username)));
        });
    }

    [Fact]
    public void SettingNewValue_RaisesEachEventExactlyOnce()
    {
        _vm.Username = "george";

        _changing.Received(1).Invoke(Arg.Any<object?>(), Arg.Any<PropertyChangingEventArgs>());
        _changed.Received(1).Invoke(Arg.Any<object?>(), Arg.Any<PropertyChangedEventArgs>());
    }

    [Fact]
    public void SettingNewValue_UpdatesProperty()
    {
        _vm.Username = "george";

        Assert.Equal("george", _vm.Username);
    }

    [Fact]
    public void SettingSameValue_RaisesNoEvents()
    {
        _vm.Username = "george";
        _changing.ClearReceivedCalls();
        _changed.ClearReceivedCalls();

        _vm.Username = "george";

        _changing.DidNotReceiveWithAnyArgs().Invoke(default, default!);
        _changed.DidNotReceiveWithAnyArgs().Invoke(default, default!);
    }

    [Fact]
    public void SettingDefaultValue_RaisesNoEvents()
    {
        _vm.Username = "";

        _changing.DidNotReceiveWithAnyArgs().Invoke(default, default!);
        _changed.DidNotReceiveWithAnyArgs().Invoke(default, default!);
    }

    [Fact]
    public void PropertyChanging_SeesOldValue_PropertyChanged_SeesNewValue()
    {
        _vm.Username = "old";
        string? valueDuringChanging = null;
        string? valueDuringChanged = null;
        _changing.When(h => h.Invoke(Arg.Any<object?>(), Arg.Any<PropertyChangingEventArgs>()))
                 .Do(_ => valueDuringChanging = _vm.Username);
        _changed.When(h => h.Invoke(Arg.Any<object?>(), Arg.Any<PropertyChangedEventArgs>()))
                .Do(_ => valueDuringChanged = _vm.Username);

        _vm.Username = "new";

        Assert.Equal("old", valueDuringChanging);
        Assert.Equal("new", valueDuringChanged);
    }

    [Fact]
    public void SettingPassword_AlsoRaisesPropertyChangedForPasswordMask()
    {
        _vm.Password = "secret";

        _changed.Received(1).Invoke(_vm, Arg.Is<PropertyChangedEventArgs>(e => e.PropertyName == nameof(LoginVm.Password)));
        _changed.Received(1).Invoke(_vm, Arg.Is<PropertyChangedEventArgs>(e => e.PropertyName == nameof(LoginVm.PasswordMask)));
    }

    [Fact]
    public void SettingPassword_DoesNotRaisePropertyChangingForPasswordMask()
    {
        _vm.Password = "secret";

        _changing.DidNotReceive().Invoke(Arg.Any<object?>(), Arg.Is<PropertyChangingEventArgs>(e => e.PropertyName == nameof(LoginVm.PasswordMask)));
    }

    [Fact]
    public void SettingSamePassword_DoesNotRaisePropertyChangedForPasswordMask()
    {
        _vm.Password = "secret";
        _changed.ClearReceivedCalls();

        _vm.Password = "secret";

        _changed.DidNotReceiveWithAnyArgs().Invoke(default, default!);
    }

    [Fact]
    public void SettingUsername_DoesNotRaisePropertyChangedForPasswordMask()
    {
        _vm.Username = "george";

        _changed.DidNotReceive().Invoke(Arg.Any<object?>(), Arg.Is<PropertyChangedEventArgs>(e => e.PropertyName == nameof(LoginVm.PasswordMask)));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("a", "*")]
    [InlineData("secret", "******")]
    public void PasswordMask_HasOneAsteriskPerCharacter(string password, string expectedMask)
    {
        _vm.Password = password;

        Assert.Equal(expectedMask, _vm.PasswordMask);
    }

    [Fact]
    public void OnPropertyChanged_RaisesEventWithGivenName()
    {
        _vm.OnPropertyChanged("Anything");

        _changed.Received(1).Invoke(_vm, Arg.Is<PropertyChangedEventArgs>(e => e.PropertyName == "Anything"));
    }

    [Fact]
    public void OnPropertyChanging_RaisesEventWithGivenName()
    {
        _vm.OnPropertyChanging("Anything");

        _changing.Received(1).Invoke(_vm, Arg.Is<PropertyChangingEventArgs>(e => e.PropertyName == "Anything"));
    }

    [Fact]
    public void UnsubscribedHandler_IsNotCalled()
    {
        _vm.PropertyChanged -= _changed;
        _vm.PropertyChanging -= _changing;

        _vm.Username = "george";

        _changing.DidNotReceiveWithAnyArgs().Invoke(default, default!);
        _changed.DidNotReceiveWithAnyArgs().Invoke(default, default!);
    }

    [Fact]
    public void RaisingEvents_WithNoSubscribers_DoesNotThrow()
    {
        var vm = new LoginVm();

        var exception = Record.Exception(() =>
        {
            vm.Username = "george";
            vm.Password = "secret";
        });

        Assert.Null(exception);
    }
}
