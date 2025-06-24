using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Monologue
{
    public class DontDestroyHelper : MonoBehaviour
    {
        public static DontDestroyHelper Instance {get; private set;}
        [SerializeField] int _TotalNotDestroyable;
        int _NotDestroyableCount = 0;

        [Header("Events")]
        public UnityEvent OnAllNotDestroyableReady;

        bool _calledHelperEvent = false;

        void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
        }
        void OnEnable()
        {
            Monologue.DontDestroyOnLoad.NotDestroyedEvent += AddToCounter;
        }

        void OnDisable()
        {
            Monologue.DontDestroyOnLoad.NotDestroyedEvent -= AddToCounter;
        }

        void AddToCounter()
        {
            _NotDestroyableCount++;
        }

        // void Start

        void Update()
        {
            if(!_calledHelperEvent && _TotalNotDestroyable == _NotDestroyableCount)
            {
                OnAllNotDestroyableReady?.Invoke();
                _calledHelperEvent = true;
            }
        }
    }
}
