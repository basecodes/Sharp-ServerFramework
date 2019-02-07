using Ssc.Ssc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ssc.Ssc {
    public interface IController {

        List<string> MethodIds { get; }

        void Register<TInterface>(
            Expression<Action<TInterface>> interfaceExpression,
            Expression<Action> implementExpression) 
            where TInterface : class;

        void Register<TDelegate>(
            string methodId,
            Expression<TDelegate> implementExpression)
            where TDelegate : Delegate;

        void Register<TInterface, TDelegate>(
            Expression<Action<TInterface>> interfaceExpression,
            Expression<TDelegate> implementExpression)
            where TDelegate : Delegate;

        void Register(
            string methodId,
            Expression<Action> implementExpression);
    }
}