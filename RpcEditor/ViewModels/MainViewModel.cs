using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DiscordRPC;

namespace RpcEditor.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _state = "Unconnected";
    [ObservableProperty] private string _greeting = "";
    [ObservableProperty] private string _appId = "679925967153922055";

    [ObservableProperty] private string _pName;
    [ObservableProperty] private string _pState;
    [ObservableProperty] private string _pArtworkLarge;
    [ObservableProperty] private string _pArtworkSmall;
    [ObservableProperty] private string _pTimestamps;

    [ObservableProperty] private string _iName;
    [ObservableProperty] private string _iState;
    [ObservableProperty] private string _iArtworkLarge;
    [ObservableProperty] private string _iArtworkSmall;
    [ObservableProperty] private string _iTimestamps;
    [ObservableProperty] private TimeSpan _iTsStart;
    [ObservableProperty] private TimeSpan _iTsEnd;

    private DiscordRpcClient client;
    public void Update()
    {
        var presence = new RichPresence
        {
            Details = IName,
            State = IState
        };

        if (!string.IsNullOrWhiteSpace(IArtworkLarge) || !string.IsNullOrWhiteSpace(IArtworkSmall))
        {
            presence.Assets = new Assets
            {
                LargeImageKey = IArtworkLarge,
                SmallImageKey = IArtworkSmall
            };
        };
        presence.Timestamps = new Timestamps
        {
            Start = new DateTime(DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromTimeSpan(ITsStart)),
            End = new DateTime(DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromTimeSpan(ITsEnd))
        };

        client.SetPresence(presence);
    }
    public void Connect()
    {
        client = new DiscordRpcClient(AppId);
        client.OnRpcMessage += (sender, msg) =>
        {
            Greeting += $"{msg.Type:G} {msg.TimeCreated}\n";
        };
        client.OnReady += (sender, args) =>
        {
            State = "Connected";
        };
        client.OnPresenceUpdate += (sender, args) =>
        {
            PName = client.CurrentPresence?.Details ?? "No presence set.";
            PState = client.CurrentPresence?.State ?? "No state set.";
            PArtworkLarge = client.CurrentPresence?.Assets?.LargeImageKey ?? "No large artwork set.";
            PArtworkSmall = client.CurrentPresence?.Assets?.SmallImageKey ?? "No small artwork set.";
            PTimestamps = client.CurrentPresence?.HasTimestamps() == true
                ? $"{client.CurrentPresence.Timestamps.Start:F} - {client.CurrentPresence.Timestamps.End:F}"
                : "No timestamps set.";
        };
        client.Initialize();
    }
}