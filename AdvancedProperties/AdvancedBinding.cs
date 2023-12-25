using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedProperties;
public class AdvancedBinding
{
    public static readonly object BindingContextSource = new();
    public static readonly object Self = new();

    public string? Path
    {
        get; set;
    }

    public object Source
    {
        get; set;
    } = BindingContextSource;
}
