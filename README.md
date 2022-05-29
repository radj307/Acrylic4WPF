# Acrylic4WPF

Acrylic4WPF is a remake of the [Acrylic Material](https://docs.microsoft.com/en-us/windows/uwp/design/style/acrylic) design from Microsoft that can only be used in UWP.

*This library is based on the work of [bbougot](https://github.com/bbougot/AcrylicWPF). All credits for creating transparency/blur effects go to him.*


![Example image; empty window with transparent bg](https://i.imgur.com/GwuNif7.jpg)
*Example image that shows a nearly empty acrlic window where the buttons, beside the close button, are deactivated*


## Download

Just download one of the releases here in GitHub or compile the project for yourself :)
The project is made with .NET Framework 4.6.1.


## Usage

**Notes**

- Can only be used with `WindowStyle.None`


Add the namespace in your `xaml`:

```xaml
<acryl:AcrylWindow
    x:Class="WpfApp1.acrylic"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:acryl="clr-namespace:WPFAcrylics;assembly=WPFAcrylics"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    Title="acrylic"
    WindowStyle="None"
    mc:Ignorable="d">
    <AcrylWindow.Resources>
    </AcrylWindow.Resources>


    <!-- Here we use WindowChrome to create a custom caption bar -->
    <WindowChrome.WindowChrome>
        <WindowChrome CaptionHeight="20" ResizeBorderThickness="{x:Static SystemParameters.WindowResizeBorderThickness}" />
    </WindowChrome.WindowChrome>

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height=""/>
        </Grid.RowDefinitions>

    <Grid>
</acryl:AcrylWindow>
```

When creating a new window in WPF, instead of extending the ````Window```` class you should extend the ````AcrylWindow```` class. You need to to this both in XAML and in Code.
Styling your other controls like buttons etc. to look acrylic compatible has to be done by yourself, however most work is already done by removing borders and setting transparent backgrounds.


## Custimization

- You can change the acrylic opacity with the ````AcrylOpacity```` property
- Changing the acrylic colored background is possible with the ````TransparentBackground```` property
- Editing the blur noise is done by editing the ````NoiseRatio```` property
- Additionally you can enable/disable the TitleBar buttons with ````ShowMinimizeButton```` ````ShowFullscreen```` ````ShowCloseButton````


## Critical Information

- Do NOT change WindowStyle in your window, since the TitleBar is redesigned in this libary and changing causes crashes or two TitleBars, instead use the ShowCloseButton, ... properties.
- Since WPF has a maximizing bug (maximizing the window larger than the screen actually is), when WindowStyle is set to None, this library uses aditional code the work around this bug. Still, it's not perfect, so display error can occur in rare cases. Mostly, when the user uses your app on two screens.
- AcrylWindows are not resizable with the mouse yet, since it has to be implemented manually.


## Further Information / ToDo

- Better window drag, when dragging out of maximized state
- Adding DependencyProperties so, acrylic effects can be made bindable
