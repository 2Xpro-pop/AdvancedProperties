using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

namespace AdvancedProperties;
public abstract class AdvancedPropertyContext
{
    public AdvancedProperty Property
    {
        get;
    }

    protected AdvancedPropertyContext(AdvancedProperty advancedProperty)
    {
        Property = advancedProperty;
    }
}

public class AdvancedPropertyContext<TOwner, TValue> : AdvancedPropertyContext
{
    public AdvancedPropertyContext(AdvancedProperty<TOwner, TValue> advancedProperty, TOwner owner) : base(advancedProperty)
    {
        Guard.IsNotNull(advancedProperty, nameof(advancedProperty));
        Guard.IsNotNull(owner, nameof(owner));

        Property = advancedProperty;
        Owner = owner;

        if (advancedProperty.DefaultValue is TValue defaultValue)
        {
            Value = defaultValue;
        }
        else if (advancedProperty.CreateDefaultValue is AdvancedProperty.CreateDefaultValueDelegate<TOwner, TValue> defaultValueDelegate)
        {
            Value = defaultValueDelegate(owner);
        }
        else
        {
            Value = default!;
        }

        Context = Value ?? AdvancedProperty.UnsetValue;
        CurrentPriority = AdvancedSetterPriority.Default;
    }

    public new AdvancedProperty<TOwner, TValue> Property
    {
        get;
    }

    public TOwner Owner
    {
        get;
    }

    /// <summary>
    /// Contains value, binding, or resource reference the property.
    /// </summary>
    public object Context
    {
        get; protected set;
    }

    public TValue Value
    {
        get; protected set;
    }

    public AdvancedSetterPriority CurrentPriority
    {
        get; protected set;
    }

    public bool SetValue(TValue value, AdvancedSetterPriority priority)
    {

        if (priority > CurrentPriority)
        {
            return false;
        }

        if (Property.ValidateValue is AdvancedProperty.ValidateValueDelegate<TOwner, TValue> validateValueDelegate)
        {

            if (!validateValueDelegate(Owner, value))
            {
                return false;
            }
        }

        if (Property.CoerceValue is AdvancedProperty.CoerceValueDelegate<TOwner, TValue> coerceValueDelegate)
        {
            value = coerceValueDelegate(Owner, value);
        }

        if (Property is AdvancedProperty.PropertyChangedDelegate<TOwner, TValue> propertyChangedDelegate)
        {
            propertyChangedDelegate(Owner, Value, value);
        }

        Value = value;
        Context ??= value ?? AdvancedProperty.UnsetValue;
        CurrentPriority = priority;

        return true;
    }

}
