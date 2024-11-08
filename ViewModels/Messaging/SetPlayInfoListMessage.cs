using CommunityToolkit.Mvvm.Messaging.Messages;
using Models;
using System.Collections.ObjectModel;

namespace ViewModels.Messaging;

public class SetPlayInfoListMessage : ValueChangedMessage<ObservableCollection<PlayInfoModel>>
{
    public SetPlayInfoListMessage(ObservableCollection<PlayInfoModel> playInfoList) : base(playInfoList) { }
}

public class SetPlayInfoMessage : ValueChangedMessage<PlayInfoModel>
{
    public SetPlayInfoMessage(PlayInfoModel playInfoModel) : base(playInfoModel) { }
}
