
using System;
using System.Text;
using TMPro;
using UdonSharp;
using UnityEngine;

using UnityEngine.UI;

using VRC.SDK3.Persistence;
using VRC.SDKBase;


namespace akaUdon
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SettingsMenu : UdonSharpBehaviour
    {
        #region instance variables
        [SerializeField] Color _OffColor = Color.grey;
        private readonly string settings = "akalink-Settings";
        private int defaultstate;

        [Header("These are custom Options, feel free to edit the code without issue")]
        //Custom Options after this comment


        
        //Custom code ends after this comment
        [SerializeField] private Slider _customSlider;
        private float _customFloatState;
        [SerializeField] private Image _customBoolStateButtonOne;
        [SerializeField] private Color _customBoolStateOnColorOne = Color.white;
        [SerializeField] private GameObject[] _customGameObjects1;
        private bool customBoolState1;
        [SerializeField] private Image _customBoolStateButtonTwo;
        [SerializeField] private Color _customBoolStateOnColorTwo = Color.white;
        private bool customBoolState2;
        [SerializeField] private GameObject[] _customGameObjects2;
        

        [Header("Related to the Post Processing Sliders and Buttons")]
        [SerializeField] private Slider _sliderLight;
        private bool ppState;
        private float lightState;
        private readonly string lightnessAnim = "Light";
        [SerializeField] private Slider _sliderBloom;
        private float bloomSate;
        private readonly string bloomAnim = "Bloom";
        [SerializeField] private Animator _ppAnimator;
        [SerializeField] Color _ppOnColor = Color.white;
        private GameObject _ppObject;
        [SerializeField] private Image _postProcessingButton;
        private Image[] _sliderImages;

        [Header("Put AudioLink Here")] [SerializeField]
        private Color _audioLinkOnColor = Color.white;
        [SerializeField] private GameObject _audiolink;
        [SerializeField] private Image _audioLinkButton;
        private bool aLState;

        [Header("Any colliders you want to toggle go here")] [SerializeField]
        private Color _collidersOnColor = Color.green;
        [SerializeField] private Collider[] _colliders;
        [SerializeField] private Image _colliderButton;
        private bool colsState;

        [Header("Any pickup objects you want to toggle go here")] 
        [SerializeField] private Color _pickUpsOnColor = Color.white;
        [SerializeField] private GameObject[] _pickUpObjects;
        [SerializeField] private Image _pickUpButton;
        private bool pickupState;
        
        [Header("Any chairs to toggle go here")]
        [SerializeField] private Color _chairsOnColor = Color.white;
        [SerializeField] private Collider[] _chairColliders;
        [SerializeField] private Image _chairButton;
        private bool chairState;
        
        [Header("The pens you want to toggle go here")] 
        [SerializeField] private Color _pensOnColor = Color.white;
        [SerializeField] private GameObject _pensObject;
        [SerializeField] private Image _pensButton;
        private bool pensState;
        

        [Header("The save icon for persistence")] [SerializeField]
        private Color _saveButtonOnColor = Color.cyan;
        [SerializeField] private Image _saveButton;
        private bool _savedToPersistence;
        [SerializeField] private TextMeshProUGUI _persistenceText;
        
        #endregion


        void Start()
        {
            //assign state and object of post processing
            if (Utilities.IsValid(_ppAnimator))
            {
                _ppObject = _ppAnimator.gameObject;
                ppState = _ppObject.activeSelf;
            }
            
            Image[] sl = new Image[0];
            Image[] bl = new Image[0];

            if (Utilities.IsValid(_sliderLight))
            {
                sl = _sliderLight.gameObject.GetComponentsInChildren<Image>();
                lightState = _sliderLight.value;
            }


            if (Utilities.IsValid(_sliderBloom))
            {
                bl = _sliderBloom.gameObject.GetComponentsInChildren<Image>();
                bloomSate = _sliderBloom.value;
            }

            _sliderImages = new Image[sl.Length+bl.Length];

            for (int i = 0; i < _sliderImages.Length; i++)
            {
                if (i < sl.Length)
                {
                    _sliderImages[i] = sl[i];
                }
                else
                {
                    _sliderImages[i] = bl[i-sl.Length];
                }
            }
            
            //assigns buttons

            if (_colliders.Length > 0 && Utilities.IsValid(_colliders[0]))
            {
                colsState = _colliders[0].enabled;
                ToggleHelperColliders(colsState, _colliders, _colliderButton, _collidersOnColor);
            }

            if (_pickUpObjects.Length > 0 && Utilities.IsValid(_pickUpObjects[0]))
            {
                pickupState = _pickUpObjects[0].activeSelf;
                TogglePickups();
            }
            

            if (_chairColliders.Length > 0 && Utilities.IsValid(_chairColliders[0]))
            {
                chairState = _chairColliders[0].enabled;
                ToggleChairs();
            }

            if (Utilities.IsValid(_audiolink))
            {
                aLState = _audiolink.activeSelf;
                ToggleAudioLink();
            }

            if (Utilities.IsValid(_pensObject))
            {
                pensState = _pensObject.activeSelf;
                TogglePens();
            }

            if (!Utilities.IsValid(_ppAnimator))
            {
                Debug.LogError($"{DateTime.Now} {gameObject.name}-The Post Processing Animator is not assigned");
            }
            SaveButtonAndColorDisable();

            defaultstate = ConvertToBinary();
        }
        

        #region PostProcessing

        public void _PpDarknessSlider()
        {
            lightState = _sliderLight.value;
            PpDarkness();
        }
        private void PpDarkness()
        {
            _ppAnimator.SetFloat(lightnessAnim, lightState);
            SaveButtonAndColorDisable();
        }

        public void _PpBloomSlider()
        {
            bloomSate = _sliderBloom.value;
            PpBloom();
        }

        private void PpBloom()
        {
            _ppAnimator.SetFloat(bloomAnim, bloomSate);
            SaveButtonAndColorDisable();
        }

        public void _TogglePostProcessingButton()
        {
            ppState = !ppState;
            TogglePostProcessing();
            
        }

        private void TogglePostProcessing()
        {
            _ppObject.SetActive(ppState);
            if (ppState)
            {
                if (Utilities.IsValid(_postProcessingButton))
                {
                    _postProcessingButton.color = Color.white;
                    foreach (Image i in _sliderImages)
                    {
                        i.color = _ppOnColor;
                    }
                }
   
            }
            else
            {
                if (Utilities.IsValid(_postProcessingButton))
                {
                    _postProcessingButton.color = _OffColor;
                    foreach (Image i in _sliderImages)
                    {
                        i.color = _OffColor;
                    }
                }

            }
            SaveButtonAndColorDisable();
        }
        #endregion

        #region AudioLink
        public void _ToggleAudioLinkButton()
        {
            if (!Utilities.IsValid(_audiolink))
            {
                Debug.LogError($"{DateTime.Now} {gameObject.name}-The AudioLink toggle is not assigned properly, make sure you assigned items to toggle");
                return;
            }
            aLState = !aLState;
            ToggleAudioLink();
        }
        
        private void ToggleAudioLink()
        {
            if(Utilities.IsValid(_audiolink)) _audiolink.SetActive(aLState);
            if (Utilities.IsValid(_audioLinkButton)) _audioLinkButton.color = aLState ? _audioLinkOnColor : _OffColor;
            SaveButtonAndColorDisable();
        }
        #endregion

        #region Colliders

        public void _ToggleCollidersButton()
        {
            if (_colliders.Length < 1)
            {
                Debug.LogError($"{DateTime.Now} {gameObject.name}-The Colliders toggle is not assigned properly, make sure you assigned items to toggle");
                return;
            }
            colsState = !colsState;
            
            ToggleHelperColliders(colsState, _colliders, _colliderButton, _collidersOnColor);
            // ToggleColliders();
        }

        // private void ToggleColliders()
        // {
        //     foreach (Collider c in _colliders)
        //     {
        //         if(Utilities.IsValid(c)) c.enabled = colsState;
        //     }
        //
        //     if(Utilities.IsValid(_colliderButton)) _colliderButton.color = colsState ? _collidersOnColor : Color.gray;
        //     SaveButtonAndColorDisable();
        //
        // }

        #endregion

        #region Pickups

        public void _TogglePickupsButton()
        {
            if (_pickUpObjects.Length < 1 )
            {
                Debug.LogError($"{DateTime.Now} {gameObject.name}-The Pickups toggle is not assigned properly, make sure you assigned items to toggle");
                return;
            }
            pickupState = !pickupState;
            TogglePickups();
        }

        private void TogglePickups()
        {
            foreach (GameObject obj in _pickUpObjects)
            {
                if(Utilities.IsValid(obj)) obj.SetActive(pickupState);
                
            }

            if(Utilities.IsValid(_pickUpButton))_pickUpButton.color = pickupState ? _pickUpsOnColor : _OffColor;
            SaveButtonAndColorDisable();
        }
        
        #endregion

        #region chairs/stations

        public void _ToggleChairsButton()
        {
            if (_chairColliders.Length < 1 || !Utilities.IsValid(_chairColliders[0]))
            {
                Debug.LogError($"{DateTime.Now} {gameObject.name}-The Chairs toggle is not assigned properly, make sure you assigned items to toggle");
                return;
            }
            chairState = !chairState;
            ToggleChairs();
        }

        private void ToggleChairs()
        {
            foreach (Collider ch in _chairColliders)
            {
                if(Utilities.IsValid(ch)) ch.enabled = chairState;
            }

            if(Utilities.IsValid(_chairButton))_chairButton.color = chairState ? _chairsOnColor : _OffColor;
            SaveButtonAndColorDisable();
        }
        

        #endregion
        
        #region PenToggle

        public void _TogglePensButton()
        {
            if (!Utilities.IsValid(_pensObject))
            {
                Debug.LogError($"{DateTime.Now} {gameObject.name}-The Pens toggle is not assigned properly, make sure you assigned items to toggle");
                return;
            }
            pensState = !pensState;
            TogglePens();
        }

        private void TogglePens()
        {
            if(Utilities.IsValid(_pensObject)) _pensObject.SetActive(pensState);
            if(Utilities.IsValid(_pensButton))_pensButton.color = pensState ? _pensOnColor : _OffColor;
            SaveButtonAndColorDisable();
        }
        #endregion

        #region Custom Options

        public void _ToggleCustomButtonOne()
        {
            customBoolState1 = !customBoolState1;
            ToggleCustomOne();
        }

        private void ToggleCustomOne()
        {
            //Custom code starts here

            foreach (GameObject obj in _customGameObjects1)
            {
                if(Utilities.IsValid(obj)) obj.SetActive(obj);
            }
            
            //Custom Code ends here
            if(Utilities.IsValid(_customBoolStateButtonOne))_customBoolStateButtonOne.color = customBoolState1 ? _customBoolStateOnColorOne : _OffColor;
            SaveButtonAndColorDisable();
        }
        
        public void _ToggleCustomButtonTwo()
        {
            customBoolState2 = !customBoolState2;
            ToggleCustomTwo();
        }

        private void ToggleCustomTwo()
        {
            //Custom code starts here
            
            foreach (GameObject obj in _customGameObjects2)
            {
                if(Utilities.IsValid(obj)) obj.SetActive(obj);
            }
            
            //Custom Code ends here
            if(Utilities.IsValid(_customBoolStateButtonTwo))_customBoolStateButtonTwo.color = customBoolState2 ? _customBoolStateOnColorTwo : _OffColor;
            SaveButtonAndColorDisable();
        }
        

        #endregion

        private void ToggleHelperColliders(bool state, Collider[] colliders, Image button, Color onColor)
        {
            foreach (Collider c in  colliders)
            {
                if (Utilities.IsValid(c)) c.enabled = state;
            }
             ToggleImageHelper(state, button, onColor);
        }

        private void ToggleImageHelper(bool state, Image button, Color onColor)
        {
            if (Utilities.IsValid(button)) button.color = state ? onColor : _OffColor;
            SaveButtonAndColorDisable();
        }
        
        
        //Everything in this region is used for handling persistence and binary to int conversion. I high recommend you do not edit this code.
        #region Saving Persistance 
        
        public void _ResetFromDefault()
        {
            ConvertFromInt(defaultstate);
        }
        
        public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
        {
            Debug.Log($"{DateTime.Now} {gameObject.name}-Infos on persistence load is {infos[0].State}");
            _GetPersistenceValues();
        }
        
        public void _SaveToPersistence()
        {
            //From the state values of the button and slider, we convert the values to binary, 8 spaces for each slider, and 1 for each button.
            //This binary value will then be changed into a 32 bit int value, which will be stored to persistance.
            Debug.Log($"{DateTime.Now} {gameObject.name}-Saving");
            
            int pInt = ConvertToBinary();

            PlayerData.SetInt(settings, pInt);
            
            SaveButtonAndColorEnabled();
        }

        private int ConvertToBinary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Capacity = 32;
            //We convert the 0-1 float value of the slider to a int. It will always take up 8 spaces.
            int sliderValueAsInt = (int) (lightState * 255); //256 creates int overflow, 255 is max value of unsigned 8 bit int 0-255
            sb.Append(ConvertToNeededLength(Convert.ToString(sliderValueAsInt, 2),8));
            sliderValueAsInt = (int) (bloomSate * 255);
            sb.Append(ConvertToNeededLength(Convert.ToString(sliderValueAsInt, 2),8));
            //add 3rd slider as temp todo
            sb.Append("00000000");
            //end add 3rd slider as temp
            //Appends 0 or 1 to the string depending on the state of the button.
            sb.Append(ppState ? "1" : "0");
            sb.Append(aLState ? "1" : "0");
            sb.Append(colsState ? "1" : "0");
            sb.Append(pickupState ? "1" : "0");
            sb.Append(chairState ? "1" : "0");
            sb.Append(pensState ? "1" : "0");
            sb.Append(customBoolState1 ? "1" : "0");
            sb.Append(customBoolState2 ? "1" : "0");
            return Convert.ToInt32(sb.ToString(), 2);
        }

        public void _GetPersistenceValues()
        {
            Debug.Log($"{DateTime.Now} {gameObject.name}-Getting Persistent Values");
            int pInt = PlayerData.GetInt(Networking.LocalPlayer, settings);

            ConvertFromInt(pInt);
            SaveButtonAndColorEnabled();
        }

        private void ConvertFromInt(int state)
        {
            string stateStr = ConvertToNeededLength(Convert.ToString(state, 2),32);

            stateStr = ConvertToNeededLength(stateStr, 32);
            
            string tempStr = stateStr.Substring(0, 8);

            
            lightState = (float) Convert.ToInt32(tempStr, 2) / 255;

            PpDarkness();
            _sliderLight.value = lightState;

            tempStr = stateStr.Substring(8, 8);
            
            bloomSate = (float) Convert.ToInt32(tempStr, 2) / 255;
            PpBloom();
            _sliderBloom.value = bloomSate;

            
            //todo get values for optional slider

            ppState = stateStr[24] == '1';
            TogglePostProcessing();
            aLState = stateStr[25] == '1';
            ToggleAudioLink();
            colsState = stateStr[26] == '1';
            ToggleHelperColliders(colsState, _colliders, _colliderButton, _collidersOnColor);
            pickupState= stateStr[27] == '1';
            TogglePickups();
            chairState = stateStr[28] == '1';
            ToggleChairs();
            pensState = stateStr[29] == '1';
            TogglePens();
            customBoolState1 = stateStr[30] == '1';
            customBoolState2 = stateStr[31] == '1';
        }

        //This is a utility method, it will guarantee any string is the exact length needed.
        private string ConvertToNeededLength(string value, int length)
        {
            StringBuilder sb = new StringBuilder(value, length);
            
            
            while (sb.Length < length) 
            {
                sb.Insert(0, "0");
            }

            return sb.ToString();
        }
        
        #endregion

        #region save button visuals

        //This could easily be combined to one method, but having them be separate I believe makes visually parsing the code easier. 
        private void SaveButtonAndColorDisable()
        {
            if(Utilities.IsValid(_saveButton))_saveButton.color = _OffColor;
            _savedToPersistence = false;
            _persistenceText.text = "Not\nSynced";
        }

        private void SaveButtonAndColorEnabled()
        {
            if(Utilities.IsValid(_saveButton))_saveButton.color = _saveButtonOnColor;
            _savedToPersistence = true;
            _persistenceText.text = "Synced";
        }

        #endregion
        
        

        
    }
}