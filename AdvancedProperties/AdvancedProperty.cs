using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace AdvancedProperties;

public abstract class AdvancedProperty
{
    public static readonly object UnsetValue = new();

    public delegate TValue CoerceValueDelegate<TOwner, TValue>(TOwner owner, TValue value);
    public delegate bool ValidateValueDelegate<in TOwner, in TValue>(TOwner owner, TValue value);
    public delegate TValue CreateDefaultValueDelegate<in TDeclarer, out TValue>(TDeclarer owner);
    public delegate void PropertyChangedDelegate<in TOwner, in TValue>(TOwner owner, TValue oldValue, TValue newValue);


    protected AdvancedProperty(string name, Type ownerType, bool isReadOnly, Type returnType, object? defaultValue, AdvancedBindingMode bindingMode, bool isAttached, bool isContent)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        Guard.IsNotNull(ownerType, nameof(ownerType));
        Guard.IsNotNull(returnType, nameof(returnType));


        if (bindingMode == AdvancedBindingMode.Default)
        {
            throw new ArgumentException("The binding mode cannot be default in Property.", nameof(bindingMode));
        }

        if (defaultValue is not null)
        {
            if (defaultValue.GetType().IsAssignableTo(returnType))
            {
                throw new ArgumentException("The default value must be assignable to the return type.", nameof(defaultValue));
            }
        }

        Name = name;
        OwnerType = ownerType;
        IsReadOnly = isReadOnly;
        ReturnType = returnType;
        DefaultValue = defaultValue;
        DefaultBindingMode = bindingMode;
        IsAttached = isAttached;
        IsContent = isContent;
    }

    public string Name
    {
        get;
    }

    public Type OwnerType
    {
        get;
    }

    public bool IsReadOnly
    {
        get;
    }

    public Type ReturnType
    {
        get;
    }

    public object? DefaultValue
    {
        get;
    }

    public AdvancedBindingMode DefaultBindingMode
    {
        get;
    }

    public bool IsAttached
    {
        get;
    }

    public bool IsContent
    {
        get;
    }

    public static AdvancedProperty<TOwner, TValue> Register<TOwner, TValue>(
        string name,
        TValue? defaultValue = default,
        AdvancedBindingMode defaultBindingMode = AdvancedBindingMode.OneWay,
        CoerceValueDelegate<TOwner, TValue>? coerceValueDelegate = null,
        ValidateValueDelegate<TOwner, TValue>? validateValueDelegate = null,
        CreateDefaultValueDelegate<TOwner, TValue>? createDefaultValueDelegate = null,
        PropertyChangedDelegate<TOwner, TValue>? propertyChangedDelegate = null) where TOwner : AdvancedObject
    {
        return new(name, false, defaultValue, defaultBindingMode, false, false, coerceValueDelegate, validateValueDelegate, createDefaultValueDelegate, propertyChangedDelegate);
    }
}

public class AdvancedProperty<TOwner, TValue> : AdvancedProperty where TOwner: AdvancedObject
{
    public AdvancedProperty(string name, bool isReadOnly, TValue? defaultValue, AdvancedBindingMode bindingMode, bool isAttached, bool isContent, CoerceValueDelegate<TOwner, TValue>? coerceValue, ValidateValueDelegate<TOwner, TValue>? validateValue, CreateDefaultValueDelegate<TOwner, TValue>? createDefaultValue, PropertyChangedDelegate<TOwner, TValue>? propertyChanged) : base(name, typeof(TOwner), isReadOnly, typeof(TValue), defaultValue, bindingMode, isAttached, isContent)
    {
        DefaultValue = defaultValue;
        CoerceValue = coerceValue;
        ValidateValue = validateValue;
        CreateDefaultValue = createDefaultValue;
        PropertyChanged = propertyChanged;
    }

    public CoerceValueDelegate<TOwner, TValue>? CoerceValue
    {
        get;
    }

    public ValidateValueDelegate<TOwner, TValue>? ValidateValue
    {
        get;
    }

    public CreateDefaultValueDelegate<TOwner, TValue>? CreateDefaultValue
    {
        get;
    }

    public PropertyChangedDelegate<TOwner, TValue>? PropertyChanged
    {
        get;
    }

    public new TValue? DefaultValue
    {
        get;
    }
}