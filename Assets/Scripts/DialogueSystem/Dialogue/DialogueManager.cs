using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ink.Runtime;
using Monologue.StoryInput;
using SimpleMan.CoroutineExtensions;

namespace Monologue.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance {get; private set;}
        public bool IsWaiting;
        public delegate void OnDialogue();
        public static event OnDialogue OnDialogueStartEvent;
        public static event OnDialogue OnDialogueEndEvent;
        public static event OnDialogue OnDialogueContinuedEvent;
        public static event OnDialogue OnDialogueTryingToContinueEvent;
        public delegate void OnChoice(List<string> choices);
        public static event OnChoice OnChoiceEvent;

        public delegate void OnDialogueContent(string text);
        public static event OnDialogueContent OnDialogueContentEvent;

        public delegate void OnChoicesPresented(List<string> choices);
        public static event OnChoicesPresented OnChoicesPresentedEvent;
        
        [Header("Globals Ink")]
        [SerializeField] TextAsset m_GlobalsJSON;
        public Variables GlobalVars;
        [Header("Dialogue UI")]
        public Story CurrentStory;
        
        // Remove Panel reference
        // public Panel _DialoguePanel;

        private bool _isDialogueActive;
        public bool IsDialogueActive 
        {
            get => _isDialogueActive;
            private set => _isDialogueActive = value;
        }

        void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
            GlobalVars = new(m_GlobalsJSON);
        }
        void OnEnable()
        {
            // FIXME: Passes up the Event, because I cannot invoke an event that isnt in the file. (in DialoguePrefab)
            Panel.OnChoiceSelectedEvent += ChoiceSelected;

            StoryInputTextFieldManager.OnStoryInputStartEvent += OnEnterInputMode;
            StoryInputTextFieldManager.OnStoryInputEndEvent += OnExitInputMode;
            DontDestroyHelper.NotDestroyedHelperEvent += DeactivatePanel;
        }
        void DeactivatePanel()
        {
            ChangeSceneOnLoadDontDestroy.Instance.NextScene();
            IsDialogueActive = false;
        }
        void OnDisable()
        {
            Panel.OnChoiceSelectedEvent -= ChoiceSelected;
            
            StoryInputTextFieldManager.OnStoryInputStartEvent -= OnEnterInputMode;
            StoryInputTextFieldManager.OnStoryInputEndEvent -= OnExitInputMode;
            DontDestroyHelper.NotDestroyedHelperEvent -= DeactivatePanel;
        }
        void OnEnterInputMode()
        {
            IsDialogueActive = false;
        }

        void OnExitInputMode()
        {
            IsDialogueActive = true;
            ContinueStory();
        }
        void Update()
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F) || (Input.GetMouseButtonDown(0) && CurrentStory?.currentChoices.Count == 0))
            && !IsWaiting)
                ContinueStory();
        }
        // FIXME: stupid flag variable
        bool _isAlreadyContinued;
        public void ContinueStory()
        {
            OnDialogueTryingToContinueEvent?.Invoke();
            if (!IsDialogueActive)
                return;
            
            if(CurrentStory.canContinue)
            {
                //FIXME: this is awful.
                if(!_isAlreadyContinued)
                {
                    CurrentStory.Continue();
                    
                    _isAlreadyContinued = true;
                }
                if(_isAlreadyContinued && StoryInputTextFieldManager.Instance.ActiveInputPanel)
                {
                    return;   
                }
                _isAlreadyContinued = false;
                
                if(CurrentStory.currentText == "" && !CurrentStory.canContinue)
                    ExitDialogMode();
                
                OnDialogueContinuedEvent?.Invoke();

                // Emit dialogue content
                OnDialogueContentEvent?.Invoke(CurrentStory.currentText);

                // Emit choices if any
                var choices = CurrentStory.currentChoices.Select(ctx => ctx.text).ToList();
                if(choices.Count > 0)
                    OnChoicesPresentedEvent?.Invoke(choices);

                StoryFunctions.HandleTags(CurrentStory);
            }
            else
            {
                ExitDialogMode();
            }
        }
        public void EnterDialogMode(TextAsset inkAsset)
        {
            OnDialogueStartEvent?.Invoke();
            IsDialogueActive = true;

            CurrentStory = new Story(inkAsset.text);
            StoryFunctions.BindFunctions(CurrentStory);
            GlobalVars.StartListening(CurrentStory);
            
            ContinueStory();
        }

        void ExitDialogMode()
        {
            StoryFunctions.UnbindFunctions(CurrentStory);
            GlobalVars.StopListening(CurrentStory);
            IsDialogueActive = false;
            OnDialogueEndEvent?.Invoke();
        }

        public void ChoiceSelected(OptionPrefab option)
        {
            CurrentStory.ChooseChoiceIndex(option.index);
            ContinueStory();
        }

        public void ChoiceSelected(int idx)
        {
            print("int" + idx);
            CurrentStory.ChooseChoiceIndex(idx);
            ContinueStory();
        }

        public static void EmitDialogueContent(string text)
        {
            OnDialogueContentEvent?.Invoke(text);
        }

    }
}