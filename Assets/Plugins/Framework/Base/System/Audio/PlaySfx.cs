using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySfx : MonoBehaviour
{
    public string sfxName;
    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(sfxName))
        {
            Aud.PlaySfx(sfxName);
        }
    }
}
