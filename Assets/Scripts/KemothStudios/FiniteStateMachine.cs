using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KemothStudios.Utility.States
{
    public sealed class FiniteStateMachine
    {
        private StateNode _currentStateNode;
        private Dictionary<Type, StateNode> _stateNodes = new ();

        public void Update()
        {
            if (_currentStateNode != null)
            {
                if (_currentStateNode.TryGetTransition(out ITransition transition))
                    ChangeState(transition);
                _currentStateNode.State.Update();
            }else DebugUtility.LogWarning($"FiniteStateMachine do not have any state to update, call <b>{nameof(ChangeState)}</b> at-least once after create a {nameof(FiniteStateMachine)}.");
        }

        public void SetState(IState state)
        {
            _currentStateNode = AddOrGetStateNode(state);
            _currentStateNode.State.Enter();
        }
        
        private void ChangeState(ITransition transition)
        {
            if(_currentStateNode.State == transition.ToState) return;
            _currentStateNode.State.Exit();
            _currentStateNode = _stateNodes[transition.ToState.GetType()];
            _currentStateNode.State.Enter();
        }

        public void AddTransition(IState fromState, IState toState, IPredicate condition)
        {
            AddOrGetStateNode(fromState).AddTransition(AddOrGetStateNode(toState).State, condition);
        }

        private StateNode AddOrGetStateNode(IState state)
        {
            StateNode stateNode = _stateNodes.GetValueOrDefault(state.GetType());
            if (stateNode == null)
            {
                stateNode = new StateNode(state);
                _stateNodes.Add(state.GetType(), stateNode);
            }

            return stateNode;
        }

        private class StateNode
        {
            public IState State { get; }
            private HashSet<Transition> _transitions;

            public StateNode([DisallowNull] IState state)
            {
                State = state;
                _transitions = new HashSet<Transition>();
            }

            public void AddTransition(IState nextState, IPredicate predicate)
            {
                _transitions.Add(new Transition(nextState, predicate));
            }

            public bool TryGetTransition(out ITransition transition)
            {
                foreach (Transition t in _transitions)
                {
                    if (t.Predicate.Evaluate())
                    {
                        transition = t;
                        return true;
                    }
                }

                transition = null;
                return false;
            }
        }
    }

    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
    }

    public abstract class BaseState : IState
    {
        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Exit()
        {
        }
    }

    public sealed class Transition : ITransition
    {
        public IState ToState { get; }
        public IPredicate Predicate { get; }

        public Transition([DisallowNull] IState toState, [DisallowNull] IPredicate predicate)
        {
            ToState = toState;
            Predicate = predicate;
        }
    }

    public interface ITransition
    {
        IState ToState { get; }
        IPredicate Predicate { get; }
    }

    public sealed class FuncPredicate : IPredicate
    {
        private readonly Func<bool> _condition;

        public FuncPredicate([DisallowNull] Func<bool> condition) => _condition = condition;

        public bool Evaluate()
        {
            return _condition();
        }
    }

    public sealed class TriggerPredicate : IPredicate
    {
        private bool _isTriggerOn;
        public bool Evaluate()
        {
            bool result = _isTriggerOn;
            _isTriggerOn = false;
            return result;
        }

        public void EnableTrigger() => _isTriggerOn = true;
    }

    public interface IPredicate
    {
        bool Evaluate();
    }
}