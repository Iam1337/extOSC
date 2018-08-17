# extOSC - Open Sound Control Protocol for Unity

Created by V. Sigalkin (ext)

### What Is extOSC?
extOSC (Open Sound Control Protocol) is a tool dedicated to simplify creation of applications with OSC protocol usage in Unity (Unity3d). Supported platforms are PC, Mac and Linux / iOS / tvOS / Android / Universal Windows Platform (UWP) and other.

Also extOSC  is available on the [Unity Asset Store](http://u3d.as/ADA) for free.

**Support Unity versions**: Unity 2017.1.4. or higher

### Release Notes:

You can read release notes in [versions.txt](Assets/extOSC/versions.txt) file.

### Features:

- **OSC with coding**<br>
Massive implementation of methods, utils, and other for simple operations with OSС through code.
- **OSC without coding**<br>
With extOSC components you can easily implement basic program logic in your application without coding. Components for basic functions: sending, receiving, mapping.
- **OSC Console**<br>
New separated console for tracking sending and receiving OSC packages:
[Video](https://www.youtube.com/watch?v=ihVw6v2Meto)
- **OSC Debug**<br>
New easy to use tool for debugging sending and receiving OSC packages:
[Video](https://www.youtube.com/watch?v=PU2oSwbbliE)
- **Map OSC packages**<br>
OSCMapping allows you to map float values:
[Video](https://www.youtube.com/watch?v=73Hjglgx6ss)
- **UI**<br>
Four new UI-components for easy creation of remote control apps with OSC-protocols:
[Video](https://www.youtube.com/watch?v=phV4Y8Go0_U)
- **Addresses with masks support**<br>
You can bind addresses with mask (for instance: *"/lights/\*/value"*)
- **Auto pack bundle**<br>
extOSC will bundle your packages for optimisation purposes

**And also:**

- Every data type support *(include **Array** and **MIDI**)*
- Compatible with OS X, Windows, iOS & Android, Windows Store
- Tested with TouchOSC, VVOpenSource, OpenFrameworks and others frameworks and applications
- Examples

**And much more**

### Examples:
**Create OSC Transmitter**<br>
```c#
// Creating a transmitter.
var transmitter = gameObject.AddComponent<OSCTransmitter>();

// Set remote host address.
transmitter.RemoteHost = "127.0.0.1";    

// Set remote port;
transmitter.RemotePort = 7001;         
```
Or you can simple create **OSCTransmitter component** in Unity editor, or use **Create/extOSC/OSC Manager** in Hierarchy window.

**Send OSCMessage**<br>
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

**Create OSC Receiver**<br>
```c#
// Creating a receiver.
var receiver = gameObject.AddComponent<OSCReceiver>(); 

// Set local port.
receiver.LocalPort = 7001;            
```
Or you can simple create **OSCReceiver component** in Unity editor, or use **Create/extOSC/OSC Manager** in Hierarchy window.

**Receive OSCMessage**<br>
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

**Other examples you can find in [Examples](Assets/extOSC/Examples) folder.**

### Installation:
Just copy the [Assets/extOSC](Assets/extOSC) folder into your Assets directory within your Unity project.

### Author Contacts:
\> [telegram.me/iam1337](http://telegram.me/iam1337) <br>
\> [ext@iron-wall.org](mailto:ext@iron-wall.org)

### License
This project is under the MIT License.
