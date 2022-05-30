# WPFAcrylics

WPFAcrylics is a *(standalone)* expansion to [Acrylic4WPF](https://github.com/Jackjan4/Acrylic4WPF); which itself is a remake of the [Acrylic Material](https://docs.microsoft.com/en-us/windows/uwp/design/style/acrylic) design from Microsoft that requires UWP/UAP.

*This library is based on the work of [bbougot **(Dead Link)**](https://github.com/bbougot/AcrylicWPF). All credits for creating transparency/blur effects go to him.*

![Example image; empty window with transparent bg](https://i.imgur.com/GwuNif7.jpg)
*Example image that shows a nearly empty acrlic window where the buttons, beside the close button, are deactivated*  
![](https://i.imgur.com/qpM14VD.png)


## Download

Just download one of the releases here in GitHub or compile the project for yourself :)  
There are no dependencies outside of WPF.  
The project is made with .NET Core 6


## Usage

 1. Add the `WPFAcrylics` namespace in your `xaml`:  
	```xaml
	  xmlns:ac="clr-namespace:WPFAcrylics;assembly=WPFAcrylics"
	```

 2. In the `xaml` for the `Window` you want to replace:  
	```xaml
	<ac:AcrylWindow
		x:Class="TestApp.TestWindow2"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
		xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
		xmlns:ac="clr-namespace:WPFAcrylics;assembly=WPFAcrylics"
		mc:Ignorable="d">

		<Window.Resources>
			<!-- ... -->
		</Window.Resources>
		
		<Grid>
			<!-- ... -->
		</Grid>
	</ac:AcrylWindow>
	```  
	Note that there is also `BasicAcrylWindow`, which doesn't have an implicit title bar allowing you to create your own using `WindowChrome.CaptionHeight`.

	Both window types expose a `Handle` property similar to WinForms.

## Customization

- You can change the acrylic opacity with the ````AcrylOpacity```` property
- Changing the acrylic colored background is possible with the ````TransparentBackground```` property
- Editing the blur noise is done by editing the ````NoiseRatio```` property
- Additionally you can enable/disable the TitleBar buttons with ````ShowMinimizeButton```` ````ShowFullscreen```` ````ShowCloseButton````


## Critical Information

- `AcrylWindow` cannot be drag-resized due to the implementation of the title bar. `BasicAcrylWindow` can be resized normally.
- Since WPF has a maximizing bug (maximizing the window larger than the screen actually is), when WindowStyle is set to None, this library uses aditional code the work around this bug. Still, it's not perfect, so display error can occur in rare cases. Mostly, when the user uses your app on two screens.


# License

Since this 
