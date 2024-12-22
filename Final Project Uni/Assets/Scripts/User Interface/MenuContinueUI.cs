using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MenuContinueUI : MonoBehaviour
{
    private Ingame_Save _save;

    [Button]
    public void Start()
    {
        _save = Ingame_Save.Instance;
        //Debug.Log(_save.haveFileLoad);
        gameObject.SetActive(_save.haveFileLoad);
    }
}
