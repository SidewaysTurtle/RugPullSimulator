using UnityEngine;

namespace Monologue.Dialogue
{
    public class TriggerDialogue : MonoBehaviour
    {
        [Header("Visual Cue")]
        [SerializeField] private GameObject _VisualCue;
        [Header("Ink JSON")]
        [SerializeField] TextAsset _InkJSON;
        bool _IsPlayerInRange;

        private void Awake()
        {
            _VisualCue.SetActive(false);
        }

        void Update()
        {
            if(!DialogueManager.Instance.IsDialogueActive && _IsPlayerInRange)
                _VisualCue.SetActive(true);
            else
                _VisualCue.SetActive(false);
            
            if (DialogueManager.Instance.IsDialogueActive || !_IsPlayerInRange)
                return;

            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F))
                DialogueManager.Instance.EnterDialogMode(_InkJSON);
        }
        private void OnTriggerEnter2D(Collider2D playerCollider)
        {
            _IsPlayerInRange = playerCollider.gameObject.CompareTag("Player");
        }

        private void OnTriggerExit2D(Collider2D playerCollider)
        {
            _IsPlayerInRange = !playerCollider.gameObject.CompareTag("Player");
        }
    }
}