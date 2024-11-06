using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Common.Base;

public abstract class ViewModelBase : ObservableObject
{
    public ViewModelBase() { }

    protected ViewModelBase(IView view)
    {
        View = view;
    }

    public IView View { get; set; }

    public virtual void Cleanup()
    {
        WeakReferenceMessenger.Default.Cleanup();
    }
}