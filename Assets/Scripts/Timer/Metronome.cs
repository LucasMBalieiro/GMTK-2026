using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Metronome : MonoBehaviour
{
    
    public int intervalMs = 1000;

    private CancellationTokenSource cancellationTokenSource;
    private bool isPaused = true;
    private float timerMs = 0f;
    
    private EventBinding<PlayMetronome> playBinging;
    private EventBinding<PauseMetronome> pauseBinding;

    private void OnEnable()
    {
        cancellationTokenSource = new CancellationTokenSource();

        playBinging = new EventBinding<PlayMetronome>(Play);
        EventBus<PlayMetronome>.Register(playBinging);

        pauseBinding = new EventBinding<PauseMetronome>(Pause);
        EventBus<PauseMetronome>.Register(pauseBinding);
        
        MetronomeTask(cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
        
        EventBus<PlayMetronome>.Deregister(playBinging);
        EventBus<PauseMetronome>.Deregister(pauseBinding);
    }

    private void Pause() { isPaused = true; }

    private void Play() { isPaused = false; }

    private async UniTaskVoid MetronomeTask(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);

            if (isPaused) 
            {
                continue;
            }
            
            timerMs += Time.deltaTime * 1000f;

            if (!(timerMs >= intervalMs)) continue;
            
            timerMs -= intervalMs; 
            EventBus<Tick>.Raise(new Tick());
        }
    }
}