using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibCinonaHardware.Interfaces;
using LibCinonaHardware.Transport;

namespace Cinona.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private IDiscoveryClient _client;
    private IDiscoveryServer _server;

    public MainViewModel(IDiscoveryClient client, IDiscoveryServer server)
    {
        _client = client;
        _server = server;
        devices = new();
    }
    [ObservableProperty]
    ObservableCollection<CinPeer> devices;
    [RelayCommand]
    async Task RefreshDevicesAsync()
    {
        _server.Stop();
        devices.Clear();
        await foreach (var peer in _client.GetPeersAsync())
        {
            devices.Add(peer);
        }
    }
    [RelayCommand]
    async Task AdvertiseAsync()
    {
        await _server.Start();
    }
}