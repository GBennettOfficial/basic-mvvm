namespace BasicMvvm
{
    internal sealed class PropertyRule<TSelf, TProp> : IPropertyRule<TSelf>
    {
        private readonly string _propertyName;
        private readonly Func<TSelf, TProp> _getter;
        private readonly Func<TSelf, TProp, bool> _predicate;

        public string Message { get; set; }
        public Func<TSelf, bool>? Condition { get; set; }

        public PropertyRule(string propertyName, Func<TSelf, TProp> getter,
                            Func<TSelf, TProp, bool> predicate, string message)
        {
            _propertyName = propertyName;
            _getter = getter;
            _predicate = predicate;
            Message = message;
        }

        public string? Validate(TSelf instance)
        {
            if (Condition is not null && !Condition(instance))
                return null;

            return _predicate(instance, _getter(instance))
                ? null
                : Message;
        }
    }
}
