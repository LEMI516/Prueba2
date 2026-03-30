namespace AML.Core.Exceptions;

public sealed class AdapterNotFoundException : Exception
{
    public AdapterNotFoundException(string message)
        : base(message)
    {
    }
}
