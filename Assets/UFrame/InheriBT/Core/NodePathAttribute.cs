/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-21
 * Version: 1.0.0
 * Description: 
 *_*/

using System;

namespace UFrame {

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class NodePathAttribute :Attribute
    {
        public string desc;
        public NodePathAttribute(string desc)
        {
            this.desc = desc;
        }
    }
}

