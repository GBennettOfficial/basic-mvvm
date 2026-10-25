using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace BasicMvvm
{
    public abstract class UserInputBaseVm<TSelf> : BaseVm, INotifyDataErrorInfo
        where TSelf : UserInputBaseVm<TSelf>
    {
        private readonly Dictionary<string, List<IPropertyRule<TSelf>>> _rules = new();
        private readonly Dictionary<string, HashSet<string>> _dependents = new();
        private readonly Dictionary<string, List<string>> _errors = new();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _errors.Count > 0;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.Values.SelectMany(e => e).ToList();

            return _errors.TryGetValue(propertyName, out var errors)
                ? errors
                : Enumerable.Empty<string>();
        }

        public bool CheckForErrors()
        {
            foreach (var name in _rules.Keys.ToList())
                ValidateIsolatedProperty(name);
            return !HasErrors;
        }

        public bool CheckForErrorsOnProperties(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
                ValidateIsolatedProperty(name);
            return !HasErrors;
        }
        protected PropertyRuleBuilder<TSelf, TProp> RuleFor<TProp>(Expression<Func<TSelf, TProp>> expression)
        {
            var name = SharedFuncs.GetPropertyNameFromFunc(expression);
            return new PropertyRuleBuilder<TSelf, TProp>(this, name, expression.Compile());
        }

        protected override bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (base.SetProperty(ref field, value, propertyName))
            {
                ValidatePropertyAndDependencies(propertyName);
                return true;
            }
            return false;
        }

        protected void SubscribeErrorsChanged(string propertyName, Action action)
        {
            ErrorsChanged += (sender, eventArgs) =>
            {
                if (eventArgs.PropertyName == propertyName)
                {
                    action();
                }
            };
        }

        internal void RegisterRule(string propertyName, IPropertyRule<TSelf> rule)
        {
            if (!_rules.TryGetValue(propertyName, out var list))
                _rules[propertyName] = list = new List<IPropertyRule<TSelf>>();
            list.Add(rule);
        }

        internal void RegisterDependency(string sourceProperty, string dependentProperty)
        {
            if (!_dependents.TryGetValue(sourceProperty, out var set))
                _dependents[sourceProperty] = set = new HashSet<string>();
            set.Add(dependentProperty);
        }

        private void ValidatePropertyAndDependencies(string propertyName)
        {
            ValidateIsolatedProperty(propertyName);

            if (_dependents.TryGetValue(propertyName, out var dependents))
                foreach (var dependent in dependents)
                    ValidateIsolatedProperty(dependent);
        }

        private void ValidateIsolatedProperty(string propertyName)
        {
            if (!_rules.TryGetValue(propertyName, out var rules))
                return;

            var newErrors = rules
                .Select(r => r.Validate((TSelf)this))
                .OfType<string>()
                .ToList();

            var hadErrors = _errors.TryGetValue(propertyName, out var oldErrors);

            if (newErrors.Count > 0)
                _errors[propertyName] = newErrors;
            else
                _errors.Remove(propertyName);

            // Only raise when something actually changed
            if (hadErrors != newErrors.Count > 0 ||
                (hadErrors && !oldErrors!.SequenceEqual(newErrors)))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        } 
    }
}
