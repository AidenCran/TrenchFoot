using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
    #region TMP Extensions

    #region Randomize Characters

    const string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";

    public static async Task DisplayDecipher(this TMP_Text tmpText, string endValue, float duration)
    {
        var endTime = Time.time + duration;
        var charList = endValue.ToCharArray();

        // Time Per Reveal
        float revealTime = duration / charList.Length;
        int revealedChars = 0;

        _ = StaticMethods.RepeatActionAsync(() =>
        {
            revealedChars += 1;
        }, revealTime, charList.Length);

        while (Time.time < endTime)
        {
            var newCharArray = new char[charList.Length].RandomizeCharArray();

            if (revealedChars > 0)
            {
                for (int i = 0; i < revealedChars; i++)
                {
                    newCharArray[i] = charList[i];
                }
            }

            tmpText.SetCharArray(newCharArray);

            // Runs 20x / Second
            await Task.Delay(50);
        }

        tmpText.text = endValue;
    }

    public static char[] RandomizeCharArray(this char[] charArray)
    {
        var availableChars = chars.ToCharArray();

        for (int i = 0; i < charArray.Length; i++)
        {
            charArray[i] = availableChars.SelectRandom();
        }

        return charArray;
    }

    #endregion

    #region Integrated Wobble

    //// Still Needs Modifications - If continued use is wanted
    //public static async Task WobbleMesh(this TMP_Text _textMesh, Vector2 Intensity, float duration)
    //{
    //    var endTime = Time.time + duration;

    //    var _mesh = _textMesh.mesh;
    //    var _vertices = _mesh.vertices;

    //    var textInfo = _textMesh.textInfo;

    //    _textMesh.ForceMeshUpdate();
    //    _mesh = _textMesh.mesh;
    //    _vertices = _mesh.vertices;

    //    Color[] colors = _mesh.colors;

    //    // Moved here to be Self-Contained
    //    Vector2 Wobble(float time) => new Vector2(Mathf.Sin(time * Intensity.x), Mathf.Cos(time * Intensity.y));

    //    while (Time.time < endTime)
    //    {
    //        for (int w = 0; w < textInfo.wordCount; w++)
    //        {
    //            Vector3 offset = Wobble(Time.time + w);

    //            for (int i = 0; i < textInfo.wordInfo[w].characterCount; i++)
    //            {
    //                TMP_CharacterInfo c = _textMesh.textInfo.characterInfo[w + i];

    //                int index = c.vertexIndex;

    //                _vertices[index] += offset;
    //                _vertices[index + 1] += offset;
    //                _vertices[index + 2] += offset;
    //                _vertices[index + 3] += offset;
    //            }
    //        }

    //        _mesh.vertices = _vertices;
    //        _mesh.colors = colors;
    //        _textMesh.canvasRenderer.SetMesh(_mesh);

    //        await Task.Delay(50);
    //        await Task.Yield();
    //    }
    //}

    #endregion

    #endregion

    #region Task Extensions

    public static async Task OnComplete(this Task task, Action action)
    {
        await task;
        action?.Invoke();
    }

    public static async Task OnStart(this Task task, Action action)
    {
        action?.Invoke();
        await task;
    }

    #endregion

    #region List Extensions

    public static T SelectRandom<T>(this IList<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static bool IsNullOrEmpty<T>(this IList<T> list)
    {
        return list == null || list.Count == 0;
    }

    #endregion
}