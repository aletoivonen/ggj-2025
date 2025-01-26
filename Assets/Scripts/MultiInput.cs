using System.Collections.Generic;
using UnityEngine;

namespace Zubble
{
    public class MultiInput
    {
        public static MultiInput Instance => _multiInput ??= new MultiInput();
        private static MultiInput _multiInput;

        private readonly Dictionary<string, float> _mobileAxis = new()
        {
            { "Horizontal", 0f },
            { "Vertical", 0f }
        };

        private readonly Dictionary<string, bool> _mobileButtonsDown = new()
        {
            { "Jump", false },
            { "Soap", false }
        };

        public MultiInput()
        {
            if (_multiInput != null)
            {
                return;
            }
            _multiInput = this;
        }

        public float GetAxis(string axis)
        {
            if (_mobileAxis.TryGetValue(axis, out var axis1))
            {
                if (axis1 != 0f)
                {
                    return axis1;
                }
            }
            return Input.GetAxis(axis);
        }
        
        public void SetAxis(string axis, float value)
        {
            if (_mobileAxis.ContainsKey(axis))
            {
                _mobileAxis[axis] = value;
                return;
            }

            Debug.LogWarning($"Axis {axis} not found");
        }

        public bool GetButtonDown(string button)
        {
            if (_mobileButtonsDown.TryGetValue(button, out var button1))
            {
                if (button1)
                {
                    return true;
                }
            }
            return Input.GetButtonDown(button);
        }

        public void SetButtonDown(string button, bool value)
        {
            if (_mobileButtonsDown.ContainsKey(button))
            {
                _mobileButtonsDown[button] = value;
                return;
            }

            Debug.LogWarning($"Button {button} not found");
        }
    }
}