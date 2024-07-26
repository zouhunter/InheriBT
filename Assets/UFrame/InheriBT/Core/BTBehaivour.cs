/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-12
 * Version: 1.0.0
 * Description: 行为树行为脚本
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT
{
    public class BTBehaivour : MonoBehaviour
    {
        [SerializeField]
        protected BTree _bt;
        [SerializeField]
        protected bool _isRunning;
        [SerializeField]
        protected bool _continueRunning;
        [SerializeField]
        protected bool _autoStartOnEnable;
        [SerializeField]
        protected float _interval = 0.1f;
        protected float _intervalTimer;
        [SerializeField]
        protected BTree _btInstance;
        [SerializeField]
        protected List<BindingInfo> _bindings;

        protected virtual void Awake()
        {
            _btInstance = Instantiate(_bt);
        }

        protected virtual void OnEnable()
        {
            if (_autoStartOnEnable)
            {
                foreach (var binding in _bindings)
                {
                    _btInstance.SetVariable(binding.name, new Variable<UnityEngine.Object>() { Value = binding.target });
                }
                _isRunning = _btInstance.StartUp();
            }
        }

        protected virtual void OnDisable()
        {
            if (_autoStartOnEnable)
            {
                _isRunning = false;
                _btInstance.Stop();
            }
        }

        protected virtual void Update()
        {
            if (!_isRunning)
                return;

            if (_interval > 0)
            {
                if (_intervalTimer < _interval)
                {
                    _intervalTimer += Time.deltaTime;
                    return;
                }
            }

            if (_isRunning)
            {
                var result = _btInstance.Tick();
                if (result == Status.Success || result == Status.Failure)
                {
                    _isRunning = _continueRunning;
                }
            }
        }
    }
}

