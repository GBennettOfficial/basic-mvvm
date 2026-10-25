using System.Linq.Expressions;

namespace BasicMvvm
{
    public sealed class PropertyRuleBuilder<TSelf, TProp> where TSelf : UserInputBaseVm<TSelf>
    {
        private readonly UserInputBaseVm<TSelf> _owner;
        private readonly string _propertyName;
        private readonly Func<TSelf, TProp> _getter;
        private readonly List<PropertyRule<TSelf, TProp>> _chain = new();

        internal PropertyRuleBuilder(UserInputBaseVm<TSelf> owner, string propertyName, Func<TSelf, TProp> getter)
        {
            _owner = owner;
            _propertyName = propertyName;
            _getter = getter;
        }

        /// <summary>Predicate on the value only.</summary>
        public PropertyRuleBuilder<TSelf, TProp> Must(Func<TProp, bool> predicate,
                                              string message = "'{PropertyName}' is not valid.")
            => Must((_, value) => predicate(value), message);

        /// <summary>Predicate with access to the whole view model (for cross-property rules).</summary>
        public PropertyRuleBuilder<TSelf, TProp> Must(Func<TSelf, TProp, bool> predicate,
                                              string message = "'{PropertyName}' is not valid.")
        {
            var rule = new PropertyRule<TSelf, TProp>(_propertyName, _getter, predicate, message);
            _chain.Add(rule);
            _owner.AddRule(_propertyName, rule);
            return this;
        }

        /// <summary>Overrides the message of the previous rule in the chain.</summary>
        public PropertyRuleBuilder<TSelf, TProp> WithMessage(string message)
        {
            if (_chain.Count == 0)
                throw new InvalidOperationException("WithMessage must follow a rule.");
            _chain[^1].Message = message;
            return this;
        }

        /// <summary>Applies the condition to every preceding rule in this chain (like FluentValidation).</summary>
        public PropertyRuleBuilder<TSelf, TProp> When(Func<TSelf, bool> condition)
        {
            foreach (var rule in _chain)
            {
                var existing = rule.Condition;
                rule.Condition = existing is null ? condition : x => existing(x) && condition(x);
            }
            return this;
        }

        public PropertyRuleBuilder<TSelf, TProp> Unless(Func<TSelf, bool> condition)
            => When(x => !condition(x));

        /// <summary>Re-validate this property whenever another property changes.</summary>
        public PropertyRuleBuilder<TSelf, TProp> DependsOn<TOther>(Expression<Func<TSelf, TOther>> other)
        {
            _owner.AddDependency(UserInputBaseVm<TSelf>.GetPropertyName(other), _propertyName);
            return this;
        }
    }
}
