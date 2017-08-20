// Copyright (c) MooCooProductions. All rights reserved.

using System;
using UnityEngine;

namespace MooCooEngine
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                //if (_instance == null)
                //{
                //    try
                //    {
                //        _instance = FindObjectOfType<T>();
                //    }
                //    catch (Exception e) { }
                //}
                return _instance;
            }
            set
            {
                //# Only assign value, if we haven't defined an instance yet. Otherwise ignore.
                if(_instance == null)
                    _instance = value;
            }
        }
    }
}