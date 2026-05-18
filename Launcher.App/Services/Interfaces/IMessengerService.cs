using CommunityToolkit.Mvvm.Messaging;

namespace Launcher.App.Services;

public interface IMessengerService
{
    IMessenger Messenger { get; }

    void Send<TMessage>(TMessage message)
        where TMessage : class;
}
