using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Metronome : MonoBehaviour
{
    
    public int intervalMs = 1000;

    private CancellationTokenSource cancellationTokenSource;
    private bool isPaused = false;
    private float timerMs = 0f;
    
    public event Action Tick;

    private void OnEnable()
    {
        cancellationTokenSource = new CancellationTokenSource();
        
        MetronomeTask(cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
    }
    
    public void Pause() { isPaused = true; }
    
    public void Play() { isPaused = false; }

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
            Tick?.Invoke();
        }
    }
}