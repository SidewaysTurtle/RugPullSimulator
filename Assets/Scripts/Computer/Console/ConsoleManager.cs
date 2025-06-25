using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

namespace Console
{
    public partial class ConsoleManager : MonoBehaviour
    {
        [SerializeField] InputField _CurrentInput;
        [SerializeField] GameObject _CurrentInputModule;
        [SerializeField] VerticalLayoutGroup _vertLayoutGroup;
        [SerializeField] GameObject _Command;
        [SerializeField] GameObject _Response;
        [SerializeField] GameObject _Input;
        TextMeshProUGUI _commandText;

        void OnEnable()
        {
            _CurrentInput.onSubmit.AddListener(OnSubmit);
        }
        void Start()
        {
            // _Command = Resources.Load("Computer/Window/Console/Command") as GameObject;
            // _Response = Resources.Load("Computer/Window/Console/Response") as GameObject;
            // _Input = Resources.Load("Computer/Window/Console/Input") as GameObject;
        }

        void Update()
        {
        }
        public void SetCurrentInputMod(GameObject input, InputField inputField)
        {
            _CurrentInputModule = input;
            _CurrentInput = inputField;
            _CurrentInput.onSubmit.AddListener(OnSubmit);
        }
        public virtual void OnSubmit(string eventData)
        {
            if(_CurrentInput.isFocused)
            {
                // delete the input
                Destroy(_CurrentInputModule);

                // make a command
                var newC = Instantiate(_Command, new Vector3(0,0,0), Quaternion.identity);
                newC.transform.SetParent(_vertLayoutGroup.transform,false);
                //FixME: GetComponent slow
                _commandText = newC.GetComponentInChildren<TextMeshProUGUI>();
                _commandText.text = eventData;
                
                // remake the input
                var newS = Instantiate(_Input, new Vector3(0,0,0), Quaternion.identity);
                newS.transform.SetParent(_vertLayoutGroup.transform,false);
                SetCurrentInputMod(newS.gameObject,newS.GetComponentInChildren<InputField>());
            }
        }
        public void CreateResponse(string eventData)
        {
            var newR = Instantiate(_Response, new Vector3(0,0,0), Quaternion.identity);
            newR.transform.SetParent(_vertLayoutGroup.transform,false);
            _commandText = newR.GetComponentInChildren<TextMeshProUGUI>();
            _commandText.text = eventData;
        }
        void DeactivateInputField()
        { 
            _CurrentInput.DeactivateInputField();
        }
    }
}