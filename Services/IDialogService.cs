using Common.Base;
using Common.Enums;

namespace Services;

public interface IDialogService
{
    bool CheckActivate(string title);

    void SetVM(ViewModelBase vm, string? title, double width, double height, EDialogHostType dialogHostType, bool isModal = true);
}

public class DialogService : IDialogService
{
    public bool CheckActivate(string title)
    {
        throw new NotImplementedException();
    }

    public void SetVM(ViewModelBase vm, string? title, double width, double height, EDialogHostType dialogHostType, bool isModal = true)
    {
        throw new NotImplementedException();
    }
}