using Cinona.ViewModels;
using LibCinona.Extensions;
using LibCinonaHardware;
using LibCinonaHardware.Interfaces;
using LibCinonaHardware.Transport;
using Microsoft.Extensions.Logging;

namespace Cinona;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddCinona();
		builder.Services.AddCinonaHardware();
		builder.Services.AddSingleton<MainPage>();
        // view models
        builder.Services.AddSingleton<MainViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
		var built = builder.Build();
		return built;
	}
}
