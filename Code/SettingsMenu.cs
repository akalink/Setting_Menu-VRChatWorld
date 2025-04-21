
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
        [Header("This effects all options, choose what color to tint the \"off setting\"\nand choose if that is on at all")]
        #region instance variables
        [SerializeField] Color _OffColor = Color.grey;
        [SerializeField] private bool _toggleColor = true;
        private readonly string settings = "akalink-Settings";
        private int defaultstate;
        private readonly string savedText = "Saved";
        private readonly string notSavedText = "Not\nSaved";

        [Header("These are custom Options, feel free to edit the code without issue")]
        [SerializeField] private Slider _customSlider;
        private float customFloatState;
        [SerializeField] private Animator _customAnimator;
        [SerializeField] private string _customFloatName;
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
        private bool inAndroid;
        

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

        #region initalization
        void Start()
        {
            InitializePostProcessing();
            InitializeOther();
                
            SaveButtonAndColorDisable();

            defaultstate = ConvertToBinary();
        }

        private void InitializePostProcessing()
        {
            #if ANDROID
            inAndroid = true;
            #endif
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

            if (inAndroid)
            {
                ppState = false;
            }
            else
            {
                if(Utilities.IsValid(_ppObject)) ppState = _ppObject.activeSelf;
            }
            
            TogglePostProcessing();

        }

        private void InitializeOther()
        {
         //assigns buttons

            if (_colliders.Length > 0 && Utilities.IsValid(_colliders[0]))
            {
                colsState = _colliders[0].enabled;
                ToggleCollidersHelper(colsState, _colliders, _colliderButton, _collidersOnColor);
            }

            if (_pickUpObjects.Length > 0 && Utilities.IsValid(_pickUpObjects[0]))
            {
                pickupState = _pickUpObjects[0].activeSelf;
                ToggleObjectsHelper(pickupState, _pickUpObjects, _pickUpButton, _pickUpsOnColor);
            }
            

            if (_chairColliders.Length > 0 && Utilities.IsValid(_chairColliders[0]))
            {
                chairState = _chairColliders[0].enabled;
                ToggleCollidersHelper(chairState, _chairColliders, _chairButton, _chairsOnColor);
            }

            if (Utilities.IsValid(_audiolink))
            {
                aLState = _audiolink.activeSelf;
                ToggleObjectsHelper(aLState, new []{_audiolink}, _audioLinkButton, _audioLinkOnColor);
            }

            if (Utilities.IsValid(_pensObject))
            {
                pensState = _pensObject.activeSelf;
                ToggleObjectsHelper(pensState, new []{_pensObject}, _pensButton, _pensOnColor);
            }

            if (_customGameObjects1.Length > 0 && Utilities.IsValid(_customGameObjects1[0]))
            {
                customBoolState1 = _customGameObjects1[0].activeSelf;
                ToggleObjectsHelper(customBoolState1, _customGameObjects1, _customBoolStateButtonOne, _customBoolStateOnColorOne);
            }
            
            if (_customGameObjects2.Length > 0 && Utilities.IsValid(_customGameObjects2[0]))
            {
                customBoolState2 = _customGameObjects2[0].activeSelf;
                ToggleObjectsHelper(customBoolState2, _customGameObjects2, _customBoolStateButtonTwo, _customBoolStateOnColorTwo);
            }
            
            if (!Utilities.IsValid(_ppAnimator))
            {
                Debug.LogError($"{DateTime.Now} {gameObject.name}-The Post Processing Animator is not assigned");
            }   
        }
        #endregion

        #region PostProcessing

        public void _PpDarknessSlider()
        {
            if(inAndroid) return;
            lightState = _sliderLight.value;
            if(Utilities.IsValid(_ppAnimator)) SetAnimatorValue(_ppAnimator, lightnessAnim, lightState);
        }
        
        public void _PpBloomSlider()
        {
            if(inAndroid) return;
            bloomSate = _sliderBloom.value;
            if(Utilities.IsValid(_ppAnimator)) SetAnimatorValue(_ppAnimator, bloomAnim, bloomSate);
        }
        


        public void _TogglePostProcessingButton()
        {
            if(inAndroid) return;
            ppState = !ppState;
            TogglePostProcessing();
        }

        private void TogglePostProcessing()
        {
            if(Utilities.IsValid(_ppAnimator)) _ppObject.SetActive(ppState);

            if (Utilities.IsValid(_postProcessingButton))
            {
                if (ppState)
                {
                    _postProcessingButton.color = _ppOnColor;
                    foreach (Image i in _sliderImages)
                    {
                        i.color = _ppOnColor;
                    }
                }
                else
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

            ToggleObjectsHelper(aLState, new []{_audiolink}, _audioLinkButton, _audioLinkOnColor);
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
            
            ToggleCollidersHelper(colsState, _colliders, _colliderButton, _collidersOnColor);
        }

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
            ToggleObjectsHelper(pickupState, _pickUpObjects, _pickUpButton, _pickUpsOnColor);
        }
        
        #endregion

        #region chairs/stations

        public void _ToggleChairsButton()
        {
            if (_chairColliders.Length < 1)
            {
                Debug.LogError($"{DateTime.Now} {gameObject.name}-The Chairs toggle is not assigned properly, make sure you assigned items to toggle");
                return;
            }
            chairState = !chairState;
            ToggleCollidersHelper(chairState, _chairColliders, _chairButton, _chairsOnColor);
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
            ToggleObjectsHelper(pensState, new []{_pensObject}, _pensButton, _pensOnColor);
            
        }

        #endregion

        #region Custom Options

        public void _ToggleCustomButtonOne()
        {
            customBoolState1 = !customBoolState1;
            ToggleObjectsHelper(customBoolState1, _customGameObjects1, _customBoolStateButtonOne, _customBoolStateOnColorOne);
        }

        
        public void _ToggleCustomButtonTwo()
        {
            customBoolState2 = !customBoolState2;
            ToggleObjectsHelper(customBoolState2, _customGameObjects2, _customBoolStateButtonTwo, _customBoolStateOnColorTwo);
        }

        public void _CustomSlider()
        {
            customFloatState = _customSlider.value;
            if(Utilities.IsValid(_customAnimator)) SetAnimatorValue(_customAnimator, _customFloatName, customFloatState);
            
        }

        #endregion

        #region helpers

        private void ToggleCollidersHelper(bool state, Collider[] colliders, Image button, Color onColor)
        {
            SaveButtonAndColorDisable();
            foreach (Collider c in  colliders)
            {
                if (Utilities.IsValid(c)) c.enabled = state;
            }
            ToggleImageHelper(state, button, onColor);
        }

        private void ToggleObjectsHelper(bool state, GameObject[] objects, Image button, Color onColor)
        {
            SaveButtonAndColorDisable();
            foreach (GameObject obj in objects)
            {
                if(Utilities.IsValid(obj)) obj.SetActive(state); 
            }
            ToggleImageHelper(state, button, onColor);
        }
        

        private void ToggleImageHelper(bool state, Image button, Color onColor)
        {
            if(!_toggleColor) return;
            if (Utilities.IsValid(button)) button.color = state ? onColor : _OffColor;
        }
        
        private string ConvertToNeededLength(string value, int length)
        {
            StringBuilder sb = new StringBuilder(value, length);
            
            
            while (sb.Length < length) 
            {
                sb.Insert(0, "0");
            }

            return sb.ToString();
        }
        
        private void SetAnimatorValue(Animator animator, string name, float state)
        {
            animator.SetFloat(name, state);
            SaveButtonAndColorDisable();
        }
        
        #endregion
        
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
            sliderValueAsInt = (int) (customFloatState * 255);
            sb.Append(ConvertToNeededLength(Convert.ToString(sliderValueAsInt, 2), 8));
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

            if(Utilities.IsValid(_ppAnimator))SetAnimatorValue(_ppAnimator, lightnessAnim, lightState);
            if(Utilities.IsValid(_sliderLight)) _sliderLight.value = lightState;

            tempStr = stateStr.Substring(8, 8);
            
            bloomSate = (float) Convert.ToInt32(tempStr, 2) / 255;
            if(Utilities.IsValid(_ppAnimator))SetAnimatorValue(_ppAnimator, bloomAnim, bloomSate);
            if(Utilities.IsValid(_sliderBloom)) _sliderBloom.value = bloomSate;
            
            tempStr = stateStr.Substring(16, 8);
            customFloatState = (float) Convert.ToInt32(tempStr, 2) / 255;
            if(Utilities.IsValid(_customAnimator)) SetAnimatorValue(_customAnimator, _customFloatName, customFloatState);
            if(Utilities.IsValid(_customSlider)) _customSlider.value = customFloatState;
            
            ppState = stateStr[24] == '1';
            TogglePostProcessing();
            aLState = stateStr[25] == '1';
            ToggleObjectsHelper(aLState, new []{_audiolink}, _audioLinkButton, _audioLinkOnColor);
            colsState = stateStr[26] == '1';
            ToggleCollidersHelper(colsState, _colliders, _colliderButton, _collidersOnColor);
            pickupState= stateStr[27] == '1';
            ToggleObjectsHelper(pickupState, _pickUpObjects, _pickUpButton, _pickUpsOnColor);
            chairState = stateStr[28] == '1';
            ToggleCollidersHelper(chairState, _chairColliders, _chairButton, _chairsOnColor);
            pensState = stateStr[29] == '1';
            ToggleObjectsHelper(pensState, new []{_pensObject}, _pensButton, _pensOnColor);
            customBoolState1 = stateStr[30] == '1';
            customBoolState2 = stateStr[31] == '1';
        }

        #endregion

        #region save button visuals

        //This could easily be combined to one method, but having them be separate I believe makes visually parsing the code easier. 
        private void SaveButtonAndColorDisable()
        {
            if(Utilities.IsValid(_saveButton))_saveButton.color = _OffColor;
            _savedToPersistence = false;
            if(Utilities.IsValid(_persistenceText)) _persistenceText.text = notSavedText;
        }

        private void SaveButtonAndColorEnabled()
        {
            if(Utilities.IsValid(_saveButton))_saveButton.color = _saveButtonOnColor;
            _savedToPersistence = true;
            if(Utilities.IsValid(_persistenceText)) _persistenceText.text = savedText;
        }

        #endregion
        
        

        
    }
}