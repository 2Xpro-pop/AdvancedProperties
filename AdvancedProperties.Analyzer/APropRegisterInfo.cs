using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdvancedProperties.Analyzer;
public class APropRegisterInfo
{
    public APropRegisterInfo(string propertyName, TypeSyntax tOwner, TypeSyntax tValue, string file)
    {
        PropertyName = propertyName;
        TOwner = tOwner;
        TValue = tValue;
        File = file;
    }

    public string File
    {
        get;
    }

    public string PropertyName
    {
        get;
    } 

    public TypeSyntax TOwner
    {
        get;
    }

    public TypeSyntax TValue
    {
        get;
    }
}
