using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImperialStudio.Core.UnityEngine.Input
{
    public class InputManager
    {
        private Dictionary<string, KeyCode> _keyMappings = new Dictionary<string, KeyCode>();
        private Dictionary<string, KeyCode> _defaultKeyMappings = new Dictionary<string, KeyCode>();
        private Dictionary<string, KeyCode> _builtinKeys = new Dictionary<string, KeyCode>();

        public InputManager()
        {
            RegisterKey(BuiltInKeys.Speed, KeyCode.LeftShift);
            RegisterKey(BuiltInKeys.Jump, KeyCode.Space);
        }

        public bool GetKeyDown(string keyName)
        {
            keyName = NormalizeKeyName(keyName);

            if (!IsKeyRegistered(keyName))
            {
                throw new ArgumentException($"Key is not registered: \"{keyName}\"", nameof(keyName));
            }

            return Input.GetKeyDown(_keyMappings[keyName]);
        }

        public bool GetKeyDown(BuiltInKeys builtInKey)
        {
            var keyName = NormalizeKeyName(builtInKey);
            return GetKeyDown(keyName);
        }

        public bool GetKeyUp(string keyName)
        {
            keyName = NormalizeKeyName(keyName);

            if (!IsKeyRegistered(keyName))
            {
                throw new ArgumentException($"Key is not registered: \"{keyName}\"", nameof(keyName));
            }

            return Input.GetKeyUp(_keyMappings[keyName]);
        }

        public bool GetKeyUp(BuiltInKeys builtInKey)
        {
            var keyName = NormalizeKeyName(builtInKey);
            return GetKeyUp(keyName);
        }

        public bool GetKey(string keyName)
        {
            keyName = NormalizeKeyName(keyName);

            if (!IsKeyRegistered(keyName))
            {
                throw new ArgumentException($"Key is not registered: \"{keyName}\"", nameof(keyName));
            }

            return Input.GetKey(_keyMappings[keyName]);
        }

        public bool GetKey(BuiltInKeys builtInKey)
        {
            var keyName = NormalizeKeyName(builtInKey);
            return GetKey(keyName);
        }

        internal void RegisterKey(BuiltInKeys builtInKey, KeyCode defaultKey)
        {
            var keyName = NormalizeKeyName(builtInKey);

            RegisterKey(keyName, defaultKey);
            _builtinKeys.Add(keyName, defaultKey);
        }

        public void RegisterKey(string keyName, KeyCode defaultKey)
        {
            keyName = NormalizeKeyName(keyName);

            if (IsKeyRegistered(keyName))
            {
                throw new ArgumentException($"Key is already registered: \"{keyName}\"", nameof(keyName));
            }

            _defaultKeyMappings.Add(keyName, defaultKey);

            // Should not happen.
            if (_keyMappings.ContainsKey(keyName))
                _keyMappings.Remove(keyName);

            _keyMappings.Add(keyName, defaultKey);
        }

        public void UnregisterKey(string keyName)
        {
            keyName = NormalizeKeyName(keyName);
        
            _defaultKeyMappings.Remove(keyName);
            _keyMappings.Remove(keyName);
        }

        public void SetKeyCode(string keyName, KeyCode value)
        {
            keyName = NormalizeKeyName(keyName);

            if (!IsKeyRegistered(keyName))
            {
                throw new ArgumentException($"Key is not registered: \"{keyName}\"", nameof(keyName));
            }

            _keyMappings[keyName] = value;
        }

        internal void ClearNonBuiltInKeys()
        {
            // remove non-builtin keys
            _defaultKeyMappings = _builtinKeys.Where(pair => !IsBuiltInKey(pair.Key))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            _keyMappings = _builtinKeys.ToDictionary(entry => entry.Key, entry => entry.Value); // copy mappings
        }

        private bool IsBuiltInKey(string keyName)
        {
            keyName = NormalizeKeyName(keyName);
            return _builtinKeys.ContainsKey(keyName);
        }


        private bool IsKeyRegistered(string keyName)
        {
            return _defaultKeyMappings.ContainsKey(keyName);
        }

        private string NormalizeKeyName(BuiltInKeys keyName)
        {
            return NormalizeKeyName(keyName.ToString());
        }

        private string NormalizeKeyName(string keyName)
        {
            return keyName.ToLower().Replace(" ", "_");
        }

        public enum BuiltInKeys
        {
            Speed,
            Jump
        }
    }
}
