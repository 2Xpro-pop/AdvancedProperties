using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedProperties;

/// <summary>
/// Represents priority levels for setting values.
/// A value can only be set if the priority level of the action is lower or equal to the current priority level.
/// For example, a value with 'Direct' priority can always be set, 
/// while a value with 'Default' priority can be replaced by any other priority level.
/// </summary>
public enum AdvancedSetterPriority
{
    Direct, // Highest priority
    Binding,
    Init,
    Default, // Lowest priority
}
