using System.Threading.Tasks;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class StaticMethods
{
    public static async Task RepeatActionAsync(Action action, float delayInSeconds, int repeatCount)
    {
        int trueDelay = Mathf.RoundToInt(delayInSeconds * 1000);

        for (int i = 0; i < repeatCount; i++)
        {
            action?.Invoke();
            await Task.Delay(trueDelay);
        }
    }
}