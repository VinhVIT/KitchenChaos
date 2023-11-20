using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHasProgress
{
    public event EventHandler<OnProgressChangedEventArg> OnProgressChanged;
    public class OnProgressChangedEventArg : EventArgs
    {
        public float currentProgess;
    }
}
