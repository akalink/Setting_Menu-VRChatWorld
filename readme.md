# Settings Menu Prefab



### Requirements
    • The newest VRChat World SDK.
    • Text Mesh Pro (it will ask to import if you do not have it)

### Basic Instructions Summary:
   1. Download the Prefab file from **Releases** on the right of this. Download the most recent release.
   2. Import the prefab into your scene and go to the "Settings Menu" folder in the assets folder
   3. Drag and drop the prefab into your scene and move it to where ever you want. 
   4. If you don’t have post processing set up, use the included editor script to set it up for you.
   5. Assign objects to the Udon Component, disable any settings you don’t need. 
   

       Press the Sync Button to save your preferred settings, the next time you enter the world you will have those same settings.

### Public Methods for integrating to your own Canvas/Buttons
    • _ResetFromDefault (Resets everything to default)
    • _GetPersistenceValues (Resets everything to last save)
    • _SaveToPersistence (Saves the current options)
    • _ToggleAudioLinkButton (Toggles AudioLink On/Off)
    • _ToggleCollidersButton (Toggles Colliders On/Off)
    • _TogglePickupsButton (Toggles Pickups On/Off)
    • _ToggleChairsButton (Toggles Chairs On/Off)
    • _TogglePensButton (Toggles Pens On/Off)
    • _TogglePostProcessingButton (Toggle Post Processing On/Off)
    • _PpDarknessSlider (Adjusts the darkness/lightness created by post processing)
    • _PpBloomSlider (Adjusts the amount of bloom created by post processing)
    • _ToggleCustomButtonOne (Toggles the object(s) assigned to custom 1 on/off)
    • _ToggleCustomButtonTwo (Toggles the object(s) assigned to custom 2 on/off)
    • _CustomSlider (Adjusts the value pushed to an animator)
