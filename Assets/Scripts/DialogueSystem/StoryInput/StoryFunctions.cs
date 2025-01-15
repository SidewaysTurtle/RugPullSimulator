using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Monologue.StoryInput;

using Ink.Runtime;

using System.Linq;

namespace Monologue.Dialogue
{
    public class StoryFunctions
    {
        public delegate void OnCameraSet(string cameraTag, bool goBack);
        public static event OnCameraSet OnCameraSetEvent;

        public delegate void OnSpeaker(string speaker);
        public static event OnSpeaker OnSpeakerEvent;

        public delegate void OnAnimation(string animation);
        public static event OnAnimation OnAnimationEvent;

        public static List<string> TagtoList(string tagValue)
        {
            if(tagValue.ToCharArray().Count() == 0)
                return new List<string>();

            if(tagValue[0] == '{' && tagValue[tagValue.Count()-1] =='}')
                // FIXME: Gross hack
                return tagValue.Remove(0).Remove(tagValue.Count()-1).Split(',').ToList();
            else
                return new List<string>(){tagValue};
                
        }
        // Custom Ink handling
        public static void HandleTags(Story story)
        {
            foreach(string tag in story.currentTags)
            {
                string[] tagSplit = tag.Split(':');
                if(tagSplit.Length != 2)
                    Debug.LogError("Tag is not formatted correctly");

                string tagValue = tagSplit[1].Trim();

                switch(tagSplit[0].Trim())
                {
                    case "speaker":
                        OnSpeakerEvent?.Invoke(tagValue);
                    break;

                    case "animation":
                        OnAnimationEvent?.Invoke(tagValue);
                    break;

                    case "image":
                        // Image handling should be done by Panel through events
                        OnSpeakerEvent?.Invoke(tagValue);  // Panel will handle the profile image
                    break;

                    // Format strings
                    // my name is: <name> #format:name
                    case "format":
                        List<string> listOfFormat = TagtoList(tagValue);
                        string text = story.currentText;
                        foreach(string vars in listOfFormat)
                            text = text?.Replace($"<{vars}>",DialogueManager.Instance.GlobalVars[vars].ToString());
                        DialogueManager.EmitDialogueContent(text);  // Updated to use the static method
                    break;

                    case "cutscene":
                        // Handle cutscene logic if needed
                    break;
                }
            }
        }

        // Used in Ink
        // EXTERNAL name(params)
        // e.g EXTERNAL InputText("What IS \"up dog\"?", false)

        public static void BindFunctions(Story story)
        {
            story.BindExternalFunction("SetCamera", (string cameraTag) => 
            {
                // FIXME: Because it doesnt wait for the anaimation to end before continuing, it can reach ends where it tries to look for camera null
                OnCameraSetEvent?.Invoke( cameraTag, false ); // will fix later
            });

            story.BindExternalFunction("ChangeScene", (string sceneName) =>
            {
                SceneManager.LoadScene(sceneName);
            });
        }

        public static void UnbindFunctions(Story story)
        {
            story.UnbindExternalFunction("SetCamera");
            story.UnbindExternalFunction("ChangeScene");
        }

    }
}