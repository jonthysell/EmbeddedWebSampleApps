# Embedded Web Sample Apps

Sample apps to test the various ways web content is embedded in native desktop apps.

## CefTester

![CEF Tester Screenshot](./screenshots/CefTester.png)

Simple .NET WPF app that loads [BrowserBench Speedometer 3.0](https://browserbench.org/Speedometer3.0) via the [Chromium Embedded Framework](https://bitbucket.org/chromiumembedded/cef/src).

### Usage
1. `dotnet run --project src\CefTester --release`
2. Click "Start"

## WebView2 Tester

![WebView2 Tester Screenshot](./screenshots/WebView2Tester.png)

Simple .NET WPF app that loads [BrowserBench Speedometer 3.0](https://browserbench.org/Speedometer3.0) via a [Microsoft Edge WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/).

### Usage
1. `dotnet run --project src\WebView2Tester --release`
2. Click "Start"

## History

This project was built by [Jon Thysell](mailto://jthysell@microsoft.com) as a part of Microsoft's March 2025 Fix/Hack/Learn event.
