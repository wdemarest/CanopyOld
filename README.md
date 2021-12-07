# development environment

# logging
From windows command prompt run adb logcat -s Unity
https://www.youtube.com/watch?v=AtOX6bXcQJE

# configuring unity for quest 2 development
https://www.youtube.com/watch?v=gGYtahQjmWQ&t=463s
https://www.youtube.com/watch?v=VdT0zMcggTQ&t=610s

Instantiate OVR Manager on XRRig

# reducing iteration time
https://developer.oculus.com/blog/unity-iteration-time-improvements/

Ctrl-K = Quick-build with / Oculus / OVR Build APK and Run
Ctrl-L = Quick build of just the scene

# Fixing "Allow Access to Data"

You have to authorize your Oculus with ADB
1) Install the Oculus ADB Drivers 2.0 and add the platform tools:
https://developer.oculus.com/downloads/package/oculus-adb-drivers/
https://www.youtube.com/watch?v=cD6zsHQdoXY
2) Unplug your Quest from your PC's USB. Wait 3 seconds. Plug your Quest back in again.
3) Now enter 'adb devices' in your command line prompt. It should show your device and behind it 'unauthorized'.
4) Mount your Quest (put it on your head). Now you should see that other message that allows you to ALWAYS access your Quest through your PC's USB connection.
5) For the second message (which you have been seeing thousands of times until now and which drove you to near insanity) just click 'Don't show again'.
6) Now enter 'adb devices' again and if correct, your Quest is now 'authorized'.
7) Unplug your Quest and plug it in again. You should get no prompt in your Quest and 'adb devices' should still should your Quest as a registered and authorized device.
8) If any of this fails, reboot your Quest and try again.
9) If it still doesn't work for you then this tutorial doesn't work for you, sorry