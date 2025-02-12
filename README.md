# extOSC - Open Sound Control Protocol for Unity

Created by [iam1337](https://github.com/iam1337)

![](https://img.shields.io/badge/unity-2018.3%20or%20later-green.svg)
[![⚙ Build and Release](https://github.com/Iam1337/extOSC/actions/workflows/ci.yml/badge.svg)](https://github.com/Iam1337/extOSC/actions/workflows/ci.yml)
[![openupm](https://img.shields.io/npm/v/com.iam1337.extosc?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.iam1337.extosc/)
[![](https://img.shields.io/github/license/iam1337/extOSC.svg)](https://github.com/Iam1337/extOSC/blob/master/LICENSE)
[![semantic-release: angular](https://img.shields.io/badge/semantic--release-angular-e10079?logo=semantic-release)](https://github.com/semantic-release/semantic-release)

### Table of Contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Examples](#examples)
- - [Create OSC Transmitter](#create-osc-transmitter)
- - [Send OSCMessage](#send-oscmessage)
- - [Create OSC Receiver](#create-osc-receiver)
- - [Receive OSCMessage](#receive-oscmessage)
- - [Get value from OSCMessage](#get-value-from-oscmessage)
- [Extensions](#extensions)
- [Author Contacts](#author-contacts)

## Introduction
extOSC (Open Sound Control Protocol) is a tool dedicated to simplify creation of applications with OSC protocol usage in Unity (Unity3d). Supported platforms are PC, Mac and Linux / iOS / tvOS / Android / Universal Windows Platform (UWP) and other.

Also extOSC  is available on the [Unity Asset Store](http://u3d.as/ADA) for free.

### Features:

- **OSC with coding**<br>
Massive implementation of methods, utils, and other for simple operations with OSС through code.
- **OSC without coding**<br>
With extOSC components you can easily implement basic program logic in your application without coding. Components for basic functions: sending, receiving, mapping.
- **OSC Console**<br>
New separated console for tracking sending and receiving OSC packages:
[Video](https://www.youtube.com/watch?v=ihVw6v2Meto).
- **OSC Debug**<br>
New easy to use tool for debugging sending and receiving OSC packages:
[Video](https://www.youtube.com/watch?v=PU2oSwbbliE).
- **Map OSC packages**<br>
OSCMapping allows you to map float values:
[Video](https://www.youtube.com/watch?v=73Hjglgx6ss).
- **UI**<br>
Four new UI-components for easy creation of remote control apps with OSC-protocols:
[Video](https://www.youtube.com/watch?v=phV4Y8Go0_U).
- **Addresses with masks support**<br>
You can bind addresses with mask (for instance: *"/lights/\*/value"*).
- **Auto pack bundle**<br>
extOSC will bundle your packages for optimisation purposes.

**And also:**

- Every data type support *(include **Array** and **MIDI**)*
- Compatible with OS X, Windows, iOS & Android, Windows Store (**Hololens** and **Hololens 2**)
- Tested with TouchOSC, VVOpenSource, OpenFrameworks and others frameworks and applications
- Examples

**And much more**

## Installation
**Old school**

Just copy the [Assets/extOSC](Assets/extOSC) folder into your Assets directory within your Unity project, or [download latest extOSC.unitypackage](https://github.com/iam1337/extOSC/releases).

**OpenUPM**

Via [openupm-cli](https://github.com/openupm/openupm-cli):<br>
```
openupm add com.iam1337.extosc
```

Or if you don't have it, add the scoped registry to manifest.json with the desired dependency semantic version:
```
"scopedRegistries": [
	{
		"name": "package.openupm.com",
		"url": "https://package.openupm.com",
		"scopes": [
			"com.iam1337.extosc",
		]
	}
],
"dependencies": {
	"com.iam1337.extosc": "1.19.7"
}
```

**Package Manager**

Project supports Unity Package Manager. To install the project as a Git package do the following:

1. In Unity, open **Window > Package Manager**.
2. Press the **+** button, choose **"Add package from git URL..."**
3. Enter "https://github.com/iam1337/extOSC.git#upm" and press Add.

## Examples
### Create OSC Transmitter
```c#
// Creating a transmitter.
var transmitter = gameObject.AddComponent<OSCTransmitter>();

// Set remote host address.
transmitter.RemoteHost = "127.0.0.1";    

// Set remote port;
transmitter.RemotePort = 7001;         
```
Or you can simple create **OSCTransmitter component** in Unity editor, or use **Create/extOSC/OSC Manager** in Hierarchy window.

### Send OSCMessage
```c#
// Create message
var message = new OSCMessage("/message/address");

// Populate values.
message.AddValue(OSCValue.String("Hello, world!"));
message.AddValue(OSCValue.Float(1337f));

// Send message
transmitter.Send(message);      
```
Or you can use any *extOSC/Transmitter* components.

### Create OSC Receiver
```c#
// Creating a receiver.
var receiver = gameObject.AddComponent<OSCReceiver>(); 

// Set local port.
receiver.LocalPort = 7001;            
```
Or you can simple create **OSCReceiver component** in Unity editor, or use **Create/extOSC/OSC Manager** in Hierarchy window.

### Receive OSCMessage
Bind method to special address. In address argument you can use masks like: *"/message/\*"*
```c#
// Bind "MessageReceived" method to special address.
receiver.Bind("/message/address", MessageReceived);     
```
Method realization:
```c#
protected void MessageReceived(OSCMessage message)
{
	// Any code...
	Debug.Log(message);
}
```
Or you can use any *extOSC/Receiver* components.<br>

### Get value from OSCMessage
You have two ways to get the value from the message.
```c#
var value = message.Values[0].FloatValue;
// Any code...
Debug.Log(value);
```
or
```c#
if (message.ToFloat(out var value))  
{
	// Any code...
	Debug.Log(value);
}
```

**Other examples you can find in [Examples](Assets/extOSC/Examples) folder.**

## Extensions

List of useful repositories to help make extDebug easier to use:

- [extOSC.InEditor](https://github.com/Iam1337/extOSC.InEditor) - Extension to support extOSC behaviour in Unity Editor

## SAST Tools

[PVS-Studio](https://pvs-studio.com/pvs-studio/?utm_source=website&utm_medium=github&utm_campaign=open_source) - static analyzer for C, C++, C#, and Java code.

## Author Contacts
\> [telegram.me/iam1337](http://telegram.me/iam1337) <br>
\> [ext@iron-wall.org](mailto:ext@iron-wall.org)

## License
This project is under the MIT License.
