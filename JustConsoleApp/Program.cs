using AdvancedProperties;

var advancedObject = new AdvancedObject();
var anotherObject = new AdvancedObject();

advancedObject.PropertyChanged += (s, e) =>
{
    Console.WriteLine(e.PropertyName);
};

advancedObject.BindingContext = anotherObject;