using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Skill_Active_Guard : ISkill
{
    public int staminaConsume = 15;
    private CancellationTokenSource guardCancellationToken;
    
    private void Start()
    {
        if(_pc == null) _pc = PlayerController.Instance;
    }
    
    public override void Activate()
    {
        if (currentCD <= 0 && _pc.staminaSystem.HasEnoughStamina(staminaConsume))
        {
            GuardAnim(true);
            _pc.staminaSystem.Consume(staminaConsume);
        }
    }

    public override void Deactivate()
    {
        GuardAnim(false);
    }
    
    public async UniTaskVoid GuardAnim(bool input)
    {
        if (guardCancellationToken != null)
        {
            // Cancel any ongoing guard action
            guardCancellationToken.Cancel();
            guardCancellationToken.Dispose();
            guardCancellationToken = null;
        }

        if (!input)
        {
            _pc.playerAnimManager.GuardAnim(false);
            _pc.PlayerHealth.IdleState();
            return;
        }
        // Start a new guard action
        guardCancellationToken = new CancellationTokenSource();
        _pc.playerAnimManager.GuardAnim(true);

        try
        {
            // Wait for 5 seconds or until cancellation
            await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: guardCancellationToken.Token);
        }
        catch (OperationCanceledException)
        {
            // Guard action was canceled, do nothing
            return;
        }
        finally
        {
            // Clean up the cancellation token
            guardCancellationToken.Dispose();
            guardCancellationToken = null;
        }

        // If not canceled, end the guard animation
        _pc.playerAnimManager.GuardAnim(false);
        _pc.PlayerHealth.IdleState();
    }
}
