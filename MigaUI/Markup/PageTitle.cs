namespace Acorisoft.Miga.UI.Markup
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PageTitleAttribute : Attribute
    {
        public string Group { get; set; }
        public bool UseResourceKey { get; set; }        
        public string Name { get; set; }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class PageTokenAttribute : Attribute
    {
        public Type View { get; set; }
        public Type ViewModel { get; set; }
    }
}