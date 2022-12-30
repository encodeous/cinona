using LibCinonaHardware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using ABI.Windows.Devices.Enumeration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using LibCinona.Extensions;

namespace LibCinonaHardware.Transport
{
    public partial class CinDiscoveryServer : IDiscoveryServer
    {
        private GattServiceProvider serviceProvider;

        private static readonly GattLocalCharacteristicParameters ConnectParams = new()
        {
            CharacteristicProperties = GattCharacteristicProperties.Write,
            WriteProtectionLevel = GattProtectionLevel.Plain,
            UserDescription = "Cinona Connect Characteristic"
        };
        private static readonly GattLocalCharacteristicParameters QueryParams = new()
        {
            CharacteristicProperties = GattCharacteristicProperties.Read,
            WriteProtectionLevel = GattProtectionLevel.Plain,
            UserDescription = "Cinona Query Device Information Characteristic",
            PresentationFormats =
            {
                GattPresentationFormat.FromParts(
                    GattPresentationFormatTypes.Utf8, 0, 0x2700, 1, 0)
            }
        };

        private GattLocalCharacteristic connectChar;
        private GattLocalCharacteristic queryChar;

        public async Task<bool> IsSupported()
        {
            var localAdapter = await BluetoothAdapter.GetDefaultAsync();
            if (localAdapter != null) return localAdapter.IsPeripheralRoleSupported;
            return false;
        }

        public async Task<bool> Start()
        {
            _logger.LogInformation("Started listening for discovery pings");
            var res = await GattServiceProvider.CreateAsync(CinDiscoveryConstants.ServiceGuid);
            if (res.Error == BluetoothError.Success)
            {
                serviceProvider = res.ServiceProvider;
            }
            else
            {
                _logger.LogError("Could not create BLE service provider {ResError}", res.Error);
                return false;
            }

            var cResult =
                await serviceProvider.Service.CreateCharacteristicAsync(CinDiscoveryConstants.ConnectCharacteristic,
                    ConnectParams);
            if (cResult.Error == BluetoothError.Success)
            {
                connectChar = cResult.Characteristic;
            }
            else
            {
                _logger.LogError("Could not create BLE characteristic for connect {CResultError}", cResult.Error);
            }
            
            connectChar.WriteRequested += ConnectCharOnWriteRequested;
            
            cResult =
                await serviceProvider.Service.CreateCharacteristicAsync(CinDiscoveryConstants.QueryCharacteristic,
                    QueryParams);
            if (cResult.Error == BluetoothError.Success)
            {
                queryChar = cResult.Characteristic;
            }
            else
            {
                _logger.LogError("Could not create BLE characteristic for query {CResultError}", cResult.Error);
            }
            
            queryChar.ReadRequested += QueryCharOnReadRequested;

            var advParams = new GattServiceProviderAdvertisingParameters()
            {
                IsConnectable = true,
                IsDiscoverable = true
            };
            serviceProvider.AdvertisementStatusChanged += ServiceProviderOnAdvertisementStatusChanged;
            serviceProvider.StartAdvertising(advParams);
            return true;
        }

        private void ServiceProviderOnAdvertisementStatusChanged(GattServiceProvider sender, GattServiceProviderAdvertisementStatusChangedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        private async void QueryCharOnReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            _logger.LogInformation("Received query for device information");
            using (args.GetDeferral())
            {
                GattReadRequest request = await args.GetRequestAsync();
                if (request == null)
                {
                    return;
                }
                
                var writer = new DataWriter();
                writer.ByteOrder = ByteOrder.LittleEndian;
                var name = _cinConfigService.GetInfo().Name;
                var pkt = new CinInfoPacket()
                {
                    PubKey = _cinCryptoService.GetPublicKey(),
                    DeviceName = name,
                    DeviceNameSig = _cinCryptoService.Sign(Encoding.UTF8.GetBytes(name))
                };
                writer.WriteBytes(pkt.Serialize());

                // Gatt code to handle the response
                request.RespondWithValue(writer.DetachBuffer());
            }
        }

        private void ConnectCharOnWriteRequested(GattLocalCharacteristic sender, GattWriteRequestedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        public void Stop()
        {
            //throw new NotImplementedException();
        }
    }
}
