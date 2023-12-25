using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedProperties;
public partial class AdvancedObject : IAdvancedBindingAccessor, INotifyPropertyChanged
{
    public static readonly AdvancedProperty<AdvancedObject, IAdvancedBindingAccessor?> BindingContextProperty =
        AdvancedProperty.Register<AdvancedObject, IAdvancedBindingAccessor?>(nameof(BindingContext));

    public event PropertyChangedEventHandler? PropertyChanged;

    public IAdvancedBindingAccessor? BindingContext
    {
        get => GetBindingContextProperty();
        set => SetBindingContextProperty(value);
    }

    public AdvancedObject()
    {
        Init();
    }

    public virtual IAdvancedBindingContext? Access(AdvancedBinding binding)
    {
        return null;
    }

    public virtual void OnPropertyChanged<TOwner,TValue>(AdvancedProperty<TOwner, TValue> property, TValue oldValue, TValue newValue) where TOwner: AdvancedObject
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property.Name));
    }
}
