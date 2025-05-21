using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KabreetGames.BladeSpinner.TowerSystem.Stats
{
    [Serializable]
    public class Stat
    {
        public string statName;
        protected List<StatModifier> modifiers = new();
        public int ModifierCount => modifiers?.Count ?? 0;

        public virtual void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public virtual void RemoveModifier(StatModifier modifier)
        {
            modifiers.Remove(modifier);
        }

        public void OnRest()
        {
            modifiers?.Clear();
        }
    }

    [Serializable]
    public class Stat<T> : Stat where T : struct
    {
        public T Value => ApplyModifiers();

        private T ApplyModifiers()
        {
            modifiers ??= new List<StatModifier>();
            return modifiers.Count == 0
                ? baseValue
                : modifiers.Aggregate(baseValue, (current, modifier) => modifier.Apply(current));
        }

        public T baseValue;


        public event Action<T, T> OnValueChanged;

        public Stat(T value, string statName)
        {
            baseValue = value;
            this.statName = statName;
        }

        public void SetValue(T value)
        {
            var oldValue = Value;
            baseValue = value;
            OnValueChanged?.Invoke(oldValue, Value);
        }

        public override void AddModifier(StatModifier modifier)
        {
            var oldValue = Value;
            modifiers.Add(modifier);
            OnValueChanged?.Invoke(oldValue, Value);
        }

        public override void RemoveModifier(StatModifier modifier)
        {
            var oldValue = Value;
            modifiers.Remove(modifier);
            OnValueChanged?.Invoke(oldValue, Value);
        }

        public static implicit operator T(Stat<T> stat) => stat.Value;
        public static implicit operator Stat<T>(T value) => new(value, "");

        public override string ToString()
        {
            return Value.ToString();
        }
    }


    [Serializable]
    public class StatModifier
    {
        public virtual ModifierType ModifierType { get; protected set; }
        [field: SerializeField] public float Value { get; private set; }
        public virtual string GetName(string statName) => "";

        protected StatModifier(float value)
        {
            Value = value;
        }

        public virtual void SetValue(float value) => Value = value;


        public virtual T Apply<T>(T current)
        {
            return current;
        }
    }

    [Serializable]
    public class MultiplyStatModifier : StatModifier
    {
        public override ModifierType ModifierType => ModifierType.Multiply;

        public override string GetName(string statName) => $"Increase {statName} by {Value * 100f}%";

        public MultiplyStatModifier(float value) : base(value)
        {
        }

        public override T Apply<T>(T value)
        {
            return value switch
            {
                int intValue => (T)Convert.ChangeType(intValue * (1 + Value), typeof(T)),
                float floatValue => (T)Convert.ChangeType(floatValue * (1 + Value), typeof(T)),
                _ => value
            };
        }
    }

    [Serializable]
    public class AddStatModifier : StatModifier
    {
        public override ModifierType ModifierType => ModifierType.Add;

        public override string GetName(string statName) => $"Increase {statName} by {Value}";
        public AddStatModifier(float value) : base(value)
        {
        }

        public override T Apply<T>(T value)
        {
            return value switch
            {
                int intValue => (T)Convert.ChangeType(intValue + Value, typeof(T)),
                float floatValue => (T)Convert.ChangeType(floatValue + Value, typeof(T)),
                _ => value
            };
        }
    }

    public enum ModifierType
    {
        Multiply,
        Add
    }
}