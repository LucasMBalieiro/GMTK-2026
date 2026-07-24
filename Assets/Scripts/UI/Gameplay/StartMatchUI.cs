using System;
using UnityEngine;

namespace UI
{
    public class StartMatchUI : MonoBehaviour
    {
        EventBinding<StartupTickEvent> startupBinding;

        public void OnEnable()
        {
            startupBinding = new EventBinding<StartupTickEvent>(ChangeUI);
            EventBus<StartupTickEvent>.Register(startupBinding);
        }
        
        private void OnDisable()
        {
            EventBus<StartupTickEvent>.Deregister(startupBinding);    
        }
        
        private void ChangeUI(StartupTickEvent tick)
        {
            if(tick.ticksRemaining == 0) Destroy(gameObject);
        }

        public void StartMatch()
        {
            EventBus<PlayMetronome>.Raise(new PlayMetronome());
        }
    }
}
