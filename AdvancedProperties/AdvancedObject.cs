using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedProperties;
public partial class AdvancedObject : IAdvancedBindingAccessor
{
    public static readonly AdvancedProperty<AdvancedObject, IAdvancedBindingAccessor?> BindingContextProperty =
        AdvancedProperty.Register<AdvancedObject, IAdvancedBindingAccessor?>(nameof(BindingContext));

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


}
