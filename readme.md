# Settings Menu Prefab

If you find a bug please report to **mcphersonsound** on discord with steps to recreate the issue.

## Requirements
    • The newest VRChat World SDK.
    • Text Mesh Pro (it will ask to import if you do not have it)

### Demo Scene Requirements
    • Audiolink, get it from the creator companion

## Basic Instructions Summary:
   1. Download the Prefab file from **Releases** on the right of this. [Download the most recent release.](https://github.com/akalink/Setting_Menu-VRChatWorld/releases)
   2. Import the prefab into your scene and go to the "Settings Menu" folder in the assets folder
   3. Drag and drop the prefab into your scene and move it to wherever you want. Do not double click the prefab.
        ![a image of the location of the prefab](https://github.com/akalink/Setting_Menu-VRChatWorld/blob/develop/documentationAssets/Prefabfile.png?raw=true)
   4. If you don’t have post processing set up, use the included editor script to set it up for you.
       - Go to Tools -> Settings Menu -> Add the Post Processing Layer
  
        ![a image of the location of the editor script option](https://github.com/akalink/Setting_Menu-VRChatWorld/blob/develop/documentationAssets/EditorScript.PNG?raw=true)
   5. Assign objects to the Udon Component. To assign, drag and drop any objects from the Hierarchy to the name of the setting in the Settings Menu. For example, drag any chairs you want to toggle to "Chair Colliders"
        ![a image of the drag and drop functionality](https://github.com/akalink/Setting_Menu-VRChatWorld/blob/develop/documentationAssets/DragDrop.png?raw=true)
   6. Disable any settings you don’t need. 
          
    Press the Save Button to save your preferred settings, the next time you enter the world you will have those same settings.


## Public Methods for integrating to your own Canvas/Buttons
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

## FAQ
 - Q: None of the post processing options are doing anything.
 - A: Check if a camera assigned to the scene descriptor, and if that camera has a post processing layer. Post processing does not work without either of those two things.
  
 - Q: How do I assign objects to the Settings Menu prefab?
 - A: Drag the objects into respective slots in the Udon component. The inspector can be locked to prevent keep the desired window active. Check the demo scene to see how things are assigned.
  
 - Q: Can I upload the demo scene as my own world and take pieces of it for my own world?
 - A: Yes, credit is optional.
  