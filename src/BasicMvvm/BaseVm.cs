using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BasicMvvm;

public abstract class BaseVm : INotifyPropertyChanging, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public event PropertyChangingEventHandler? PropertyChanging;

    public void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }

    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        OnPropertyChanging(propertyName);
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void SubscribePropertyChanging(string propertyName, Action action)
    {
        PropertyChanging += (sender, eventArgs) =>
        {
            if (eventArgs.PropertyName == propertyName)
            {
                action();
            }
        };
    }

    protected void SubscribePropertyChanged(string propertyName, Action action)
    {
        PropertyChanged += (sender, eventArgs) =>
        {
            if (eventArgs.PropertyName == propertyName)
            {
                action();
            }
        };
    }
}
