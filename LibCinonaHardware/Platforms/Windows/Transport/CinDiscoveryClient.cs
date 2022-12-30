using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Security.Cryptography;
using LibCinona.Extensions;
using Microsoft.Extensions.Logging;
using LibCinonaHardware.Interfaces;
using Windows.Devices.Bluetooth.Advertisement;
using System.Diagnostics;

namespace LibCinonaHardware.Transport;

public partial class CinDiscoveryClient : IDiscoveryClient
{

    private BluetoothLEAdvertisementWatcher? watcher;

    private void StopWatching()
    {
        if (watcher != null)
        {
            // Stop the watcher.
            watcher.Stop();
            watcher = null;
        }
    }
    
    public async IAsyncEnumerable<CinPeer> GetPeersAsync()
    {
        StopWatching();
        watcher = new BluetoothLEAdvertisementWatcher
        {
            ScanningMode = BluetoothLEScanningMode.Active,
            AllowExtendedAdvertisements = true
        };

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var c = Channel.CreateUnbounded<CinPeer>();

        watcher.Received += async (w, btAdv) =>
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(btAdv.BluetoothAddress);
            var result = await PingDevice(device.DeviceInformation);
            if (result is not null)
            {
                await c.Writer.WriteAsync(result, cts.Token);
            }
        };

        watcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(CinDiscoveryConstants.ServiceGuid);

        watcher.Start();

        watcher.Stopped += (sender, args) =>
        {
            if (!cts.IsCancellationRequested)
                cts.Cancel();
        };
        cts.Token.Register(() =>
        {
            c.Writer.Complete();
        });
        await foreach (var v in c.Reader.ReadAllAsync())
        {
            yield return v;
        }
        StopWatching();
    }

    private async Task<CinPeer?> PingDevice(DeviceInformation di)
    {
        var leDiv = await BluetoothLEDevice.FromIdAsync(di.Id);
        if (leDiv == null)
        {
            _logger.LogWarning("Failed to connect to device {DId}", di.Id);
            return null;
        }
        var result = await leDiv.GetGattServicesForUuidAsync(CinDiscoveryConstants.ServiceGuid, BluetoothCacheMode.Uncached);

        if (result is null || result.Status != GattCommunicationStatus.Success)
        {
            return null;
        }

        foreach (var service in result.Services)
        {
            if(service.Uuid != CinDiscoveryConstants.ServiceGuid) continue;
            var accessStatus = await service.RequestAccessAsync();
            if (accessStatus == DeviceAccessStatus.Allowed)
            {
                var chars = await service.GetCharacteristicsForUuidAsync(CinDiscoveryConstants.QueryCharacteristic);
                if(chars is null) continue;
                if (chars.Status == GattCommunicationStatus.Success)
                {
                    foreach (var characteristic in chars.Characteristics)
                    {
                        var cinInfo = await RequestCinPeerInfo(characteristic, di);
                        if (cinInfo is not null)
                        {
                            return cinInfo;
                        }
                    }
                }
            }
        }

        return null;
    }

    private async Task<CinPeer?> RequestCinPeerInfo(GattCharacteristic characteristic, DeviceInformation di)
    {
        var result = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
        byte[] data;
        CryptographicBuffer.CopyToByteArray(result.Value, out data);
        var cinPacket = data.Deserialize<CinInfoPacket>();
        if (CryptoExtensions.ValidateSignature(Encoding.UTF8.GetBytes(cinPacket.DeviceName), cinPacket.DeviceNameSig,
                cinPacket.PubKey))
        {
            return new CinPeer()
            {
                DeviceInformation = di,
                Name = cinPacket.DeviceName,
                PubKey = cinPacket.PubKey
            };
        }

        return null;
    }
}
