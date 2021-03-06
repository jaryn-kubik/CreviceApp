 
| master | develop |
|--------|---------|
| [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/master?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/master) | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/develop?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/develop) |

<p align="center"> 
<img src="https://i.imgur.com/1BSdwPN.png" alt="alt logo">
</p>

Crevice is a mouse gesture utility which is consisted of small and robust core of 2000 lines fully tested, thin GUI wrapper, and [Microsoft Roslyn](https://github.com/dotnet/roslyn).
Mouse gestures can be defined as a C# Script file, so there is nothing that can not be done.<sup>[citation needed]</sup>

This software requires Windows 7 or later, and .Net Framework 4.6.

## Quickstart

Extract zip file to any location, and click `CreviceApp.exe`.

### Userscript

After the first execution of Crevice, you could find `default.csx` in the directory `%APPDATA%\Crevice\CreviceApp`. It's the user script file. Please open it with a text editor and take a look through.


After several `using` declaring lines, you will see `Browser` definition following:

```cs
var Browser = @when((ctx) =>
{
    return ctx.ForegroundWindow.ModuleName == "chrome.exe" ||
           ctx.ForegroundWindow.ModuleName == "firefox.exe" ||
           ctx.ForegroundWindow.ModuleName == "opera.exe" ||
           ctx.ForegroundWindow.ModuleName == "iexplore.exe");
});
```

When the `ModuleName` of `ForegroundWindow` is as follows, `chrome.exe`, `firefox.exe`, `opera.exe` .., then, `@when` returns true; this is a declare of browsers. 

After declaration of `Browser`, the declaration of mouse gestures of it follows. Let's see the first one:

```cs
Browser.
@on(RightButton).
@if(WheelUp).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_CONTROL).
    ExtendedKeyDown(VK_SHIFT).
    ExtendedKeyDown(VK_TAB).
    ExtendedKeyUp(VK_TAB).
    ExtendedKeyUp(VK_SHIFT).
    ExtendedKeyUp(VK_CONTROL).
    Send(); // Previous tab
});
```

This is a mouse gesture definition; when you press and hold `RightButton`, and then if you `WheelUp` the mouse, codes declared in `@do` will be executed.


This file is just a C# Script file. So, you can use `#load` directive to load another csx file, and can use `#r` directive to add assembly references to the script. By default, the script has the assembly references to `microlib.dll`, `System.dll`, `System.Core.dll`, `Microsoft.CSharp.dll` and `CreviceApp.exe`. In other words, if there need to add an another assembly reference to the script, it should be declared by using `#r` directive at the head of the script.

For more information about C# Script, please see [Directives - Interactive Window · dotnet/roslyn Wiki](https://github.com/dotnet/roslyn/wiki/Interactive-Window#directives).

## Mouse gesture DSL

All mouse gesture definition starting from `@when` clause represents the condition for the activation of a mouse gesture.
```cs
var Chrome = @when((ctx) =>
{
    return ctx.ForegroundWindow.ModuleName == "chrome.exe";
});
```

The next `@when` is `@on`, the next will continue `@if`, and `@do`.

```cs
Chrome.
@on(RightButton).
@if(MoveDown, MoveRight).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_LCONTROL).
    KeyDown(VK_W).
    KeyUp(VK_W).
    ExtendedKeyUp(VK_LCONTROL).
    Send(); // Send Ctrl+W to Chrome
});
```


`@on` caluse tells the system which mouse button will be used at start of the gesture. 

`@on` clause takes an argument any of following; `LeftButton`, `MiddleButton`, `RightButton`, `X1Button`, `X2Button`.

`@if` clause tells the trigger of the action of the gesture. 

And finally, `@do` clause represents the action of the gesture to be activated when all given conditions to be satisfied. 

### Stroke gestures

"Mouse gestures by strokes", namely "stroke gesture" is in the functions of mouse gesture utilities, is the most important part. Crevice naturally supports this.

`@if` clause takes arguments that consist of combination of `MoveUp`, `MoveDown`, `MoveLeft` and `MoveRight`. These are directions of movements of the mouse pointer.

```cs
Chrome.
@on(RightButton).
@if(MoveUp, MoveDown, MoveLeft, MoveRight, ...). // There is no limit on the length.
@do((ctx) => {
    SendInput.Multiple().
    ExtendedKeyDown(VK_LCONTROL).
    ExtendedKeyDown(VK_LSHIFT).
    KeyDown(VK_T).
    KeyUp(VK_T).
    ExtendedKeyUp(VK_LSHIFT).
    ExtendedKeyUp(VK_LCONTROL).
    Send(); // Send Ctrl+Shift+T to Chrome
});
```

### Button gestures
As you may know, mouse gestures with buttons is called "rocker gestures" in mouse gesture utility communities. 
But we call it "button gestures" here. 
Crevice supports two kinds of button gestures. 
Both these button gestures are almost the same except that the one have `@on` clause and the other do not have it.

`@if` clause takes an argument any of following; `LeftButton`, `MiddleButton`, `RightButton`, `WheelUp`, `WheelDown`, `WheelLeft`, `WheelRight`, `X1Button`, `X2Button`.

#### Button gestures (with `@on` clause)

```cs
Chrome.
@on(RightButton).
@if(WheelDown).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_CONTROL).
    ExtendedKeyDown(VK_TAB).
    ExtendedKeyUp(VK_TAB).
    ExtendedKeyUp(VK_CONTROL).
    Send(); // Send Ctrl+Tab to Chrome
});
```

#### Button gestures (without `@on` clause)

```cs
Chrome.
@if(WheelLeft).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_LMENU).
    ExtendedKeyDown(VK_LEFT).
    ExtendedKeyUp(VK_LEFT).
    ExtendedKeyUp(VK_LMENU).
    Send(); // Send Alt+Left to Chrome
});
```

### @before/@after clause
`@do` clause is just simple but does not fit to cases where there is need to hook push / release events of mouse buttons. `@before` and `@after` clauses support it. These can be written just after `@if` clause given with a **double** action mouse button any of following; `LeftButton`, `MiddleButton`, `RightButton`, `X1Button`, `X2Button`.

```cs
var Whenever = @when((ctx) => {
    return true;
});

// Convert X1Button to Win-key.
Whenever.
@if(X1Button).
@before((ctx) =>
{
    SendInput.ExtendedKeyDown(VK_LWIN);
}).
@after((ctx) =>
{
    SendInput.ExtendedKeyUp(VK_LWIN);
});
```
 
Actions given in `@before` and `@after` clauses are different from it of `@do` clause, the execution of these actions are assured.

```cs
Whenever.
@if(X2Button).
@before((ctx) =>
{
    // Assured.
}).
@do((ctx) =>
{
    // Not assured. 
    // e.g. When the gesture to timeout,
    //      this action will not be executed.
}).
@after((ctx) =>
{
    // Assured.
});
```

Note 1:  Calling `@before` and `@after` clauses from `@if` clause given with a **single** action mouse button causes compilation error because the event of these buttons can not be separated into two parts.

Note 2: Even if you specify a part of `@before`or `@after`, the other one will automatically be captured and be ignored. This is the limitation on the specification. When you want to set a hook to the one, and want to bypass the other one, you should do it by your hands.

## Config

The system default parameters can be configured by using `Config` as following:

```cs
// When moved distance of the cursor is exceeded this value, the first stroke 
// will be established.
Config.Gesture.InitialStrokeThreshold = 10;

// When moved distance of the cursor is exceeded this value, and the direction is changed,
// new stroke for new direction will be established.
Config.Gesture.StrokeDirectionChangeThreshold = 20;

// When moved distance of the cursor is exceeded this value, and the direction is not changed, 
// it will be extended.
Config.Gesture.StrokeExtensionThreshold = 10;

// Interval time for updating strokes.
Config.Gesture.WatchInterval = 10; // ms

// When stroke is not established and this period of time has passed, 
// the gesture will be canceled and the original click event will be reproduced.
Config.Gesture.Timeout = 1000; // ms

// The period of time for showing a tooltip message.
Config.UI.TooltipTimeout = 3000; // ms

// The period of time for showing a balloon message.
Config.UI.BalloonTimeout = 10000; // ms

// Binding for the position of tooltip messages.
Config.UI.TooltipPositionBinding = (point) =>
{
    var newPoint = // Create new point.
    return newPoint;
}
```

## Comamnd line interface

```
Usage:
  CreviceApp.exe [--nogui] [--script path] [--help]

  -g, --nogui       (Default: False) Disable GUI features. Set to true if you 
                    use Crevice as a CUI application.

  -n, --nocache     (Default: False) Disable user script assembly caching. 
                    Strongly recommend this value to false because compiling 
                    task consumes CPU resources every startup of application if
                    true.

  -s, --script      (Default: default.csx) Path to user script file. Use this 
                    option if you need to change the default location of user 
                    script. If given value is relative path, Crevice will 
                    resolve it to absolute path based on the default directory 
                    (%USERPROFILE%\AppData\Roaming\Crevice\CreviceApp).

  -p, --priority    (Default: High) Process priority. Acceptable values are the
                    following: AboveNormal, BelowNormal, High, Idle, Normal, 
                    RealTime.

  -V, --verbose     (Default: False) Show details about running application.

  -v, --version     (Default: False) Display product version.

  --help            Display this help screen.
```

Added in Crevice 3.0.

## Core API

### ExecutionContext
`@when` and `@do` clause take an argument of a function, and the function takes an argument of an `ExecutionContext`. 
An `ExecutionContext` will be generated each time a gesture started, and the same instance will be passed to the actions of `@when` and `@do` to guarantee that these actions to be executed on the same context.

#### Properties

##### ForegroundWindow

The window which was on the foreground when a gesture started. 
This is an instance of `WindowInfo`.

##### PointedWindow

The window which was under the cursor when a gesture started. 
This is an instance of `WindowInfo`.

### WindowInfo

`WindowInfo` is a thin wrapper of the handle of a window. This class provides properties and methods to use window handles more easily.

#### Properties
This class provides properties as following; `WindowHandle`, `ThreadId`, `ProcessId`, `WindowId`, `Text`, `ClassName`, `Parent`, `ModulePath`, `ModuleName`.

#### Methods

##### SendMessage(uint Msg, uint wParam, uint lParam)

A shortcut to win32 API `SendMessage(WindowHandle, Msg, wParam, lParam)`. 
This function returns a `long` value directly from win32 API.

##### PostMessage(uint Msg, uint wParam, uint lParam)

A shortcut to win32 API `PostMessage(WindowHandle, Msg, wParam, lParam)`.
This function returns a `bool` value directly from win32 API.

##### BringWindowToTop()

A shortcut to win32 API `BringWindowToTop(WindowHandle)`.
This function returns a `bool` value directly from win32 API.

##### FindWindowEx(IntPtr hwndChildAfter, string lpszClass, string lpszWindow)

A shortcut to win32 API `FindWindowEx(WindowHandle, hwndChildAfter, lpszClass, lpszWindow)`.
This function returns an instance of `WindowInfo`.

##### FindWindowEx(string lpszClass, string lpszWindow)

A shortcut to win32 API `FindWindowEx(WindowHandle, IntPtr.Zero, lpszClass, lpszWindow)`.
This function returns an instance of `WindowInfo`.

##### GetChildWindows()

A shortcut to win32 API `EnumChildWindows(WindowHandle, EnumWindowProc, IntPtr.Zero)`.
This function returns an instance of `IEmumerable<WindowInfo>`.

##### GetPointedDescendantWindows(Point point, Window.WindowFromPointFlags flags)

A shortcut to win32 API `ChildWindowFromPointEx(hWnd, point, flags)`.
This function recursively calls `ChildWindowFromPointEx` until the last descendant window and returns an instance of `IEmumerable<WindowInfo>`.

##### GetPointedDescendantWindows(Point point)

A shortcut to win32 API `ChildWindowFromPointEx(hWnd, point, Window.WindowFromPointFlags.CWP_ALL)`.
This function recursively calls `ChildWindowFromPointEx` until the last descendant window and returns an instance of `IEmumerable<WindowInfo>`.

##### Activate()
Brings window into the foreground and activates the window.

### SendInput

Send mouse and keyboard input events to the foreground window. 
This API provides single and multiple sending method. 
The events sent by single sending method is guaranteed to arrive to the window in order, but this does not necessarily mean it will not be interrupted by the other events. 
Multiple sending method guarantees that the events sent by it will not be interrupted by the other events.
Both methods support the same API for sending mouse and keyboard events except that multiple sending method is need to be called `Send()` at last.

```cs
SendInput.ExtendedKeyDown(VK_LWIN);
// When D key interrupts here,
// Win+D will be invoked unintentionally.
SendInput.ExtendedKeyUp(VK_LWIN); 
```

```cs
SendInput.Multiple().
ExtendedKeyDown(VK_LWIN).
ExtendedKeyUp(VK_LWIN).
Send(); // This won't be interrupted by any other input.
```

#### Mouse event
`Down`, `Up`, and `Click` events are supported for the push-release type buttons of mouse devices as following; `LeftButton`, `MiddleButton`, `RightButton`, `X1Button`, `X2Button`. For example, the provided API for `LeftButton` is `LeftDown()`, `LeftUp()` and `LeftClick()`. 

For single push type buttons, `WheelUp()`, `WheelDown()`, `WheelLeft()` and `WheelRight()` are provided. 

For move events, `Move(int dx, int dy)` and `MoveTo(int x, int y)` are provided.

##### Complete list of supported methods
- LeftDown()
- LeftUp()
- LeftClick()
- RightDown()
- RightUp()
- RightClick()
- Move(int dx, int dy)
- MoveTo(int x, int y)
- MiddleDown()
- MiddleUp()
- MiddleClick()
- VerticalWheel(short delta)
- WheelDown()
- WheelUp()
- HorizontalWheel(short delta)
- WheelLeft()
- WheelRight()
- X1Down()
- X1Up()
- X1Click()
- X2Down()
- X2Up()
- X2Click()

#### Keyboard event

A keyboard event is synthesized from a key code and two logical flags, `ExtendedKey` and  `ScanCode`. For sending `Up` and `Down` events, `KeyDown(ushort keyCode)` and `KeyUp(ushort keyCode)` are provided. 

```cs
SendInput.KeyDown(VK_A);
SendInput.KeyUp(VK_A); // Send `A` to the foreground application.
```

`ExetendedKeyDown(ushort keyCode)` and `ExtentedKeyUp(ushort keyCode)` are provided when `ExtendedKey` flag is needed to be set.

```cs
SendInput.ExetendedKeyDown(VK_LWIN);
SendInput.ExtentedKeyUp(VK_LWIN); // Send `Win` to the foreground application.
```

For four API above mentioned, combined it with `ScanCode` flag,
`KeyDownWithScanCode(ushort keyCode)`, `KeyUpWithScanCode(ushort keyCode)`, `ExtendedKeyDownWithScanCode(ushort keyCode)` and `ExtendedKeyUpWithScanCode(ushort keyCode)` are provided.

```cs
SendInput.ExtendedKeyDownWithScanCode(VK_LCONTROL);
SendInput.KeyDownWithScanCode(VK_S);
SendInput.KeyUpWithScanCode(VK_S);
SendInput.ExtendedKeyUpWithScanCode(VK_LCONTROL); // Send `Ctrl+S` with scan code to the foreground application.
```

And finally, for to support `Unicode` flag, following functions are provided; `UnicodeKeyDown(char c)`, `UnicodeKeyUp(char c)`,  `UnicodeKeyStroke(string str)`.

```cs
SendInput.UnicodeKeyDown('🍣');
SendInput.UnicodeKeyUp('🍣'); // Send `Sushi` to the foreground application.
```

Note: `keyCode` is a virtual key code. See [VirtualKeys](#virtualkeys).

##### Complete list of supported methods

- KeyDown(ushort keyCode)
- KeyUp(ushort keyCode)
- ExtendedKeyDown(ushort keyCode)
- ExtendedKeyUp(ushort keyCode)
- KeyDownWithScanCode(ushort keyCode)
- KeyUpWithScanCode(ushort keyCode)
- ExtendedKeyDownWithScanCode(ushort keyCode)
- ExtendedKeyUpWithScanCode(ushort keyCode)
- UnicodeKeyDown(char c)
- UnicodeKeyUp(char c)
- UnicodeKeyStroke(string str)

### Notification

#### Tooltip(string text)

Show tooltip message at the right bottom corner of the display on the cursor.

```cs
Tooltip("This is tooltip.");
```

#### Tooltip(string text, Point point)

Show a tooltip message at the specified position.

#### Tooltip(string text, Point point, int duration)

Show a tooltip message at the specified position for a specified period.

#### Balloon(string text)

Show a balloon message.

```cs
Balloon("This is balloon.");
```

#### Balloon(string text, string title)

Show a balloon message and a title.

#### Balloon(string text, string title, int timeout)

Show a balloon message and a title for a specified period.

#### Balloon(string text, string title, ToolTipIcon icon)

Show a balloon message, a title, and a icon.

#### Balloon(string text, string title, ToolTipIcon icon, int timeout)

Show a balloon message, a title, and a icon for a specified period.

## Extension API

### VirtualKeys

This class provides the virtual key constants. 

Note: for `VK_0` to `VK_9` and `VK_A` to `VK_Z`, this is an extension for convenience limited in this application.

To use this class, declare as following:
```cs
using static CreviceApp.WinAPI.Constants.VirtualKeys;
```

For more details, see [Virtual-Key Codes (Windows)](https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd375731(v=vs.85).aspx).

### WindowsMessages

This class provides the windows message constants. 
To use this class, declare as following:
```cs
using static CreviceApp.WinAPI.Constants.WindowsMessages;
```

For more details, see [Window Messages (Windows)](https://msdn.microsoft.com/en-us/library/windows/desktop/ff381405(v=vs.85).aspx).

### Window

`Window` is a utility static class about Windows's window.
To use this class, declare as following:
```cs
using CreviceApp.WinAPI.Window;
```

#### From(IntPtr hWnd)

This function wraps `IntPtr` and returns an instance of `WindowInfo`.

#### GetCursorPos()

Returns current position of the cursor.
This function returns an instance of `Point`.

#### WindowFromPoint(Point point)

Returns a window under the cursor.
This function returns an instance of `WindowInfo`.

#### FindWindow(string lpClassName, string lpWindowName)

Find a window matches given class name and window name.
This function returns an instance of `WindowInfo`.

#### GetTopLevelWindows()

Enumerates all windows.
This function returns an instance of `IEnumerable<WindowInfo>`.

#### GetThreadWindows(uint threadId)

Enumerates all windows belonging specified thread.
This function returns an instance of `IEnumerable<WindowInfo>`.

### VolumeControl

`VolumeControl` is a utility class about system audio volume.
To use this class, declare as following:
```cs
using CreviceApp.WinAPI.CoreAudio;
var VolumeControl = new VolumeControl();
```

#### GetMasterVolume()

Returns window's current master mixer volume.
This function returns a `float` value, within the range between 0 and 1.

#### SetMasterVolume(float value)

Sets window's current master mixer volume. The value should be within the range between 0 and 1.

## Lisence

MIT Lisense

## Author
[Rubyu](https://twitter.com/ruby_U), [Yasuyuki Nishiseki](mailto:tukigase@gmail.com)

## Latest releases (not recommended)

| Branch | Status | Download |
|--------|---------------|--------- |
| master | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/master?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/master) | [crevice.zip](https://ci.appveyor.com/api/projects/rubyu/creviceapp/artifacts/crevice.zip?branch=master&job=Configuration%3A+Release) |
| develop | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/develop?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/develop) | [crevice.zip](https://ci.appveyor.com/api/projects/rubyu/creviceapp/artifacts/crevice.zip?branch=develop&job=Configuration%3A+Release) |
