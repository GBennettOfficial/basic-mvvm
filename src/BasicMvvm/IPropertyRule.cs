namespace BasicMvvm
{
    internal interface IPropertyRule<TSelf>
    {
        string? Validate(TSelf instance);
    }
}
