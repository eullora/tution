
namespace Zeeble.Mobile
{
    [Preserve(AllMembers = true)]
    public class MaterialFontMarker{}

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property, Inherited = false)]
    public sealed class PreserveAttribute : Attribute
    {
        public bool AllMembers;
        public bool Conditional;

        public PreserveAttribute()
        {
        }
    }
}
