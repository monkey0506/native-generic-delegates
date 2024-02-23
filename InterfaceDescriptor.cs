namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct InterfaceDescriptor
    {
        public readonly string FullName;
        public readonly string Name;
        public readonly string OriginalName;

        public InterfaceDescriptor(in ClassDescriptor.Builder builder)
        {
            Name = $"INative{builder.Identifier}";
            FullName = $"{Name}{builder.InterfaceTypeArgumentsSourceText}";
            int arity = builder.InterfaceSymbol.Arity;
            var typeParameters = builder.IsAction ?
                arity == 0 ? "" : $"<{Constants.Actions.TypeParameters[arity]}>" :
                $"<{Constants.Funcs.TypeParameters[arity]}>";
            OriginalName = $"{Name}{typeParameters}";
        }
    }
}
