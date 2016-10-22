# AudioSwitch-CLI

This project is forked from [sirWest/AudioSwitch](https://github.com/sirWest/AudioSwitch). 

The application in in this repo offers the functionality of AudioSwitch with all of its graphical components removed.
AudioSwitch allows you to change the default audio device, select input or output devices, and change their volume. 
AudioSwitch-CLI does not run in the background. The application terminates as soon as the arguement is executed, so 
nothing will appear in your system tray as AudioSwitch does. This fork is targeted at AutoHotKey users like myself 
who are looking for a good way to bind a hotkey that allows them to quickly change the active audio device at the 
push of a button.

Get started by typing `AudioSwitch-CLI.exe /help`

Sample AutoHotKey script for changing audio devices

```AutoHotKey
; Change devices and play a sound on the new device to confirm
ActivateAudioDevice(deviceName, outputDevice:=true)
{
	type := outputDevice ? "-o" : "-i"
	sound := "D:\MiscPrograms\AudioSwitch\" . deviceName . ".wav"
	RunWait D:\MiscPrograms\AudioSwitch\AudioSwitch-CLI.exe %type% -s %deviceName%, Hide
	SoundPlay %sound%, wait
}

F5::ActivateAudioDevice("Speakers")
F6::ActivateAudioDevice("Headset")

#F5::ActivateAudioDevice("DesktopMic", false)
#F6::ActivateAudioDevice("HeadsetMic", false)

```