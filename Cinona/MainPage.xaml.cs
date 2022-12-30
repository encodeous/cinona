using Cinona.ViewModels;
using LibCinonaHardware.Interfaces;
using LibCinonaHardware.Transport;

namespace Cinona;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}

